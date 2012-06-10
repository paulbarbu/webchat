#! /usr/bin/env python2.7

from flask import Flask, render_template, redirect, url_for, session
from forms import ConnectForm, ChatForm
from simplekv.fs import FilesystemStore
from flaskext.kvsession import KVSessionExtension

app = Flask(__name__)
app.secret_key = 'populate this string yourself!'

store = FilesystemStore('data')
sess_ext = KVSessionExtension(store, app)

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
        #TODO: send AJAX request!
        pass

    return render_template('chat.html', nick=session['nick'], form=form)


if __name__ == '__main__':
    sess_ext.cleanup_sessions()
    app.run(debug=True)
