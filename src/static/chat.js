Handler = {
    event_message: function handle_event_message(e){
        var rooms = JSON.parse($('#rooms').val());
        var data = JSON.parse(e.data);

        if(-1 == rooms.indexOf(data['room'])){
            return;
        }

        var lineDiv = document.createElement('div');

        var timeSpan = document.createElement('span');
        var roomSpan = document.createElement('span');
        var nickSpan = document.createElement('span');
        var msgSpan = document.createElement('span');


        timeSpan.innerHTML = get_current_time() + ' ';
        timeSpan.className = 'time';

        roomSpan.innerHTML = '[' + data['room'] + '] ';
        roomSpan.className = 'room';

        nickSpan.innerHTML = data['nick'] + ': ';
        nickSpan.className = 'nick';

        msgSpan.innerHTML = data['message'] + ' ';
        msgSpan.className = 'msg';

        lineDiv.className = 'line';
        lineDiv.appendChild(timeSpan);
        lineDiv.appendChild(roomSpan);
        lineDiv.appendChild(nickSpan);
        lineDiv.appendChild(msgSpan);

        $('#chat').append(lineDiv);

        //TODO: scroll the div to the bottom of the page when the content is larger
        //than the div
    },

    event_users: function handle_event_users(e){
        var users = $.parseJSON(e.data);
        var usersDiv = $('#user-list')[0];

        usersDiv.innerHTML = '';

        for(i=0; i<users.length-1; i++){
            usersDiv.innerHTML += users[i] + ', ';
        }

        usersDiv.innerHTML += users[users.length-1];
    },

    event_ping: function handle_event_ping(e){
        $.post('/_pong', 'PONG!');
    },

    publish_error: function handle_publish_error(e){
        var lineDiv = document.createElement('div');

        lineDiv.className = 'line error';
        lineDiv.innerHTML = e.responseText;

        $('#chat').append(lineDiv);
    },

    event_error: function handle_event_error(e){
        this.close(); //here, `this` refers to `stream`
        console.log('err:', e);
        //TODO: display some error to the user here (some kind of pop up) with
        //e.data as the err msg, also show a reconnect btn with a timer for
        //auto-reconnect
    },

    join_error: function handle_join_error(e){
        var lineDiv = document.createElement('div');

        lineDiv.className = 'join error';
        lineDiv.innerHTML = e.responseText;

        $('#chat').append(lineDiv);
    },
}

function publish_message(e){
    e.preventDefault();
    $.post('/_publish_message', {'message': $('#text').val(), 'room': $('#room').val()})
        .fail(Handler.publish_error);
    $('#text').val('');
}

function update_rooms(e){
    $('#rooms').val(e);
    load_rooms();
}

function load_rooms(){
    var rooms = JSON.parse($('#rooms').val());
    var room_selector = document.getElementById('room');

    if(room_selector){
        room_selector.options.length = 0;
    }
    else{
        room_selector = document.createElement('select');

        room_selector.id = 'room';


        $('form').append(room_selector);
    }

    for(i=0; i < rooms.length; i++){
        var option = document.createElement('option');
        option.value = option.innerHTML = rooms[i];

        room_selector.appendChild(option);
    }
}

function join_rooms(e){
    e.preventDefault();
    $.post('/_join_rooms', {'join_rooms': $('#join_rooms').val()})
        .fail(Handler.join_error).success(update_rooms);
    $('#join_rooms').val('');
}

function load_chat(){
    var stream = new EventSource('/_sse_stream');

    stream.onmessage = Handler.event_message;
    stream.onerror = Handler.event_error;

    stream.addEventListener('users', Handler.event_users);
    stream.addEventListener('ping', Handler.event_ping);

    load_rooms();
}

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

load_chat();
$('[name="send"]').click(publish_message);
$('[name="join"]').click(join_rooms);
