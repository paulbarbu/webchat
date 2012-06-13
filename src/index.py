#! /usr/bin/env python2.7

from flask import Flask, render_template, redirect, url_for, session, Response, request
from forms import ConnectForm, ChatForm
from simplekv.fs import FilesystemStore
from flaskext.kvsession import KVSessionExtension
from redis import Redis

app = Flask(__name__)
app.secret_key = 'populate this string yourself!'

store = FilesystemStore('data')
sess_ext = KVSessionExtension(store, app)

r = Redis()

def event_stream():
    #TODO: check for errors
    pubsub = r.pubsub()
    pubsub.subscribe('webchat')

    for event in pubsub.listen():
        print event
        yield 'data: {0}\n\n'.format(event)


@app.route('/', methods=['GET', 'POST'])
def index():
    sess_ext.cleanup_sessions()

    if 'nick' in session:
        return redirect(url_for('chat'))

    form = ConnectForm()

    if form.validate_on_submit():
        session['nick'] = form.nick.data
        session.regenerate()

        return redirect(url_for('chat'))

    return render_template('index.html', form=form)


@app.route('/chat', methods=['GET', 'POST'])
def chat():
    sess_ext.cleanup_sessions()
    form = ChatForm()

    if 'nick' not in session: #user has no nickname
        return redirect(url_for('index'))

    if form.quit.data:
        session.destroy()
        sess_ext.cleanup_sessions()

        return redirect(url_for('index'))

    if form.send.data and form.text.validate(form):
        pass
        #return send_event()
        #this part causes the browser to download the data
        #just send the FORM via AJAX and handle the event sending in something
        #like receive_message()

    return render_template('chat.html', nick=session['nick'], form=form)


@app.route('/_receive_message', methods=['POST'])
def receive_message():
    print request.form['message']
    #if the request cannot be fulfilled return 500 with the explanation
    r.publish('webchat', request.form['message'])

    return 'ok'

@app.route('/_send_event')
def send_event():
    return Response(event_stream(), mimetype='text/event-stream',
            headers={'Cache-Control': 'no-cache'})


if __name__ == '__main__':
    sess_ext.cleanup_sessions()
    app.run(debug=True, threaded=True, port=5001)

    #TODO: handle the user logout, via PING-PONG maybe?
    #TODO: check the nick uniqueness
    #TODO: set a lengthier reconnect time for the EventSource
