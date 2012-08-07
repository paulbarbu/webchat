Handler = {
    /**
     * Display a user's message
     */
    event_message: function handle_event_message(e){
        var rooms = JSON.parse($('#rooms').val());
        var data = JSON.parse(e.data);

        if(-1 == rooms.indexOf(data['room'])){
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

        lineDiv.className = 'line';
        lineDiv.appendChild(timeSpan);
        lineDiv.appendChild(nickSpan);
        lineDiv.appendChild(msgSpan);

        $('#' + data['room']).append(lineDiv);

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
        $.post('/_pong', 'PONG!');
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
}

/**
 * Create an AJAX request that will publish the message the user wants to
 * transmit
 */
function publish_message(e){
    e.preventDefault();
    $.post('/_publish_message', {'message': $('#text').val(),
        'room': $('.tab-pane.active').attr('id')}).fail(Handler.publish_error);
    $('#text').val('');
}

/**
 * Callback for successfully joining rooms after login
 */
function update_rooms(e){
    $('#rooms').val(e);
    display_rooms();
}

/**
 * Displays the rooms as tabs
 *
 * Thsi function is able to decied whether it needs to create all the tabs or
 * just some of the tabs by array difference
 */
function display_rooms(){
    var rooms = JSON.parse($('#rooms').val());
    var pos = 1; //starting from one when joining rooms at login, because the 
                 //first room is displayed outside the for loop since I need 
                 //to set some classes to "active"

    if($('.nav.nav-tabs').length == 0){ //the user joins the rooms at login
        $('#chat').append(
            $('<ul>').attr({class: 'nav nav-tabs'}).append(
                $('<li>').attr({class: 'active'}).append(
                    $('<a>').attr({
                        href: '#' + rooms[0],
                        'data-toggle': 'tab',
                    }).html(rooms[0])
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
        var rooms = rooms.filter(function(i){
            return current_rooms.indexOf(i) < 0;
        });

        pos = 0;
    }

    for(i=pos; i < rooms.length; i++){
        $('.nav.nav-tabs').append(
            $('<li>').append(
                $('<a>').attr({
                    href: '#' + rooms[i],
                    'data-toggle': 'tab',
                }).html(rooms[i])
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
    $.post('/_join_rooms', {'join_rooms': $('#join_rooms').val()})
        .fail(Handler.join_error).success(update_rooms);
    $('#join_rooms').val('');
}

/**
 * Initialize the elements that compose the chat, the EventSource (for the Server Sent
 * Events) and display the rooms the user joined
 */
function load_chat(){
    var stream = new EventSource('/_sse_stream');

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

//Global initializations
load_chat();
$('[name="send"]').click(publish_message);
$('[name="join"]').click(join_rooms);
