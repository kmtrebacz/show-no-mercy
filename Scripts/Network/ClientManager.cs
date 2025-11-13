using System;
using System.Diagnostics;
using Godot;
using Lidgren.Network;

namespace ShowNoMercy.Scripts.Network;

public class ClientManager
{
    public NetClient? NetClient { get; private set; }
    public bool IsConnected { get; private set; }
    
    private void ConnectToServer(string host, ushort port)
    {
        if (NetClient != null &&
            (NetClient.ConnectionStatus != NetConnectionStatus.Disconnected ||
             NetClient.ConnectionStatus != NetConnectionStatus.None) &&
            IsConnected)
        {
            GD.Print("Client already connected");
            return;
        }

        NetPeerConfiguration config = new NetPeerConfiguration("ShowNoMercy");
        
        NetClient = new NetClient(config);
        NetClient.Start();

        try
        {
            NetOutgoingMessage message = NetClient.CreateMessage();
            message.Write($"Hi server!");
            
            NetClient.Connect(host, port, message);
            IsConnected = true;
        }
        catch (Exception e)
        {
            GD.Print(e);
        }
    }
    
    public void Start(string ipport)
    {
        string[] split = ipport.Split(':');
        string ip = split[0];
        ushort port = ushort.Parse(split[1]);
        ConnectToServer(ip, port);
    }
    
    public void Update()
    {
        NetIncomingMessage? message;
        while ((message = NetClient.ReadMessage()) != null)
        {
            try
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        HandleStatusChanged(message);
                        break;
                    case NetIncomingMessageType.Data:
                        HandleMessage(message);
                        break;
                }
            }
            finally
            {
                NetClient.Recycle(message);
            }
        }

        NetClient.FlushSendQueue();
    }
    
    private void HandleStatusChanged(NetIncomingMessage message)
    {
        var status = (NetConnectionStatus)message.ReadByte();
        
        switch (status)
        {
            case NetConnectionStatus.Connected:
                GD.Print("Client was connected to the server");
                OnConnected();
                break;
        }
    }

    private void OnConnected()
    {
        var message = NetClient.CreateMessage();
        message.Write((byte)NetworkManager.MessageType.Ready);

        NetClient.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
    }

    private void HandleMessage(NetIncomingMessage message)
    {
        if (message.SenderConnection == null)
        {
            GD.Print("Received message with null sender connection");
            return;
        }
        
        var messageId = (NetworkManager.MessageType) message.ReadByte();
        
        switch (messageId)
        {
            case NetworkManager.MessageType.ServerMeta:
                GD.Print($"[Server] {message.ReadString()}");
                break;
            default:
                GD.Print($"Unhandled message type: {messageId}");
                break;
        }
    }
}