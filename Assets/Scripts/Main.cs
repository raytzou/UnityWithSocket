using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Profiling;

public class Main
{
    private static Main _singleton;
    private static IPEndPoint _endPoint;
    private static TcpClient _client;
    private static NetworkStream _stream;
    public int RetryTime = 0;

    public string Username { get; set; } = "Unknown";
    public bool IsOnline { get; set; }

    public static Main GetSingleton
    {
        get => _singleton ??= new Main(_endPoint);
        set => _singleton = value;
    }

    private Main(IPEndPoint endPoint)
    {
        endPoint = new(IPAddress.Parse("127.0.0.1"), 8787);
        _endPoint = endPoint;
        _client = new();
    }

    public void PrintNetInfo()
    {
        Debug.Log("Address: " + _endPoint.Address);
        Debug.Log("Port Number: " + _endPoint.Port);
        Debug.Log("Client Name: " + Username);
    }

    public void ConnectToServer()
    {
        if (IsOnline) return;
        if(RetryTime >= 5)
        {
            Debug.LogError("Cannot connect to server >_O");
            return;
        }

        try
        {
            _client.Connect(_endPoint);
            _stream = _client.GetStream();
            //Debug.LogWarning("client: " + (_client is null) + " stream: " + (_stream is null) + " endpoint: " + (_endPoint is null)); // f f f, idk why
            IsOnline = true;
        }
        catch (Exception ex)
        {
            //Debug.LogWarning("client: " + (_client is null) + " stream: " + (_stream is null) + " endpoint: " + (_endPoint is null)); // f t f
            Debug.LogError(ex.Message);
            IsOnline = false;
            _stream?.Close();
            RetryTime++;
        }
    }

    public void SendMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        var msgBytes = Encoding.UTF8.GetBytes(Username + "$" + message + "$" + _endPoint.Address);
        _stream.Write(msgBytes, 0, msgBytes.Length);
    }

    // https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/cancel-an-async-task-or-a-list-of-tasks
    public async Task<string> ReceiveMessage() // async Task, use it on your own risk
    {
        try
        {
            var buffer = new byte[256];
            var receiver = await _stream.ReadAsync(buffer);
            var decoder = Encoding.UTF8.GetString(buffer, 0, receiver);

            var msgArray = decoder.Split("$");

            if (msgArray.Length >= 3)
            {
                Debug.LogWarning("decoder: " + decoder);
                if (msgArray[0] == "Msg")
                    return decoder;
                else
                    Debug.Log(msgArray[2]);

                return string.Empty;

            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            //Debug.LogWarning("client: " + (_client is null) + " stream: " + (_stream is null) + " endpoint: " + (_endPoint is null)); // f f f
            IsOnline = false;
            return string.Empty;
        }

        return string.Empty;
    }
}
