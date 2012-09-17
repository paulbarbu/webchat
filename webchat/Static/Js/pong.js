/**
 * Callback to respond to ping events via AJAX
 */
function handle_event_ping(e){
    $.post(Url.PongEvent, 'PONG!');
}