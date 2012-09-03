#! /bin/bash
redis-server &
redis-cli FLUSHALL
./ping.py &
./index.py
