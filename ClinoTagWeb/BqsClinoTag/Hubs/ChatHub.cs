using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private static Dictionary<string, List<string>> _rooms = new Dictionary<string, List<string>>();

    public async Task SendMessageToRoom(string roomId, string message)
    {
        bool isNewRoom = false;
        if (!_rooms.ContainsKey(roomId))
        {
            _rooms[roomId] = new List<string>();
            isNewRoom = true;
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }
        _rooms[roomId].Add(message);

        var translation = await TranslateTextAsync(message);

        // Broadcast message to all clients in the room
        await Clients.Group(roomId).SendAsync("ReceiveMessage", new { roomId, text = translation });

        if (isNewRoom)
        {
            await Clients.All.SendAsync("ReceiveNewRoom", roomId);
        }
    }

    public async Task JoinRoom(string roomId)
    {
        if (!_rooms.ContainsKey(roomId))
        {
            _rooms[roomId] = new List<string>();
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }

    public static List<string> GetRooms()
    {
        return new List<string>(_rooms.Keys);
    }

    public static List<string> GetMessages(string roomId)
    {
        if (_rooms.ContainsKey(roomId))
        {
            return _rooms[roomId];
        }
        return new List<string>();
    }

    public async Task GetRoomList()
    {
        var rooms = GetRooms();
        await Clients.Caller.SendAsync("ReceiveRoomList", rooms);
    }

    public async Task GetRoomMessages(string roomId)
    {
        var messages = GetMessages(roomId);
        await Clients.Caller.SendAsync("ReceiveRoomMessages", new { roomId, messages });
    }

    private async Task<string> TranslateTextAsync(string text)
    {
        // Dummy translation method (replace with actual translation API call)
        return await Task.FromResult(text);
    }
}
