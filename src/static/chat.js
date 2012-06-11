function load_chat(){
    console.log('loaded');
    var stream = new EventSource('/_send_event');

    stream.addEventListener('message', function(e){console.log(e)}, false);
}

load_chat();
