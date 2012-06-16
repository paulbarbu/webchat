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

from event import MessageEvent, ErrorEvent
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

    if form.validate_on_submit():
        #TODO: check the uniqness of the name
        session['nick'] = form.nick.data
        session.regenerate() # anti session-fixation attack

        return redirect(url_for('chat'))

    return render_template('index.html', form=form)


@app.route('/chat', methods=['GET', 'POST'])
def chat():
    sess_ext.cleanup_sessions()
    form = ChatForm()

    if 'nick' not in session: #user has no nickname (he's not logged in)
        return redirect(url_for('index'))

    if form.quit.data:
        session.destroy()
        sess_ext.cleanup_sessions()
        #TODO: update the user list on exit, check if the browser closes
        #without the quit btn being pressed

        return redirect(url_for('index'))

    return render_template('chat.html', nick=session['nick'], form=form)


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

    return Response(get_event(), mimetype='text/event-stream',
        headers={'Cache-Control': 'no-cache'})


def get_event():
    '''Yields an Event object according to the situation'''

    try:
        pubsub = r.pubsub()
        pubsub.subscribe('webchat')
    except redis.RedisError as e:
        logging.critical(e)
        yield ErrorEvent(const.UnexpectedBackendError)
    except Exception as e:
        logging.critical(e)
        yield ErrorEvent(const.UnexpectedError)
    else:
        for event in pubsub.listen():
            yield MessageEvent(event['data'])


if __name__ == '__main__':
    logging.basicConfig(filename='logs.log', level=logging.DEBUG,
            format='%(levelname)s: %(asctime)s - %(message)s',
            datefmt='%d-%m-%Y %H:%M:%S')

    sess_ext.cleanup_sessions()
    app.run(debug=True, threaded=True, port=5001)

    #TODO: handle the user logout, via PING-PONG maybe?
    #TODO: show a user list
    #TODO: usage limiter (per user)!
    #TODO: timezones?
    #TODO: on IE the page reloads, not good
    #TODO: try to run the app using mod_wsgi in apache
