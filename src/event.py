class Event(object):
    '''Base class for implementing other types of Events'''

    def __init__(self, data):
        self.data = data
        self.format_string = 'event: {type}\ndata: {data}\n\n'

    def __str__(self):
        raise NotImplementedError

class MessageEvent(Event):
    def __str__(self):
        return self.format_string.format(type='message', data=self.data)

class ErrorEvent(Event):
    def __str__(self):
        return self.format_string.format(type='error', data=self.data)
