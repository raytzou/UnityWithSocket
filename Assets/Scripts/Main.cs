using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Main
{
    private static Main _singleton;
    private static IPHostEntry _hostEntry;
    private static IPAddress _ipAddress;
    private static IPEndPoint _ipEndPoint;

    public string Username { get; set; } = "Unknown";

    public static Main GetSingleton 
    { 
        get => _singleton ??= new Main(_hostEntry, _ipAddress, _ipEndPoint); 
        set => _singleton = value; 
    }
    
    private Main(IPHostEntry hostEntry, IPAddress ipAddress, IPEndPoint ipEndPoint)
    {
        hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        _hostEntry = hostEntry;
        ipAddress = _hostEntry.AddressList[0];
        _ipAddress = ipAddress;
        ipEndPoint = new(ipAddress, 8787);
        _ipEndPoint = ipEndPoint;
    }

    public void PrintNetInfo()
    {
        Debug.Log("Address: " + _ipEndPoint.Address);
        Debug.Log("Port Number: " + _ipEndPoint.Port);
        Debug.Log("Client Name: " + Username);
    }

    public async void SendMessage(string message) // call void but not Task function in Unity looks safer with async programming, but not normal
    {
        if (string.IsNullOrEmpty(message)) return;

        try
        {
            using Socket client = new(_ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            await client.ConnectAsync(_ipEndPoint);

            var msgBytes = Encoding.UTF8.GetBytes(Username + "$" + message + "$" + _ipEndPoint.Address);

            await client.SendAsync(msgBytes, SocketFlags.None);

            client.Close();
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
}
