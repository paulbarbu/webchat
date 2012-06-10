from flaskext.wtf import Form, TextField, validators, SubmitField, Required

#TODO: reCaptcha form
class ConnectForm(Form):
    nick = TextField('Nick', [validators.Length(min=3), validators.Regexp('\w',
        message="Only alphanumeric characters and underscores allowed!")])
    submit = SubmitField('Connect!')

class ChatForm(Form):
    text = TextField(validators=[Required('Your message cannot be empty!')])
    send =  SubmitField('Send!')
    quit = SubmitField('Quit!')
