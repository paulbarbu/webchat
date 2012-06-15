from flaskext.wtf import Form, TextField, validators, SubmitField

#TODO: add reCaptcha field
class ConnectForm(Form):
    nick = TextField('Nick', [validators.Length(min=3), validators.Regexp('\w',
        message="Only alphanumeric characters and underscores allowed!")])
    submit = SubmitField('Connect!')

class ChatForm(Form):
    text = TextField()
    send =  SubmitField('Send!')
    quit = SubmitField('Quit!')
