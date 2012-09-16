/**
 * Update the data-source for typeahead with the users on the current room
 *
 * This should be called if the user chages rooms
 */
function update_typeahead() {
    var autocomplete = $('#text').typeahead();

    autocomplete.data('typeahead').source = users[$('.tab-pane.active').attr('id')];
}

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