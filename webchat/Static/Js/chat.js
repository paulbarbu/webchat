/**
 * Update scrollbars and focus the textbox when moving through tabs
 */
function handle_tab_shown(e){
    handle_update_scrollbar();
    $('#text').focus();
    update_typeahead();
}

/**
 * This is fired when a tab is clicked on
 */
function handle_tab_show(e){
    display_users($(e.target).attr('href').slice(1));
    $('#icon-' + $(e.target).attr('href').slice(1)).remove();

    //add a <hr> to the last tab in order to mark the activity on that room
    //since the user moved away
    if(typeof e.relatedTarget !== 'undefined'){
        add_hr($('div' + $(e.relatedTarget).attr('href')));
    }
}

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

    display_rooms();
    display_users($('.tab-pane.active').attr('id'));
    $('#text').attr('data-provide', 'typeahead');
    $('#text').focus();
    update_typeahead();
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

//Global initializations
load_chat();

/**
 * Modify the matcher method of Typeahead in order to be able to tab-complete
 * the last word written even if it's not at the beginning of the message
 */
$('#text').typeahead().data('typeahead').matcher = function (item) {
    var last_word = get_last_word(this.query.toLowerCase());

    if ('' == last_word) {
        return false;
    }

    return ~item.toLowerCase().indexOf(last_word);
};

/**
 * Modify the select method of Typeahead in order to append the tab-completed
 * part to the message, instead of replacing the whole message with the
 * tab-completed word
 */
$('#text').typeahead().data('typeahead').select = function () {
    var len_last_word = get_last_word(this.query).length;
    var val = this.$menu.find('.active').attr('data-value')

    this.$element
        .val(this.$element.val().slice(0, -len_last_word) + this.updater(val))
        .change()

    return this.hide()
}

var away = false;
var toolbar_hidden = false;
var message_list = [];
var current_msg = 0;
var unsent_message = '';

$('[name="send"]').click(publish_message);
$('[name="join"]').click(join_rooms);
$('.actionbox-toolbar').click(toggle_actionbox);
$('div#content').bind('update_scrollbar', handle_update_scrollbar);

$(document).ready(adjust_blocks);
$(window).resize(adjust_blocks);
