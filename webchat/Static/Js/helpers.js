/**
 * Get the current time
 *
 * @return string the current time in HH:MM format
 */
function get_current_time() {
    var currentTime = new Date();

    var h = currentTime.getHours();
    var m = currentTime.getMinutes();

    if (h < 10) {
        h = '0' + h;
    }

    if (m < 10) {
        m = '0' + m;
    }

    return h + ':' + m;
}

/**
 * Check if a character is a letter
 *
 * @param l string this should be a single character to be checked
 *
 * @return bool true if l is a letter, else false
 */
function is_letter(l) {
    if ((l >= 'a' && l <= 'z') || (l >= 'A' && l <= 'Z')) {
        return true;
    }

    return false;
}

/**
 * Add horizontal rules to each object in an array
 *
 * If there are existing <hr> elements they are removed and a new one is added
 *
 * @param array obj array of objects to add a horizontal rule to
 */
function add_hr(obj) {
    for (i = 0; i < obj.length; i++) {
        if ($(obj[i]).has('div.line').length) {
            obj[i].innerHTML = obj[i].innerHTML.split('<hr>').join('') + '<hr>';
        }
    }

    handle_update_scrollbar();
}

/**
 * Display an error dialog
 *
 * When this dialog is shown then the user is advied to reconnect because some
 * connection error occurred
 */
function show_error_dialog() {
    $('#error-dialog').dialog({
        resizable: false,
        modal: true,
        draggable: false,
        closeOnEscape: false,
        buttons: {
            'OK': function () {
                window.location.replace(Url.Disconnect);
            }
        },
        open: function (event, ui) {
            $('.ui-dialog-buttonpane button').each(function () {
                $(this).attr('class', 'btn btn-danger');
            });
        },
    });
}

/**
 * Reverse a string
 *
 * @param string s the string to be reversed
 *
 * @return string the reversed string
 */
function reverse_str(s) {
    return s.split('').reverse().join('');
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
function get_last_word(text) {
    var word = '';

    for (i = text.length - 1; i >= 0; i--) {
        if (is_letter(text[i])) {
            word += text[i];
        }
        else {
            break;
        }
    }

    return reverse_str(word);
}

/**
 * Adjust the height of the content where the messages appear in order to keep
 * the whole app on the screen.
 */
function adjust_blocks() {
    var box_h = 0;

    if (!Actionbox.hidden) { //if the toolbar is not hidden then include it's height in the calculation
        box_h = $('#actionbox').outerHeight(true);
    }

    var win_h = $(window).height();
    var footer_h = $('p.footer').outerHeight(true);
    var ab_toolbar = $('.actionbox-toolbar').outerHeight(true);
    var tabs_h = $('.nav.nav-tabs').outerHeight(true);
    var body_margins_h = parseInt($('body').css('margin-top')) +
        parseInt($('body').css('margin-bottom'));

    var block_height = win_h - box_h - footer_h - tabs_h - body_margins_h - ab_toolbar;

    $('#content').css('height', block_height);
    $('#user-list').css('height', block_height);
}

/**
 * Callback for handling SSE errors
 */

function handle_event_error(e) {
    if (e.readyState != EventSource.CLOSED) {
        this.close(); //here, `this` refers to `stream`
        //show_error_dialog(); //https://github.com/paullik/webchat/issues/43
    }
}

/**
 * Toggle the actionbox
 *
 * This also footer's "spaced" class and causes the toggler icon to change 
 * according to the state of the actionbox.
 */
function toggle_actionbox() {
    Actionbox.hidden = !Actionbox.hidden;

    $('#actionbox').slideToggle('fast', function () {
        adjust_blocks();
        handle_update_scrollbar();
    });

    $('#toggler').toggleClass('icon-resize-small')
        .toggleClass('icon-resize-full');

    $('.footer').toggleClass('spaced');
}


/**
 * Update the data-source for typeahead with the users on the current room
 *
 * This should be called if the user chages rooms
 */
function update_typeahead() {
    var autocomplete = $('#text').typeahead();

    autocomplete.data('typeahead').source = Data.users[$('.tab-pane.active').attr('id')];
}

/**
 * Update scrollbars and focus the textbox when moving through tabs
 */
function handle_tab_shown(e) {
    handle_update_scrollbar();
    $('#text').focus();
    update_typeahead();
}

/**
 * This is fired when a tab is clicked on
 */
function handle_tab_show(e) {
    display_users($(e.target).attr('href').slice(1));
    $('#icon-' + $(e.target).attr('href').slice(1)).remove();

    //add a <hr> to the last tab in order to mark the activity on that room
    //since the user moved away
    if (typeof e.relatedTarget !== 'undefined') {
        add_hr($('div' + $(e.relatedTarget).attr('href')));
    }
}

/**
 * Callback to respond to ping events via AJAX
 */
function handle_event_ping(e) {
    $.post(Url.PongEvent, 'PONG!');
}

/**
 * Set the spacing according to the device's resolution
 * More specifically: when on mobile phones, remove some spacing
 */
function set_spacing() {
    if (window.screen.availHeight < 600 && window.screen.availWidth < 600) {
        $('body').css('margin', '0px 0px 0px 0px');
        $('.nav.nav-tabs').css('margin', '0px 0px 0px 0px');
        $('#user-list').css('padding', '0px');
        $('#user-list').css('margin', '1px');
        $('label[for=text]').css('margin-bottom', '0px');
        $('#text').css('margin-bottom', '0px');
        $('#join_rooms').css('margin-bottom', '0px');
        $('.well').css('margin-bottom', '0px');
        $('.help-block').css('margin-bottom', '0px');

        if(document.URL.indexOf('Chat') == -1 ){
            $('#rooms').css('margin-bottom', '0px');
            $('#nick').css('margin-bottom', '0px');
            $('form').css('margin', '0px 0px 0px 0px');
            $('br')[1].remove()
        }
    }
}

/**
 * Modify bootstrap's typeahead in order to provide different behaviour
 */
function override_typeahead(id) {
    /**
     * Modify the matcher method of Typeahead in order to be able to tab-complete
     * the last word written even if it's not at the beginning of the message
     */
    $(id).typeahead().data('typeahead').matcher = function (item) {
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
    $(id).typeahead().data('typeahead').select = function () {
        var len_last_word = get_last_word(this.query).length;
        var val = this.$menu.find('.active').attr('data-value')

        this.$element
            .val(this.$element.val().slice(0, -len_last_word) + this.updater(val))
            .change()

        return this.hide()
    }
}