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

    $('div#content').trigger('update_scrollbar');
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
function adjust_blocks(e) {
    var box_h = 0;

    if (!toolbar_hidden) { //if the toolbar is hidden then don't include it
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
    toolbar_hidden = !toolbar_hidden;

    $('#actionbox').slideToggle('fast', function () {
        adjust_blocks(!toolbar_hidden);
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

    autocomplete.data('typeahead').source = users[$('.tab-pane.active').attr('id')];
}