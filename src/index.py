#! /usr/bin/env python2.7

from flask import Flask, render_template, redirect, url_for, session, request
from forms import ConnectForm, DisconnectForm
from simplekv.fs import FilesystemStore
from flaskext.kvsession import KVSessionExtension

app = Flask(__name__)
app.secret_key = '\xf8\x8a\xda\r\x81C\xc5O\xc5E\xa6\xb1\xa2"\x8eb\xdb\xca3\xfe\x86eZ%'

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


@app.route('/chat', methods=['GET'])
def chat():
    sess_ext.cleanup_sessions()
    form = DisconnectForm()

    if 'nick' in session:
        return render_template('chat.html', nick=session['nick'], form=form)

    return redirect(url_for('index'))

@app.route('/quit', methods=['POST'])
def quit():
    session.destroy()
    sess_ext.cleanup_sessions()

    return redirect(url_for('index'))


if __name__ == '__main__':
    sess_ext.cleanup_sessions()
    app.run(debug=True)
