using System;
using System.Net;
using Godot;
using Lidgren.Network;

namespace ShowNoMercy.Scripts.Network;

public class ServerManager
{
    public NetServer NetServer { get; private set; }
    public bool IsRunning { get; private set; }
    
    public void Start(ushort maxClients = 10)
    {
        try
        {
            NetPeerConfiguration config = new NetPeerConfiguration("ShowNoMercy")
            {
                MaximumConnections = maxClients,
                Port = 2137,
                LocalAddress = IPAddress.Parse("0.0.0.0"),
            };

            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            NetServer = new NetServer(config);
            NetServer.Start();
            IsRunning = true;

            Console.WriteLine($"Server started on 0.0.0.0:2137");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
    
    public void Stop()
    {
        NetServer.Shutdown("ShowNoMercy");
        IsRunning = false;
    }
    
    public void Update()
    {
        NetIncomingMessage? message;
        while ((message = NetServer.ReadMessage()) != null)
        {
            try
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        HandleConnectionApproval(message);
                        break;
                    case NetIncomingMessageType.Data:
                        HandleMessage(message);
                        break;
                }

                NetServer.Recycle(message);
            }
            catch
            {
                return;
            }
        }
    }

    private void HandleMessage(NetIncomingMessage message)
    {
        if (message.SenderConnection == null)
        {
            Console.WriteLine("Received message with null sender connection");
            return;
        }
        
        var messageId = (NetworkManager.MessageType) message.ReadByte();
        var connection = message.SenderConnection;
        
        switch (messageId)
        {
            case NetworkManager.MessageType.Ready:
                var outMessage = NetServer.CreateMessage();
                outMessage.Write((byte) NetworkManager.MessageType.ServerMeta);
                outMessage.Write("Hi!");
                NetServer.SendMessage(outMessage, connection, NetDeliveryMethod.ReliableOrdered);
                break;
            default:
                GD.Print($"Unhandled message type: {messageId}");
                break;
        }
    }

    private void HandleConnectionApproval(NetIncomingMessage message)
    {
        if (message.SenderConnection == null)
        {
            GD.Print("Connection approval message received with null sender connection");
            return;
        }
        
        Console.WriteLine(
            $"Client {NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier)} ({message.SenderConnection.RemoteEndPoint.Address}) is connecting...");
        
        NetConnection connection = message.SenderConnection;
                        
        connection.Approve();
        
        GD.Print($"[CLIENT] {message.ReadString()}");
    }
}