// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(function () {

    const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    let isLoggedIn = false;

    const authCookie = getCookie("Demo-ClinoTag-Access-Token");

    function getCookie(cname) {
        let name = cname + "=";
        let ca = document.cookie.split(';');
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }
        }
        return "";
    }


    $('#roomListContainer').hide();

    if (authCookie.length > 0)
        isLoggedIn = true;

    console.log("authCookie::::", authCookie);
    console.log("isLoggedIn::::", isLoggedIn);

    let currentRoomId = '';

    connection.start().then(function () {
        console.log("SignalR connected.");
    }).catch(function (err) {
        console.error(err.toString());
    });

    $('#messageButton').click(function () {
        $('#messageModal').modal('show');
        if (isLoggedIn) {
            // Load room list for logged-in user
            $('#roomListContainer').show();
            console.log("roomContainer");
        } else {
            // Load chat interface for non-logged-in user
 
            loadChatInterface();
        }
    });

    connection.invoke("GetRoomList").catch(function (err) {
        return console.error(err.toString());
    });

    connection.on("ReceiveRoomList", function (rooms) {
        console.log("ReceiveRoomList::::");
        rooms.forEach(room => {
            addRoomToList(room);
        });
    });

    connection.on("ReceiveNewRoom", function (room) {
        console.log("ReceiveNewRoom::::", room);
        addRoomToList(room);
    });

    function addRoomToList(room) {
        $('#roomList').append(`<li class="list-group-item">${room}</li>`);
    }

    $('#roomList').on('click', 'li', function () {

        $(this).addClass("active").siblings().removeClass("active");
      
        currentRoomId = $(this).text();
        $('#roomName').text(currentRoomId); 

        connection.invoke("GetRoomMessages", currentRoomId).catch(function (err) {
            return console.error(err.toString());
        });
    });

    connection.on("ReceiveRoomMessages", function (data) {
        console.log("ReceiveRoomMessages::::");
        $('#chatContainer').empty(); // Clear previous messages
        data.messages.forEach(message => {
            $('#chatContainer').append(`<div class="message">${message}</div>`);
        });
    });

    $('#sendMessageButton').click(function () {
        const message = $('#chatMessage').val();
        const roomName = $('#roomName').val(); // Get room name

        console.log("sendMessageButton::::", message);

        if (message && roomName) {
            currentRoomId = roomName;
            connection.invoke("SendMessageToRoom", currentRoomId, message).catch(function (err) {
                return console.error(err.toString());
            });
            $('#chatMessage').val('');
        }
    });

    connection.on("ReceiveMessage", function (message) {
        console.log("ReceiveMessage::::", message);

        if (message.roomId === currentRoomId) {
            const msg = $('<div class="message"></div>').text(message.roomId + ":" + message.text);
            $('#chatContainer').append(msg);
        }
    });

    $("input").attr("autocomplete", "off");
    $("select").attr("autocomplete", "off");
    $('[data-toggle="tooltip"]').tooltip({
        html: true,
        template: '<div class="tooltip" role="tooltip"><div class="tooltip-inner" style="background-color: white;"></div></div>'
    });

    $('[data-toggle="copytooltip"]').click(function () {
        var tooltip = $(this);
        // Create tooltip
        tooltip.tooltip({
            html: true,
            template: '<div class="tooltip" role="tooltip"><div class="tooltip-inner" style="background-color: white;"></div></div>',
            trigger: 'manual'
        });
        // Toggle tooltip
        tooltip.tooltip('toggle');
        // Remove tooltip after 2 seconds
        setTimeout(function () {
            tooltip.tooltip('hide');
        }, 1600);
    });

    $('.copy').on('click', function () {
        var value = $(this).attr('value');

        var content = $('#' + value).text().trim();

        navigator.clipboard.writeText(content)
            .then(function () {
                console.log('Text copied to clipboard successfully.');
            })
            .catch(function (error) {
                console.error('Unable to copy text to clipboard:', error);
            });
    });

});
