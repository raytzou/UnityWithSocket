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
    private static TcpClient _client;
    private static NetworkStream _stream;

    public string Username { get; set; } = "Unknown";
    public bool IsOnline { get; set; }

    public static Main GetSingleton
    {
        get => _singleton ??= new Main(_hostEntry, _addressList, _endPoint);
        set => _singleton = value;
    }

    private Main(IPHostEntry hostEntry, IPAddress ipAddress, IPEndPoint ipEndPoint)
    {
        hostEntry = Dns.GetHostEntry("127.0.0.1");
        _hostEntry = hostEntry;
        ipAddress = _hostEntry.AddressList[0];
        _addressList = ipAddress;
        ipEndPoint = new(ipAddress, 8787);
        _endPoint = ipEndPoint;
        _client = new();

        //ConnectToServer();
    }

    public void PrintNetInfo()
    {
        Debug.Log("Address: " + _endPoint.Address);
        Debug.Log("Port Number: " + _endPoint.Port);
        Debug.Log("Client Name: " + Username);
    }

    public void ConnectToServer()
    {
        if (_client.Connected) return;

        try
        {
            _client.Connect(_endPoint);
            _stream = _client.GetStream();
            IsOnline = true;
            Debug.Log("client is online");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            IsOnline = false;
            _client.Close();
            _client = new();
        }
    }

    public void SendMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        var msgBytes = Encoding.UTF8.GetBytes(Username + "$" + message + "$" + _endPoint.Address);
        _stream.Write(msgBytes, 0, msgBytes.Length);
        /*if (IsOnline)
            _stream.Write(msgBytes, 0, msgBytes.Length);
        else
            Debug.LogError("Can't send a message, client is not online");*/
        //client.Close();
    }

    public async Task<string> ReceiveMessage()
    {
        if (!IsOnline)
        {
            Debug.LogError("Client is disconnected, please check server side or restart game!");
            return "";
        }

        try
        {
            var buffer = new byte[256];
            var receiver = await _stream.ReadAsync(buffer);
            var decoder = Encoding.UTF8.GetString(buffer, 0, receiver);

            //Debug.Log(decoder);

            var message = decoder.Split("$");

            if (message[0] == "Ack")
            {
                Debug.Log(message[2]);
            }
            else
            {
                return decoder;
            }

            return "";
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }
}
