#! /usr/bin/env python2.7

from flask import (
    Flask, render_template, redirect, url_for, session, Response, request
)
from forms import ConnectForm, ChatForm
from simplekv.fs import FilesystemStore
from flaskext.kvsession import KVSessionExtension
import redis
import logging

import err

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
    try:
        #TODO: sanitize data and pack it into JSON before publishing
        #security: Redis, XSS
        #validity: non-blank msg and len(msg) >= 1
        r.publish('webchat', request.form['message'])
    except redis.ConnectionError as e:
        logging.critical(e)
        return Response(err.ConnectionError, 500)
    except redis.RedisError as e:
        logging.critical(e)
        return Response(err.UnexpectedBackendError, 500)
    except Exception as e:
        logging.critical(e)
        return Response(err.UnexpectedError, 500)

    return 'Received!'


@app.route('/_sse_stream')
def sse_stream():
    return Response(get_event(), mimetype='text/event-stream',
        headers={'Cache-Control': 'no-cache'})


def get_event():
    try:
        pubsub = r.pubsub()
        pubsub.subscribe('webchat')
    except redis.RedisError as e:
        logging.critical(e)
        yield 'event: error\ndata: {0}\n\n'.format(err.UnexpectedBackendError)
    except Exception as e:
        logging.critical(e)
        yield 'event: error\ndata: {0}\n\n'.format(err.UnexpectedError)
    else:
        for event in pubsub.listen():
            yield 'data: {0}\n\n'.format(event['data'])


if __name__ == '__main__':
    logging.basicConfig(filename='logs.log', level=logging.DEBUG,
            format='%(levelname)s: %(asctime)s - %(message)s',
            datefmt='%d-%m-%Y %H:%M:%S')

    sess_ext.cleanup_sessions()
    app.run(debug=True, threaded=True, port=5001)

    #TODO: handle the user logout, via PING-PONG maybe?
    #TODO: show a user list
    #TODO: add class for the yielded events
    #TODO: usage limiter (per user)!
