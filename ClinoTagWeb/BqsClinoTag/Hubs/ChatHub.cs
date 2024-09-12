using BqsClinoTag.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private static Dictionary<string, List<string>> _rooms = new Dictionary<string, List<string>>();

    private readonly CLINOTAGBQSContext _db;
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;

    public ChatHub(CLINOTAGBQSContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
        _apiKey = _configuration["OpenAiSettings:ApiKey"];
    }

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

        ChatHistory chatHistory = new ChatHistory();

        var translation = await TranslateTextAsync(message);

        chatHistory.CreatedTime = DateTime.Now;
        chatHistory.RoomName = roomId;
        chatHistory.Content = message;
        chatHistory.Checked = true;

        _db.ChatHistorys.Add(chatHistory);
        await _db.SaveChangesAsync();

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

    public static List<string> GetMessages(string roomId, CLINOTAGBQSContext db)
    {
        var messages = db.ChatHistorys
                         .Where(ch => ch.RoomName == roomId)
                         .OrderBy(ch => ch.CreatedTime)
                         .Select(ch => ch.Content)
                         .ToList();

        return messages;
    }

    public async Task GetRoomMessages(string roomId)
    {
        var messages = GetMessages(roomId, _db);
        await Clients.Caller.SendAsync("ReceiveRoomMessages", new { roomId, messages });
    }

    private async Task<string> TranslateTextAsync(string text)
    {
        // Dummy translation method (replace with actual translation API call)
        return await Task.FromResult(text);
    }
}
