/**
 * Get the currently joined rooms
 *
 * @return array of room names
 */
function get_current_rooms() {
    var current_rooms = [];
    $('.tab-pane').each(function () {
        current_rooms.push($(this).attr('id'))
    });

    return current_rooms;
}

/**
 * Callback that handles the join errors
 */
function handle_join_error(e) {
    if ('' == e.responseText) {
        return;
    }

    var lineDiv = document.createElement('div');

    lineDiv.className = 'line alert alert-error';
    lineDiv.innerHTML = e.responseText;

    $('.tab-pane.active').append(lineDiv);
    handle_update_scrollbar();
}

/**
 * Display the rooms as tabs
 *
 * This function is able to decide whether it needs to create all the tabs or
 * just some of the tabs by array difference
 */
function display_rooms() {
    var close_btn = $('<button>').attr({
        class: 'close',
        onclick: 'leave_room($(this).parent().parent(), $("li.active"))'
    }).html('&nbsp;&times;');

    var display_rooms = rooms;

    /**
     * starting from one when joining rooms at login,
     * because the first room is displayed outside the for loop 
     * since I need to set some classes to "active"
     */
    var pos = 1;

    if ($('.nav.nav-tabs').length == 0) { //the user joins the rooms at login
        $('#chat').append(
            $('<ul>').attr({ class: 'nav nav-tabs' }).append(
                $('<li>').attr({ class: 'active' }).append(
                    $('<a>').attr({
                        href: '#' + display_rooms[0],
                        'data-toggle': 'tab',
                    }).html(display_rooms[0] + close_btn.prop('outerHTML'))
                    .on('show', handle_tab_show)
                    .on('shown', handle_tab_shown)
                )
            )
        );

        $('#chat').append(
            $('<div>').attr({ class: 'row-fluid' }).append(
                $('<div>').attr({ class: 'tab-content span11', id: 'content' }).append(
                    $('<div>').attr({
                        class: 'tab-pane active',
                        id: display_rooms[0],
                    })
                )
            ).append(
                $('<div>').attr({ class: 'span2', id: 'user-list' })
            )
        );
    }
    else { //the user joins the rooms after he logged in, so we display only the 
        //new rooms since the rest are already displayed
        var current_rooms = get_current_rooms();

        display_rooms = rooms.filter(function (i) {
            return current_rooms.indexOf(i) < 0;
        });

        /**
         * every room should be processed because if we join we don't
         * have to change the active room
         */
        pos = 0;
    }

    for (i = pos; i < display_rooms.length; i++) {
        $('.nav.nav-tabs').append(
            $('<li>').append(
                $('<a>').attr({
                    href: '#' + display_rooms[i],
                    'data-toggle': 'tab',
                }).html(display_rooms[i] + close_btn.prop('outerHTML'))
                .on('show', handle_tab_show)
                .on('shown', handle_tab_shown)
            )
        );

        $('.tab-content').append(
            $('<div>').attr({
                class: 'tab-pane',
                id: display_rooms[i],
            })
        );

        if (0 == pos && i == display_rooms.length - 1) {
            options = {
                to: $('a[href="' + '#' + display_rooms[i] + '"]'),
                className: 'ui-effects-transfer'
            };
            $('[name="join"]').effect("transfer", options, 700);
        }
    }
}

/**
 * Create an AJAX request to join more rooms after logging in
 */
function join_rooms(e) {
    e.preventDefault();
    $.post(Url.JoinRooms, { 'rooms': $('#join_rooms').val() })
        .fail(handle_join_error)
        .success(function (e) {
            if ("" != e) {
                rooms = JSON.parse(e);
                display_rooms();
                $('#join_rooms').val('');
            }
        });
}

/**
 * Callback for handling the leave error (these occur when leaving rooms)
 */
function handle_leave_room_error(e) {
    //if the status is 404 then the user closed his last room, so we logged
    //him out
    if (404 === e.status) {
        window.location.replace(Url.Index);
    }
    else {
        var lineDiv = document.createElement('div');

        lineDiv.className = 'line alert alert-error';
        lineDiv.innerHTML = e.responseText;

        $('.tab-pane.active').append(lineDiv);
        handle_update_scrollbar();
    }
}

/**
 * Leave a room
 *
 * An AJAX request is made to the server which will respond with the new room
 * list, also the tabs will be updated
 *
 * @param string room the clicked room (the one the user wants to leave)
 * @param string active_room the room currently active
 */
function leave_room(room, active_room) {
    var room_name = room.children().attr("href").slice(1);

    $.post(Url.LeaveRooms, { 'room': room_name })
        .fail(handle_leave_room_error)
        .success(function (e) {
            var active_room_name = active_room.children().attr("href").slice(1);

            if (active_room_name == room_name) {

                var next_room = active_room.next();
                if (next_room.length) { //the current tab is not the last, go right
                    active_room.next().attr('class', 'active');
                }
                else { //current tab is the last, move to left
                    active_room.prev().attr('class', 'active');
                }
            }
            else {
                active_room.attr('class', 'active');
            }

            $(room).remove();
            $('#' + room_name).remove();

            $($('.active').children().attr('href')).addClass('active');

            rooms = JSON.parse(e);
            display_users($('.active').children().attr('href').slice(1));
            $('#text').focus();
            update_typeahead();
        });
}