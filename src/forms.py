from flaskext.wtf import Form, TextField, validators, SubmitField

#TODO: reCaptcha form
class ConnectForm(Form):
    nick = TextField('Nick', [validators.Length(min=3), validators.Regexp('\w',
        message="Only alphanumeric characters and underscores allowed!")])
    submit = SubmitField('Connect!')

class DisconnectForm(Form):
    quit = SubmitField('Quit!')
