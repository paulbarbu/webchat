from flaskext.wtf import Form, TextField, validators, SubmitField, HiddenField

#TODO: add reCaptcha field
class ConnectForm(Form):
    nick = TextField('Nick', [validators.Length(min=3), validators.Regexp('\w',
        message="Only alphanumeric characters and underscores allowed!")])
    rooms = TextField('Rooms (separated by spaces)', [validators.regexp('\w',
        message="Only alphanumeric characters and underscores allowed!"),
        validators.Optional()])
    submit = SubmitField('Connect!')

class ChatForm(Form):
    text = TextField()
    rooms = HiddenField('rooms')
    join_rooms = TextField('join rooms (separated by spaces)', [validators.regexp('\w',
        message="Only alphanumeric characters and underscores allowed!"),
        validators.Required("Empty room list not allowed!")])
    join = SubmitField('Join!')
    send = SubmitField('Send!')
    quit = SubmitField('Quit!')
