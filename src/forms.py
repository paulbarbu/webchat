from flaskext.wtf import Form, TextField, validators, SubmitField

#TODO: add reCaptcha field
class ConnectForm(Form):
    nick = TextField('Nick', [validators.Length(min=3), validators.Regexp('\w',
        message="Only alphanumeric characters and underscores allowed!")])
    rooms = TextField('Rooms (separated by spaces)', [validators.Length(min=1), validators.regexp('\w',
        message="Only alphanumeric characters and underscores allowed!"),
        validators.Required()])
    submit = SubmitField('Connect!')

class ChatForm(Form):
    text = TextField()
    room = TextField('room')
    send =  SubmitField('Send!')
    quit = SubmitField('Quit!')
