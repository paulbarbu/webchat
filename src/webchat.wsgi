import sys
import os

path = os.path.abspath(__file__)
path = path[:path.rfind('/')]

sys.path.insert(0, path)
from index import app as application
