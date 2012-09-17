Message = {
    list: [],
    current: 0,
    unsent: '',
};
/**
 * Adds a message to the back of the list
 *
 * @param string msg the message you want to add to the list
 */
function add_message(msg) {
    Message.list.push(msg);
    Message.current = Message.list.length;
}

/**
 * Get the n'th message from the message list
 *
 * @param int n the message you want to retrieve
 *
 * @return the message as a string or an empty string which means the requested
 * message doesn't exist
 */
function get_message(n) {
    return Message.list[n] || '';
}

/**
 * Callback that handles message sending errors
 */
function handle_publish_error(e) {
    if ('' != e.responseText) {
        var lineDiv = document.createElement('div');

        lineDiv.className = 'line alert alert-error';
        lineDiv.innerHTML = e.responseText;

        $('.tab-pane.active').append(lineDiv);
        handle_update_scrollbar();
    }
    else {
        show_error_dialog();
    }
}

/**
 * Create an AJAX request that will publish the message the user wants to
 * transmit
 */
function publish_message(e) {
    e.preventDefault();
    var message = $('#text').val();

    $.post(Url.MessageEvent,
        {
            'message': message,
            'room': $('.tab-pane.active').attr('id')
        })
        .fail(handle_publish_error)
        .success(function () {
            $('#text').val('');
            Message.unsent = '';

            // only store the message if it's not empty
            message && add_message(message);
        });

    $('#text').focus();
}

/**
 * Display a user's message
 */
function handle_event_message(e) {
    var data = JSON.parse(e.data);

    /**
     * don't display the message if the user is not on the room 
     * where the message comes from
     */
    if (-1 === rooms.indexOf(data['room'])) {
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

    if (is_mention(data['message'])) {
        lineDiv.className = 'line alert alert-info';
    }
    else {
        lineDiv.className = 'line';
    }

    lineDiv.appendChild(timeSpan);
    lineDiv.appendChild(nickSpan);
    lineDiv.appendChild(msgSpan);

    $('#' + data['room']).append(lineDiv);

    notify_activity(data['room']);

    handle_update_scrollbar();
}

/**
 * Move the scrollbar at the bottom in order to see the latest message
 */
function handle_update_scrollbar() {
    $('#content').scrollTop($('#content')[0].scrollHeight + 42);
}

/**
 * Allow previous messages navigation in the textarea
 *
 * This is managed through a random access list, the user will get a random
 * element from the message list according to the number of up/down arrw key
 * presses
 */
$(document).keydown(function (e) {
    var start_pos = $('#text').caret().start;
    var end_pos = $('#text').caret().end;
    var len = $('#text').val().length;
    var message = $('#text').val();

    if ('text' == e.target.id &&
       (0 == start_pos && 0 == end_pos || len == start_pos && len == end_pos)) {

        switch (e.keyCode) {
            case 40: //down
                if (Message.current == Message.list.length) {
                    Message.unsent = message;
                    Message.current++;
                }

                if (Message.current < Message.list.length) {
                    Message.current++;
                }

                if (Message.current == Message.list.length) {
                    $('#text').val(Message.unsent);
                }
                else {
                    $('#text').val(get_message(Message.current));
                }

                break;
            case 38: //up
                if (Message.current == Message.list.length) {
                    Message.unsent = message;
                }

                if (Message.current > 0) {
                    Message.current--;
                }

                if (!Message.unsent && Message.current == Message.list.length) {
                    Message.current--;
                }

                if (Message.current == Message.list.length) {
                    $('#text').val(Message.unsent);
                }
                else {
                    $('#text').val(get_message(Message.current));
                }

                break;
        }
    }
});