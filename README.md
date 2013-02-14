Webchat
=======

Real time chat for the web, using Server Sent Events (HTML5), AJAX 
and ASP.NET MVC (C#) as backend, along with Redis for managing the
NoSQL database the chat is based on.

* SSE are used for one way communication from the server to the client, when a
  new message is sent by an user, the server sends an event to everyone, that
  event will contain the new submitted message. Also events will be sent with 
  other ocasions too, such as pinging the users, notifying a join or a leave
  from a room is also done via SSE.

* Since SSE is a mean of unidirectional communication, the AJAX requests are
  resbonsible for the client to server communication by issuing POST requests to
  the server when the user wants to send a message, these requests are handled 
  by the controllers.

* Redis is used to manage the messages by using it's Pub/Sub commands, this 
  is needed because all the threads will feed by subscription to 
  channels, where the messages will be published as a result of the AJAX 
  requests (hopefully this will change in the future).

* ASP.NET MVC is the webframework that powers this chat, it handles everything
  described above.
  
**Note:** Redis is now replaced by an in-application database which in fact is just 
a static class handled by the threads as if it were a real database, I decided 
to switch because the code is now more clear and there's nothing that Redis 
had and I cannot replicate with this in-application database.

Python
======
The initial version was written in python, you can find it 
[on the python branch](https://github.com/paullik/webchat/tree/python).
  
Idea
====
The original idea came from [Dani J](https://github.com/danij).

License
=======

(C) Copyright 2013 Barbu Paul - Gheorghe

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
