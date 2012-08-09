#! /bin/sh

if [ -z "$1" ]; then
    echo "Please provide the location where to deploy the application"
    echo "For example if you want to deploy it to /srv/http/webchat"
    echo "The passed path must be /srv/http"

    exit 1
fi

SRC_PATH="/webchat/src"
WSGI_PATH=$SRC_PATH"/webchat.wsgi"
VHOST_NAME="webchat-vhost.conf"

user_path="$1"
DEPLOY_PATH=${user_path%/}

source ./vhost_tmpl > $VHOST_NAME

echo "Don't forget to add"
echo "Include $DEPLOY_PATH$SRC_PATH/webchat-vhost.conf"
echo "to httpd-vhosts.conf in apache."

cd ../..
cp -r --remove-destination webchat $DEPLOY_PATH
