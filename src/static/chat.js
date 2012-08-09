Handler = {
    /**
     * Display a user's message
     */
    event_message: function handle_event_message(e){
        var rooms = JSON.parse($('#rooms').val());
        var data = JSON.parse(e.data);

        /**
         * don't display the message if the user is not on the room 
         * where the message comes from
         */
        if(-1 === rooms.indexOf(data['room'])){
            return;
        }

        var lineDiv = document.createElement('div');

        var timeSpan = document.createElement('span');
        var nickSpan = document.createElement('span');
        var msgSpan = document.createElement('span');

        timeSpan.innerHTML = get_current_time() + ' ';
        timeSpan.className = 'time';

        nickSpan.innerHTML = data['nick'] + ': ';
        nickSpan.className = 'nick';

        msgSpan.innerHTML = data['message'] + ' ';
        msgSpan.className = 'msg';

        if(is_mention(data['message'])){
            lineDiv.className = 'line mention';
        }
        else{
            lineDiv.className = 'line';
        }

        lineDiv.appendChild(timeSpan);
        lineDiv.appendChild(nickSpan);
        lineDiv.appendChild(msgSpan);

        $('#' + data['room']).append(lineDiv);

        notify_activity(data['room']);

        //TODO: scroll the div to the bottom of the page when the content is larger
        //than the div
    },

    /**
     * Update the user list when someone joins the chat
     */
    event_users: function handle_event_users(e){
        var users = $.parseJSON(e.data);
        var usersDiv = $('#user-list')[0];

        usersDiv.innerHTML = '';

        for(i=0; i<users.length-1; i++){
            usersDiv.innerHTML += users[i] + ', ';
        }

        usersDiv.innerHTML += users[users.length-1];
    },

    /**
     * Callback to respond to ping events via AJAX
     */
    event_ping: function handle_event_ping(e){
        $.post($SCRIPT_ROOT + '/_pong', 'PONG!');
    },

    /**
     * Callback that handles message sending errors
     */
    publish_error: function handle_publish_error(e){
        var lineDiv = document.createElement('div');

        lineDiv.className = 'line error';
        lineDiv.innerHTML = e.responseText;

        $('.tab-pane.active').append(lineDiv);
    },

    /**
     * Callback for handling SSE errors
     */
    event_error: function handle_event_error(e){
        this.close(); //here, `this` refers to `stream`
        console.log('err:', e);
        //TODO: display some error to the user here (some kind of pop up) with
        //e.data as the err msg, also show a reconnect btn with a timer for
        //auto-reconnect
    },

    /**
     * Callback that handles the join errors
     */
    join_error: function handle_join_error(e){
        var lineDiv = document.createElement('div');

        lineDiv.className = 'join error';
        lineDiv.innerHTML = e.responseText;

        $('.tab-pane.active').append(lineDiv);
    },

    /**
     * Callback for handling the leave error (these occur when leaving rooms)
     */
    leave_room_error: function handle_leave_room_error(e){
        //if the status is 404 then the user closed his last room, so we logged
        //him out
        if(404 === e.status){
            window.location.replace($SCRIPT_ROOT);
        }

        var lineDiv = document.createElement('div');

        lineDiv.className = 'leave error';
        lineDiv.innerHTML = e.responseText;

        $('.tab-pane.active').append(lineDiv);
    },
}

/**
 * Create an AJAX request that will publish the message the user wants to
 * transmit
 */
function publish_message(e){
    e.preventDefault();
    $.post($SCRIPT_ROOT + '/_publish_message', {'message': $('#text').val(),
        'room': $('.tab-pane.active').attr('id')}).fail(Handler.publish_error);
    $('#text').val('');
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
function leave_room(room, active_room){
    var room_name = room.children().attr("href").slice(1);

    $.post($SCRIPT_ROOT + '/_leave_room', {'room': room_name})
        .fail(Handler.leave_room_error)
        .success(function(e){
            var active_room_name = active_room.children().attr("href").slice(1);

            if(active_room_name == room_name){

                var next_room = active_room.next();
                if(next_room.length){ //the current tab is not the last, go right
                    active_room.next().attr('class', 'active');
                }
                else{ //current tab is the last, move to left
                    active_room.prev().attr('class', 'active');
                }
            }
            else{
                active_room.attr('class', 'active');
            }

            $(room).remove();
            $('#' + room_name).remove();

            $('#rooms').val(e);
        });
}

/**
 * Display the rooms as tabs
 *
 * This function is able to decide whether it needs to create all the tabs or
 * just some of the tabs by array difference
 */
function display_rooms(){
    var rooms = JSON.parse($('#rooms').val());

    var close_btn = $('<button>').attr({
        class: 'close',
        onclick: 'leave_room($(this).parent().parent(), $("li.active"))'
    }).html('&times;');

    /**
     * starting from one when joining rooms at login,
     * because the first room is displayed outside the for loop 
     * since I need to set some classes to "active"
     */
    var pos = 1;

    if($('.nav.nav-tabs').length == 0){ //the user joins the rooms at login
        $('#chat').append(
            $('<ul>').attr({class: 'nav nav-tabs'}).append(
                $('<li>').attr({class: 'active'}).append(
                    $('<a>').attr({
                        href: '#' + rooms[0],
                        'data-toggle': 'tab',
                    }).html(rooms[0] + close_btn.prop('outerHTML'))
                )
            )
        );

        $('#chat').append(
            $('<div>').attr({class: 'tab-content'}).append(
                $('<div>').attr({
                    class: 'tab-pane active',
                    id: rooms[0],
                })
            )
        );
    }
    else{ //the user joins the rooms after he logged in, so we display only the 
          //new rooms since the rest are already displayed
        var current_rooms = get_current_rooms();

        rooms = rooms.filter(function(i){
            return current_rooms.indexOf(i) < 0;
        });

        /**
         * every room should be processed because if we join we don't
         * have to change the active room
         */
        pos = 0;
    }

    for(i=pos; i<rooms.length; i++){
        $('.nav.nav-tabs').append(
            $('<li>').append(
                $('<a>').attr({
                    href: '#' + rooms[i],
                    'data-toggle': 'tab',
                }).html(rooms[i] + close_btn.prop('outerHTML'))
            )
        );

        $('.tab-content').append(
            $('<div>').attr({
                class: 'tab-pane',
                id: rooms[i],
            })
        );
    }
}

/**
 * Get the currently joined rooms
 *
 * @return array of room names
 */
function get_current_rooms(){
    var current_rooms = [];
    $('.tab-pane').each(function(){
        current_rooms.push($(this).attr('id'))
    });

    return current_rooms;
}

/**
 * Create an AJAX request to join more rooms after logging in
 */
function join_rooms(e){
    e.preventDefault();
    $.post($SCRIPT_ROOT + '/_join_rooms', {'join_rooms': $('#join_rooms').val()})
        .fail(Handler.join_error)
        .success(function(e){
            $('#rooms').val(e);
            display_rooms();
        });
    $('#join_rooms').val('');
}

/**
 * Initialize the elements that compose the chat, the EventSource (for the Server Sent
 * Events) and display the rooms the user joined
 */
function load_chat(){
    var stream = new EventSource($SCRIPT_ROOT + '/_sse_stream');

    stream.onmessage = Handler.event_message;
    stream.onerror = Handler.event_error;

    stream.addEventListener('users', Handler.event_users);
    stream.addEventListener('ping', Handler.event_ping);

    display_rooms();
}

/**
 * Get the current time
 *
 * @return string the current time in HH:MM format
 */
function get_current_time(){
    var currentTime = new Date();

    var h = currentTime.getHours();
    var m = currentTime.getMinutes();

    if(h < 10){
        h = '0' + h;
    }

    if(m < 10){
        m = '0' + m;
    }

    return h + ':' + m;
}

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
function is_mention(message){
    var nick = $('label[for="text"]').html().slice(0, -1);
    var pos = message.indexOf(nick);

    while(-1 !== pos){
        if((pos === 0 || (pos - 1 >= 0 && !is_letter(message[pos-1]))) &&
           (pos+nick.length == message.length || 
                (pos+nick.length < message.length &&
                !is_letter(message[pos+nick.length])
                )
           )){
            return true;
        }

        message = message.slice(pos+nick.length);
        pos = message.indexOf(nick);
    }

    return false;
}

/**
 * Check if a character is a letter
 *
 * @param l string this should be a single character to be checked
 *
 * @return bool true if l is a letter, else false
 */
function is_letter(l){
    if((l >= 'a' && l <= 'z') || (l >= 'A' && l <= 'Z')){
        return true;
    }

    return false;
}

/**
 * Notify the user that there is activity on the chat
 *
 * @param string room_name room's name where is activity
 */
function notify_activity(room_name){
    //create an activity notice if the tab is not focused
    var a = $('a[href="#' + room_name + '"]');
    if('active' !== a.parent().attr('class')){
        if(0 === $('#icon-' + room_name).length){ //prepend just one icon
            a.prepend($('<i>').attr({
                class: 'icon-comment',
                id: 'icon-' + room_name,
            }));
        }
    }

    if(away){
        Notificon("!", {
            font: '10px arial',
            color: '#ffffff',
            stroke: 'rgba(240, 61, 37, 1)',
            width: 7,
        });
    }
}

/**
 * Add horizontal rules to each object in an array
 *
 * If there are existing <hr> elements they are removed and a new one is added
 *
 * @param array obj array of objects to add a horizontal rule to
 */
function add_hr(obj){
    for(i=0; i<obj.length; i++){
        if($(obj[i]).has('div.line').length){
            obj[i].innerHTML = obj[i].innerHTML.split('<hr>').join('') + '<hr>';
        }
    }
}

//Global initializations
load_chat();
$('[name="send"]').click(publish_message);
$('[name="join"]').click(join_rooms);

//clear the activity notice upon clicking on the tab
$('a[data-toggle="tab"]').on('show', function(e){
    $('#icon-' + $(e.target).attr('href').slice(1)).remove();

    //add a <hr> to the last tab in order to mark the activity on that room
    //since the user moved away
    if(typeof e.relatedTarget !== 'undefined'){
        add_hr($('div' + $(e.relatedTarget).attr('href')));
    }
});

//if the browser or the browser's tab is not focused display a Notificon
var away = false;

$(window).focus(function(){
    away = false;
    Notificon('');
});

$(window).blur(function(){
    away = true;

    add_hr($('div.tab-pane'));
});
