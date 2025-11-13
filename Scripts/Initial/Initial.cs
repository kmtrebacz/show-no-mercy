using System.Linq;
using Godot;
using ShowNoMercy.Scripts.Network;

namespace ShowNoMercy.Scripts.Initial;

public partial class Initial : Node
{
	private const string StartMenuScenePath = "res://Scenes/UI/StartMenu.tscn";
	
	public override void _Ready()
	{
		var args = OS.GetCmdlineArgs();

		if (args.Contains("--server"))
		{
			NetworkManager.Server.Start();
		}
		else
		{
			var scene = GD.Load<PackedScene>(StartMenuScenePath);
			GetTree().ChangeSceneToPacked(scene);
		}
	}
}
