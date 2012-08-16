Webchat
=======

Real time chat for the web, using Server Sent Events (HTML5), AJAX 
and the Python web framework Flask as backend, along with Redis for managing the
NoSQL database the chat is based on.

* SSE are used for one way communication from the server to the client, when a
  new message is sent by an user, the server sends an event to everyone, that
  event will contain the new submitted message.

* Since SSE is a mean of unidirectional communication, the AJAX requests are
  resbonsible for the client to server communication by issuing POST requests to
  the server when the user wants to send a message.

* Redis is used to manage the messages by using it's Pub/Sub commands, this
  provides the blocking interface for the SSE, this is needed because all the
  threads will feed by subscription to channels, where the messages will be
  published as a result of the AJAX requests.

* Flask is the webframework that powers this chat, it handles everything
  described above.

Idea
====
The original idea came from [Dani J](https://github.com/danij).

License
=======

(C) Copyright 2012 Barbu Paul - Gheorghe

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
