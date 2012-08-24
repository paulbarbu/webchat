from flaskext.wtf import (
    Form, TextField, validators, SubmitField, HiddenField, TextAreaField,
    RecaptchaField
)

room_allowed_chars = 'Room names may be composed only of alphanumeric characters and underscores!'

class ConnectForm(Form):
    nick = TextField('Nick', [validators.Length(min=3,
        message='Your nickname must be at least 3 characters long!'), validators.Regexp('\w',
        message='Your nickname must be composed only of alphanumeric characters and underscores!')])
    rooms = TextField('Rooms (separated by spaces)', [validators.regexp('^[\w ]+$',
        message=room_allowed_chars),
        validators.Optional()])
    recaptcha = RecaptchaField()
    submit = SubmitField('Connect!')

class ChatForm(Form):
    text = TextAreaField()
    rooms = HiddenField('rooms')
    join_rooms = TextField('Join rooms (separated by spaces)', [validators.regexp('^[\w ]+$',
        message=room_allowed_chars),
        validators.Required('Empty room list not allowed!')])
    join = SubmitField('Join!')
    send = SubmitField('Send!')
    quit = SubmitField('Quit!')
