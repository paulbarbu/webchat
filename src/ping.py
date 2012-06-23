#! /usr/bin/env python2.7
import redis, logging, time

def ping(r, interval=30):
    '''Publish a ping message to redis (r) every `interval` seconds'''
    try:
        try:
            last_ping = float(r.get('ping.time'))
        except TypeError:
            last_ping = None

        users = list(r.smembers('users'))

        # if there are users connected and interval seconds passed since the
        # last ping then update the ping time and send a ping event
        # see index.py::get_event for how the event is actually sent via SSE
        if len(users) and (not last_ping or last_ping + interval < time.time()):
            r.set('ping.time', time.time())
            r.publish('webchat.ping', 'ping')
    except redis.RedisError as e:
        logging.critical(e)
    else:
        import pdb
        pdb.set_trace()
        time.sleep(20)

if __name__ == '__main__':
    r = redis.Redis()
    logging.basicConfig(filename='logs.log', level=logging.DEBUG,
                        format='%(levelname)s: %(asctime)s - %(message)s',
                        datefmt='%d-%m-%Y %H:%M:%S')

    while True:
        ping(r)
