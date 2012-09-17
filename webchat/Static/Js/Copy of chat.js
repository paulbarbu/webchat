<<<<<<< HEAD
Handler = {
    /**
     * Display a user's message
     */
    event_message: function handle_event_message(e) {
        var data = JSON.parse(e.data);

        /**
         * don't display the message if the user is not on the room 
         * where the message comes from
         */
        if(-1 === rooms.indexOf(data['room'])){
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

        if(is_mention(data['message'])){
            lineDiv.className = 'line alert alert-info';
        }
        else{
            lineDiv.className = 'line';
        }

        lineDiv.appendChild(timeSpan);
        lineDiv.appendChild(nickSpan);
        lineDiv.appendChild(msgSpan);

        $('#' + data['room']).append(lineDiv);

        notify_activity(data['room']);

        Handler.update_scrollbar();
    },

    /**
     * Update the user list when someone joins the chat
     */
    event_users: function handle_event_users(e){
        users = $.parseJSON(e.data);

        current_room = $('.tab-pane.active').attr('id');
        display_users(current_room);
        update_typeahead();
    },

    /**
     * Callback to respond to ping events via AJAX
     */
    event_ping: function handle_event_ping(e){
        $.post(Url.PongEvent, 'PONG!');
    },

    /**
     * Callback that handles message sending errors
     */
    publish_error: function handle_publish_error(e){
        if('' != e.responseText){
            var lineDiv = document.createElement('div');

            lineDiv.className = 'line alert alert-error';
            lineDiv.innerHTML = e.responseText;

            $('.tab-pane.active').append(lineDiv);
            Handler.update_scrollbar();
        }
        else{
            show_error_dialog();
        }
    },

    /**
     * Callback for handling SSE errors
     */
    event_error: function handle_event_error(e) {
        if (e.readyState != EventSource.CLOSED) {
            this.close(); //here, `this` refers to `stream`
            //show_error_dialog(); //https://github.com/paullik/webchat/issues/43
        }
    },

    /**
     * Callback that handles the join errors
     */
    join_error: function handle_join_error(e){
        if('' == e.responseText){
            return;
        }

        var lineDiv = document.createElement('div');

        lineDiv.className = 'line alert alert-error';
        lineDiv.innerHTML = e.responseText;

        $('.tab-pane.active').append(lineDiv);
        Handler.update_scrollbar();
    },

    /**
     * Callback for handling the leave error (these occur when leaving rooms)
     */
    leave_room_error: function handle_leave_room_error(e){
        //if the status is 404 then the user closed his last room, so we logged
        //him out
        if(404 === e.status){
            window.location.replace(Url.Index);
        }
        else{
            var lineDiv = document.createElement('div');

            lineDiv.className = 'line alert alert-error';
            lineDiv.innerHTML = e.responseText;

            $('.tab-pane.active').append(lineDiv);
            Handler.update_scrollbar();
        }
    },

    /**
     * Move the scrollbar at the bottom in order to see the latest message
     */
    update_scrollbar: function handle_update_scrollbar(e){
        $('#content').scrollTop($('#content')[0].scrollHeight + 42);
    },

    /**
     * Update scrollbars and focus the textbox when moving through tabs
     */
    tab_shown: function handle_tab_shown(e){
        Handler.update_scrollbar();
        $('#text').focus();
        update_typeahead();
    },

    /**
     * This is fired when a tab is clicked on
     */
    tab_show: function handle_tab_show(e){
        display_users($(e.target).attr('href').slice(1));
        $('#icon-' + $(e.target).attr('href').slice(1)).remove();

        //add a <hr> to the last tab in order to mark the activity on that room
        //since the user moved away
        if(typeof e.relatedTarget !== 'undefined'){
            add_hr($('div' + $(e.relatedTarget).attr('href')));
        }
    }
}

=======
>>>>>>> multifile-js
/**
 * Update scrollbars and focus the textbox when moving through tabs
 */
<<<<<<< HEAD
function publish_message(e){
    e.preventDefault();
    var message = $('#text').val();

    $.post(Url.MessageEvent, {
            'message': message,
            'room': $('.tab-pane.active').attr('id')
        })
        .fail(Handler.publish_error)
        .success(function(){
            $('#text').val('');
            Message.unsent = '';
            
            // only store the message if it's not empty
            message && add_message(message);
        });

=======
function handle_tab_shown(e){
    handle_update_scrollbar();
>>>>>>> multifile-js
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
<<<<<<< HEAD
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

/**
 * Check if the user is mentioned in the message
 *
 * A user is mentioned when his nick appears as a word in a message:
 * "hello @nick"
 * "nick, hello"
 * "nick: hi"
 * "how are you nick?"
 *
 * The nick may be surrounded by punctuation, symbols or spaces, but not other
 * letters, so for the following message this function will return false:
 * "hello foonick"
 *
 * @param message the message that is checked for mentions
 *
 * @return bool true if the user is mentioned, else false
 */
function is_mention(message){
    var nick = $('label[for="text"]').html();
    var pos = message.indexOf(nick);

    while(-1 !== pos){
        if((pos === 0 || (pos - 1 >= 0 && !is_letter(message[pos-1]))) &&
           (pos+nick.length == message.length || 
                (pos+nick.length < message.length &&
                !is_letter(message[pos+nick.length])
                )
           )){
            return true;
        }

        message = message.slice(pos+nick.length);
        pos = message.indexOf(nick);
    }

    return false;
}

/**
 * Check if a character is a letter
 *
 * @param l string this should be a single character to be checked
 *
 * @return bool true if l is a letter, else false
 */
function is_letter(l){
    if((l >= 'a' && l <= 'z') || (l >= 'A' && l <= 'Z')){
        return true;
    }

    return false;
}

/**
 * Notify the user that there is activity on the chat
 *
 * @param string room_name room's name where is activity
 */
function notify_activity(room_name){
    //create an activity notice if the tab is not focused
    var a = $('a[href="#' + room_name + '"]');
    if('active' !== a.parent().attr('class')){
        if(0 === $('#icon-' + room_name).length){ //prepend just one icon
            a.prepend($('<i>').attr({
                class: 'icon-comment',
                id: 'icon-' + room_name,
            }));
        }
    }

    if(User.away){
        Notificon("!", {
            font: '10px arial',
            color: '#ffffff',
            stroke: 'rgba(240, 61, 37, 1)',
            width: 7,
        });
    }
}

/**
 * Add horizontal rules to each object in an array
 *
 * If there are existing <hr> elements they are removed and a new one is added
 *
 * @param array obj array of objects to add a horizontal rule to
 */
function add_hr(obj){
    for(i=0; i<obj.length; i++){
        if($(obj[i]).has('div.line').length){
            obj[i].innerHTML = obj[i].innerHTML.split('<hr>').join('') + '<hr>';
        }
    }

    Handler.update_scrollbar();
}

/**
 * Adjust the height of the content where the messages appear in order to keep
 * the whole app on the screen.
 */
function adjust_blocks(e) {
    var box_h = 0;

    if(!Actionbox.hidden){ //if the toolbar is not hidden include it's height in the calculation
        box_h = $('#actionbox').outerHeight(true);
    }

    var win_h = $(window).height();
    var footer_h = $('p.footer').outerHeight(true);
    var ab_toolbar = $('.actionbox-toolbar').outerHeight(true);
    var tabs_h = $('.nav.nav-tabs').outerHeight(true);
    var body_margins_h = parseInt($('body').css('margin-top')) +
        parseInt($('body').css('margin-bottom'));

    var block_height = win_h-box_h-footer_h-tabs_h-body_margins_h-ab_toolbar;

    $('#content').css('height', block_height);
    $('#user-list').css('height', block_height);
}

/**
 * Adds a message to the back of the list
 *
 * @param string msg the message you want to add to the list
 */
function add_message(msg){
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
function get_message(n){
    return Message.list[n] || '';
}

/**
=======
>>>>>>> multifile-js
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

<<<<<<< HEAD
/**
 * Display a previous message in the textarea where messages are submitted
 *
 * This is managed through a random access list, the user will get a random
 * element from the message list according to the number of up/down arrw key
 * presses
 */
$(document).keydown(function(e){
    var start_pos = $('#text').caret().start;
    var end_pos = $('#text').caret().end;
    var len = $('#text').val().length;
    var message = $('#text').val();

    if('text' == e.target.id &&
       (0 == start_pos && 0 == end_pos || len == start_pos && len == end_pos)){
        
        switch(e.keyCode){
            case 40: //down
                if(Message.current == Message.list.length){
                    Message.unsent = message;
                    Message.current++;
                }

                if(Message.current < Message.list.length){
                    Message.current++;
                }
                
                if(Message.current == Message.list.length){
                    $('#text').val(Message.unsent);
                }
                else{
                    $('#text').val(get_message(Message.current));
                }

                break;
            case 38: //up
                if(Message.current == Message.list.length){
                    Message.unsent = message;
                }

                if(Message.current > 0){
                    Message.current--;
                }

                if(!Message.unsent && Message.current == Message.list.length){
                    Message.current--;
                }

                if(Message.current == Message.list.length){
                    $('#text').val(Message.unsent);
                }
                else{
                    $('#text').val(get_message(Message.current));
                }

                break;
        }
    }
});

/**
 * Display an error dialog
 *
 * When this dialog is shown then the user is advied to reconnect because some
 * connection error occurred
 */
function show_error_dialog(){
    $('#error-dialog').dialog({
        resizable: false,
        modal: true,
        draggable: false,
        closeOnEscape: false,
        buttons: {
            'OK': function(){
                window.location.replace(Url.Disconnect);
            }
        },
        open: function(event, ui){
            $('.ui-dialog-buttonpane button').each(function(){
                $(this).attr('class', 'btn btn-danger');
            });
        },
    });
}

/**
 * Update the data-source for typeahead with the users on the current room
 *
 * This should be called if the user chages rooms
 */
function update_typeahead(){
    var autocomplete = $('#text').typeahead();

    autocomplete.data('typeahead').source = users[$('.tab-pane.active').attr('id')];
}

/**
 * Get the last word in a string
 *
 * For text = 'hello ', the retval is: ''
 * For text = 'hello ;', the retval is: ''
 * For text = 'hello', the retval is: 'hello'
 * For text = 'hello foo', the retval is: 'foo'
 *
 * @param string text the text from which the word whould be extracted
 *
 * @return string the last word in the passed text
 */
function get_last_word(text){
    var word = '';

    for(i=text.length-1; i>=0; i--){
        if(is_letter(text[i])){
            word += text[i];
        }
        else{
            break;
        }
    }

    return reverse_str(word);
}

/**
 * Reverse a string
 *
 * @param string s the string to be reversed
 *
 * @return string the reversed string
 */
function reverse_str(s){
    return s.split('').reverse().join('');
}

/**
 * Toggle the actionbox
 *
 * This also footer's "spaced" class and causes the toggler icon to change 
 * according to the state of the actionbox.
 */
function toggle_actionbox(){
    Actionbox.hidden = !Actionbox.hidden;

    $('#actionbox').slideToggle('fast', function(){
        adjust_blocks();
        Handler.update_scrollbar();
    });

    $('#toggler').toggleClass('icon-resize-small')
        .toggleClass('icon-resize-full');

    $('.footer').toggleClass('spaced');
}

=======
>>>>>>> multifile-js
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

<<<<<<< HEAD
//if the browser or the browser's tab is not focused display a Notificon
$(window).focus(function(){
    User.away = false;
    Notificon('');
});

$(window).blur(function(){
    User.away = true;

    add_hr($('div.tab-pane'));
});
=======
var away = false;
var toolbar_hidden = false;
var message_list = [];
var current_msg = 0;
var unsent_message = '';
>>>>>>> multifile-js

$('[name="send"]').click(publish_message);
$('[name="join"]').click(join_rooms);
$('.actionbox-toolbar').click(toggle_actionbox);
<<<<<<< HEAD
=======
$('div#content').bind('update_scrollbar', handle_update_scrollbar);
>>>>>>> multifile-js

$(document).ready(adjust_blocks);
$(window).resize(adjust_blocks);
