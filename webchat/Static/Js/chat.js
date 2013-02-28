/**
 * Initialize the elements that compose the chat, the EventSource (for the Server Sent
 * Events) and display the rooms the user joined
 */
function load_chat(){
    var stream = new EventSource("/api/EventStream");

    stream.onmessage = handle_event_message;
    stream.onerror = handle_event_error;

    stream.addEventListener('users', handle_event_users);
    stream.addEventListener('ping', handle_event_ping);
    stream.addEventListener('rooms', handle_event_rooms);

    set_spacing();

    display_rooms();
    display_users($('.tab-pane.active').attr('id'));

    $('#text').attr('data-provide', 'typeahead');
    $('#join_rooms').attr('data-provide', 'typeahead');

    $('#text').focus();
    update_typeahead('#text', Data.users[$('.tab-pane.active').attr('id')]);
    update_typeahead('#join_rooms', Data.all_rooms);
}

/**
 * If the user presses Enter while he's on the message textfield (#text) the
 * message is sent, if he presses Ctrl + Enter a newline is added to the message
 *
 * If the user presses Enter while on the rooms textfield (#join_rooms) he will
 * join them
 */
$(document).keypress(function(e){
    if('join_rooms' == e.target.id && 13 == e.which){
        join_rooms(e);
    }
    else if('text' == e.target.id && (10 == e.which
            //this part is needed in order to solve browser incompatibilities
            || 13 == e.which && e.ctrlKey)) {
        var start_pos = $('#text').caret().start;
        var end_pos = $('#text').caret().end;

        if (start_pos == end_pos) {
            $('#text').val($('#text').val().slice(0, start_pos) + '\n' + $('#text').val().slice(start_pos));

            return false;
        }
    }
    else if('text' == e.target.id && 13 == e.which){
        publish_message(e);
    }
});

load_chat();
override_typeahead('#text');
override_typeahead('#join_rooms');

$('[name="send"]').click(publish_message);
$('[name="join"]').click(join_rooms);
$('.actionbox-toolbar').click(toggle_actionbox);
$('div#content').bind('update_scrollbar', handle_update_scrollbar);

$(document).ready(adjust_blocks);
$(window).resize(adjust_blocks);
