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
 * Initialize the select tag that specifies the rooms on which the message will
 * be published
 */
function display_rooms(){
    var rooms = JSON.parse($('#rooms').val());
    var room_selector = document.getElementById('room');

    if(room_selector){
        //if the select tag exists, reset it
        room_selector.options.length = 0;
    }
    else{
        //if the select tag doesn't exist, create it
        room_selector = document.createElement('select');

        room_selector.id = 'room';


        $('form').append(room_selector);
    }

    //populate the select tag with options
    for(i=0; i < rooms.length; i++){
        var option = document.createElement('option');
        option.value = option.innerHTML = rooms[i];

        room_selector.appendChild(option);
    }
}

function display_rooms2(){
    var rooms = JSON.parse($('#rooms').val());
    var chat = $('#chat');
    var room_selector = document.createElement('ul');
    var tab = document.createElement('li');
    var a = document.createElement('a');
    var tab_content = document.createElement('div');
    var tab_pane = document.createElement('div');

    room_selector.setAttribute('class', 'nav nav-tabs');
    tab.setAttribute('class', 'active');
    tab_content.setAttribute('class', 'tab-content');

    a.href = '#' + rooms[0];
    a.setAttribute('data-toggle', 'tab');
    a.innerHTML  = rooms[0];

    tab_pane.setAttribute('class', 'tab-pane active');
    tab_pane.id = rooms[0];
    tab_pane.innerHTML = rooms[0];

    $('#chat').append(room_selector);
    room_selector.appendChild(tab);
    tab.appendChild(a);

    chat.append(tab_content);
    tab_content.appendChild(tab_pane);

    for(i=1; i < rooms.length; i++){
        tab = document.createElement('li');
        a = document.createElement('a');
        tab_pane = document.createElement('div');

        a.href = '#' + rooms[i];
        a.setAttribute('data-toggle', 'tab');
        a.innerHTML  = rooms[i];

        tab_pane.setAttribute('class', 'tab-pane');
        tab_pane.id = rooms[i];
        tab_pane.innerHTML = rooms[i];

        room_selector.appendChild(tab);
        tab.appendChild(a);
        tab_content.appendChild(tab_pane);
    }
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

    display_rooms2();
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
