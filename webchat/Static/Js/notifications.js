User = {
    away: false,
};

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
        document.title = '\u2709 Webchat'; // \u2709 is an envelope UTF-8 character
    }
}

$(window).focus(function () {
    User.away = false;
    document.title = 'Webchat';
});

$(window).blur(function () {
    User.away = true;

    add_hr($('div.tab-pane'));
});