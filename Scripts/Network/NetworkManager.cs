using Godot;

namespace ShowNoMercy.Scripts.Network;

public partial class NetworkManager : Node
{
	public ushort Port = 2137;
	
	public static NetworkManager Instance;
	public static ClientManager Client { get; private set; }
	public static ServerManager Server { get; private set; }
	public enum MessageType : byte
	{
		Ready,
		ServerMeta,
	}
	
	public bool IsServerMode => Server.NetServer != null && Server.IsRunning;
	public bool IsClientMode => Client.NetClient != null && Client.IsConnected;
	
	public override void _Ready()
	{
		Instance = this;
		
		Client = new();
		Server = new();
	}

	public override void _Process(double delta)
	{
		if (IsClientMode)
			Client.Update();
		if (IsServerMode)
			Server.Update();
	}
}
