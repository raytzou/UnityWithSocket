using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;

public class Main
{
    private static Main _singleton;
    private static IPHostEntry _hostEntry;
    private static IPAddress _addressList;
    private static IPEndPoint _endPoint;
    private static Socket _client;
    private static int _disconnectedTimes = 0;

    public string Username { get; set; } = "Unknown";
    public bool IsOnline { get; set; }

    public static Main GetSingleton 
    { 
        get => _singleton ??= new Main(_hostEntry, _addressList, _endPoint); 
        set => _singleton = value; 
    }
    
    private Main(IPHostEntry hostEntry, IPAddress ipAddress, IPEndPoint ipEndPoint)
    {
        hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        _hostEntry = hostEntry;
        ipAddress = _hostEntry.AddressList[0];
        _addressList = ipAddress;
        ipEndPoint = new(ipAddress, 8787);
        _endPoint = ipEndPoint;

        _client = new(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        //ReceiveMessage();
    }

    public void PrintNetInfo()
    {
        Debug.Log("Address: " + _endPoint.Address);
        Debug.Log("Port Number: " + _endPoint.Port);
        Debug.Log("Client Name: " + Username);
    }

    public async Task ConnectToServer() // calling void but not Task function in Unity looks safer in async programming, but not normal
    {
        //_client.Blocking = false; // God knows how I found this shit
        try
        {
            //Debug.LogError(_client.Connected);
            if (_client.Connected) return;

            await _client.ConnectAsync(_endPoint); // Socket.ConnectAsync is a blocking call, use on your own risk
            IsOnline = _client.Connected;
        }
        catch (Exception ex) 
        {
            //Debug.LogError(ex.Message);
        }
    }

    public async Task SendMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        var msgBytes = Encoding.UTF8.GetBytes(Username + "$" + message + "$" + _endPoint.Address);
        //_client.Blocking = false;

        if (IsOnline)
            await _client.SendAsync(msgBytes, SocketFlags.None);
        else
            Debug.LogError("Can't send a message, client is not online");
        //client.Close();
    }

    public async Task ReceiveMessage() // Wish I would know what I'm doing hmm.....
    {
        if (!IsOnline)
        {
            Debug.LogError("Client is disconnected, please check server side or restart game!");
            return;
        }

        try
        {
            var buffer = new byte[256];
            var receiver = await _client.ReceiveAsync(buffer, SocketFlags.None);
            var decoder = Encoding.UTF8.GetString(buffer, 0, receiver);

            Debug.Log(decoder);
        }
        catch
        {
            // I hate my life
        }
    }
}
