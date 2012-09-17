User = {
    away: false,
};

/**
 * Check if the user is mentioned in the message
 *
 * A user is mentioned when his nick appears as a word in a message:
 * "hello @nick"
 * "nick, hello"
 * "nick: hi"
 * "how are you nick?"
 *
 * The nick may be surrounded by punctuation, symbols or spaces, but not other
 * letters, so for the following message this function will return false:
 * "hello foonick"
 *
 * @param message the message that is checked for mentions
 *
 * @return bool true if the user is mentioned, else false
 */
function is_mention(message) {
    var nick = $('label[for="text"]').html();
    var pos = message.indexOf(nick);

    while (-1 !== pos) {
        if ((pos === 0 || (pos - 1 >= 0 && !is_letter(message[pos - 1]))) &&
           (pos + nick.length == message.length ||
                (pos + nick.length < message.length &&
                !is_letter(message[pos + nick.length])
                )
           )) {
            return true;
        }

        message = message.slice(pos + nick.length);
        pos = message.indexOf(nick);
    }

    return false;
}

/**
 * Notify the user that there is activity on the chat
 *
 * @param string room_name room's name where is activity
 */
function notify_activity(room_name) {
    //create an activity notice if the tab is not focused
    var a = $('a[href="#' + room_name + '"]');
    if ('active' !== a.parent().attr('class')) {
        if (0 === $('#icon-' + room_name).length) { //prepend just one icon
            a.prepend($('<i>').attr({
                class: 'icon-comment',
                id: 'icon-' + room_name,
            }));
        }
    }

    if (User.away) {
        Notificon("!", {
            font: '10px arial',
            color: '#ffffff',
            stroke: 'rgba(240, 61, 37, 1)',
            width: 7,
        });
    }
}


//if the browser or the browser's tab is not focused display a Notificon
$(window).focus(function () {
    User.away = false;
    Notificon('');
});

$(window).blur(function () {
    User.away = true;

    add_hr($('div.tab-pane'));
});