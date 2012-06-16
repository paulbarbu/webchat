function display_publish_error(){
    var lineDiv = document.createElement('div');

    lineDiv.className = 'line error';

    lineDiv.innerHTML = 'Error sending your message!';

    $('#chat').append(lineDiv);
}

function publish_message(e){
    e.preventDefault();
    $.post('/_publish_message', {'message': $('input:text').val()})
        .fail(display_publish_error);
    $('input:text').val('');
}

function handle_event_error(e){
    this.close(); //here, `this` refers to `stream`
    console.log('err:', e);
    //TODO: display some error to the user here (some kind of pop up) with
    //e.data as the err msg, also show a reconnect btn with a timer for
    //auto-reconnect
}

function handle_message_event(e){
    var lineDiv = document.createElement('div');

    var timeSpan = document.createElement('span');
    var nickSpan = document.createElement('span');
    var msgSpan = document.createElement('span');

    var data = JSON.parse(e.data);

    lineDiv.className = 'line';

    timeSpan.innerHTML = data['time'] + ' ';
    timeSpan.className = 'time';

    nickSpan.innerHTML = data['nick'] + ': ';
    nickSpan.className = 'nick';

    msgSpan.innerHTML = data['message'] + ' ';
    msgSpan.className = 'msg';


    lineDiv.appendChild(timeSpan);
    lineDiv.appendChild(nickSpan);
    lineDiv.appendChild(msgSpan);

    $('#chat').append(lineDiv);

    //TODO: scroll the div to the bottom of the page when the content is larger
    //than the div
}

function load_chat(){
    var stream = new EventSource('/_sse_stream');

    stream.onmessage = handle_message_event;
    //stream.addEventListener('message', handle_message_event);
    stream.onerror = handle_event_error;
}

load_chat();
$('[name="send"]').click(publish_message);

//TODO: add class for handle_event_* as a Strategy pattern?
