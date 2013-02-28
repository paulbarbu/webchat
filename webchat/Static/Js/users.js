/**
 * Display the users on the current room
 */
function display_users(current_room) {
    if (!(current_room in Data.users)) {
        return;
    }

    var usersDiv = $('#user-list')[0];
    usersDiv.innerHTML = '';

    for (i = 0; i < Data.users[current_room].length - 1; i++) {
        usersDiv.innerHTML += Data.users[current_room][i] + '<br />';
    }

    usersDiv.innerHTML += Data.users[current_room][Data.users[current_room].length - 1];
}

/**
 * Update the user list when someone joins the chat
 */
function handle_event_users(e) {
    Data.users = $.parseJSON(e.data);

    current_room = $('.tab-pane.active').attr('id');
    display_users(current_room);
    update_typeahead('#text', Data.users[$('.tab-pane.active').attr('id')]);
}