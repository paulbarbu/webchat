function get_status(data){
/*add here a callback that will get the status code of the page and in case of an error it will display an error to the user*/
}

function send_message(e){
    e.preventDefault();
    $.post('/_receive_message', {'message': $('input:text').val()});
}


function load_chat(){
    console.log('loaded');
    var stream = new EventSource('/_send_event');

    stream.addEventListener('message', function(e){console.log(e)}, false);
}

load_chat();
$('[name="send"]').click(send_message);
