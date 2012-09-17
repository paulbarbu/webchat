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