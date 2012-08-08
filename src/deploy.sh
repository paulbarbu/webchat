#! /bin/sh

if [ -z "$1" ]; then
    echo "Please provide the location where to deploy the application"
    echo "For example if you want to deploy it to /srv/http/webchat"
    echo "The passed path must be /srv/http"

    exit 1
fi

SRC_PATH="/src"
WSGI_PATH=$SRC_PATH"/webchat.wsgi"

user_path="$1"
DEPLOY_PATH=${user_path%/}

tmpl=`source vhost_tmpl`
echo $tmpl > webchat-vhost.conf

echo "Don't forget to add"
echo "Include `pwd`/webchat-vhost.conf"
echo "to httpd-vhosts.conf in apache."

cd ../..
cp -r --remove-destination webchat $DEPLOY_PATH
