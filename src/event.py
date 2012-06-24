class Event(object):
    '''Base class for implementing other types of Events'''

    format_string = 'event: {type}\ndata: {data}\n\n'

    def __init__(self, data):
        self.data = data

    def __str__(self):
        raise NotImplementedError

class MessageEvent(Event):
    def __str__(self):
        return self.format_string.format(type='message', data=self.data)

class ErrorEvent(Event):
    def __str__(self):
        return self.format_string.format(type='error', data=self.data)

class UsersEvent(Event):
    def __str__(self):
        return self.format_string.format(type='users', data=self.data)

class PingEvent(Event):
    def __init__(self):
        self.data = 'PING!'

    def __str__(self):
        return self.format_string.format(type='ping', data=self.data)
