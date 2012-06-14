function display_publish_error(){
    //TODO: display an error to the user (into the chat)
    console.log('xhr fail')
}

function publish_message(e){
    e.preventDefault();
    $.post('/_publish_message', {'message': $('input:text').val()})
        .fail(display_publish_error);
}

function handle_event_error(e){
    this.close(); //`this` refers to `stream` in this context
    console.log('err:', e);
    //TODO: display some error to the user here (some kind fo pop up) with
    //e.data as the err msg, also show a reconnect btn with a timer for
    //auto-reconnect
}

function handle_message_event(e){
    //TODO: display the message unpacked from JSON into the chat
    console.log(e);
}

function load_chat(){
    var stream = new EventSource('/_sse_stream');

    stream.onmessage = handle_message_event;
    stream.onerror = handle_event_error;
}

load_chat();
$('[name="send"]').click(publish_message);

//TODO: add class for handle_event_*
