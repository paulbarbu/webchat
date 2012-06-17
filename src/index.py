#! /usr/bin/env python2.7

from flask import (
    Flask, render_template, redirect, url_for, session, Response, request, json
)
from forms import ConnectForm, ChatForm
from simplekv.fs import FilesystemStore
from flaskext.kvsession import KVSessionExtension
from jinja2 import utils
import redis
import logging
import time

from event import MessageEvent, ErrorEvent, UsersEvent
import const

app = Flask(__name__)
app.secret_key = 'populate this string yourself!'

store = FilesystemStore('data')
sess_ext = KVSessionExtension(store, app)

r = redis.Redis()

@app.route('/', methods=['GET', 'POST'])
def index():
    sess_ext.cleanup_sessions()

    if 'nick' in session:
        return redirect(url_for('chat'))

    form = ConnectForm()

    errors = []

    if form.validate_on_submit():
        try:
            if r.sismember('users', form.nick.data):
                form.nick.errors.append(const.UsedNickError)
            else:
                r.sadd('users', form.nick.data)
                session['nick'] = form.nick.data
                session.regenerate() # anti session-fixation attack

                try:
                    publish_users()
                except redis.RedisError as e:
                    logging.critical(e)
                    errors.append(const.UnexpectedBackendError)
                else:
                    return redirect(url_for('chat'))
        except redis.RedisError as e:
            logging.critical(e)
            errors.append(const.UnexpectedBackendError)

    return render_template('index.html', form=form, errors=errors)


@app.route('/chat', methods=['GET', 'POST'])
def chat():
    sess_ext.cleanup_sessions()
    form = ChatForm()

    if 'nick' not in session: # user has no nickname (he's not logged in)
        return redirect(url_for('index'))

    if form.quit.data:
        try:
            r.srem('users', session['nick'])
            publish_users()
        except redis.RedisError as e:
            logging.critical(e)

        session.destroy()
        sess_ext.cleanup_sessions()

        return redirect(url_for('index'))

    users = None
    errors = []
    try:
        users = r.sort('users', alpha=True)
    except redis.RedisError as e:
        logging.critical(e)
        errors.append(const.GetUsersError)

    return render_template('chat.html', nick=session['nick'], form=form,
        users=users, errors=errors)


@app.route('/_publish_message', methods=['POST'])
def publish_message():
    if 'nick' not in session:
        return Response(const.NotAuthentifiedError, 403)

    try:
        message = request.form['message'].strip()

        if message and len(message) >= 1:
            r.publish('webchat', json.dumps({
                'message': str(utils.escape(message)),
                'nick': session['nick'],
                'time': time.strftime('%H:%M'),
            }))
        else:
            return Response(const.EmptyMessage, 204)
    except redis.ConnectionError as e:
        logging.critical(e)
        return Response(const.ConnectionError, 500)
    except redis.RedisError as e:
        logging.critical(e)
        return Response(const.UnexpectedBackendError, 500)
    except Exception as e:
        logging.critical(e)
        return Response(const.UnexpectedError, 500)

    return const.Received


@app.route('/_sse_stream')
def sse_stream():
    if 'nick' not in session:
        return Response(const.NotAuthentifiedError, 403)

    try:
        if 0 == r.scard('users'):
            return Response(const.NoUsers, 404)
    except redis.RedisError as e:
        logging.critical(e)

    return Response(get_event(), mimetype='text/event-stream',
        headers={'Cache-Control': 'no-cache'})


def get_event():
    '''Yields an Event object according to the situation'''

    try:
        pubsub = r.pubsub()
        pubsub.subscribe(['webchat', 'webchat.users'])
    except redis.RedisError as e:
        logging.critical(e)
        yield ErrorEvent(const.UnexpectedBackendError)
    except Exception as e:
        logging.critical(e)
        yield ErrorEvent(const.UnexpectedError)
    else:
        for event in pubsub.listen():
            if 'message' == event['type']:
                if 'webchat' == event['channel']:
                    yield MessageEvent(event['data'])
                elif 'webchat.users' == event['channel']:
                    yield UsersEvent(event['data'])


def publish_users():
    users = r.sort('users', alpha=True)
    r.publish('webchat.users', json.dumps(users))

if __name__ == '__main__':
    logging.basicConfig(filename='logs.log', level=logging.DEBUG,
            format='%(levelname)s: %(asctime)s - %(message)s',
            datefmt='%d-%m-%Y %H:%M:%S')

    sess_ext.cleanup_sessions()
    app.run(debug=True, threaded=True, port=5001)

    #TODO: handle the user logout(browser exit, computer restart,
    # network down), via PING-PONG maybe?
    #TODO: show a user list (update on logout)
    #TODO: usage limiter (per user)!
    #TODO: timezones?
    #TODO: on IE the page reloads, not good
    #TODO: try to run the app using mod_wsgi in apache
