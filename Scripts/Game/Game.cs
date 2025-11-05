using System.Linq;
using Godot;

namespace ShowNoMercy.Scripts.Game;

public partial class Game : Node
{
	private const string StartMenuScenePath = "res://Scenes/UI/StartMenu.tscn";
	
	public override void _Ready()
	{
		var args = OS.GetCmdlineArgs();

		if (args.Contains("--headless"))
		{
			
		}
		else
		{
			var scene = GD.Load<PackedScene>(StartMenuScenePath);
			var instance = scene.Instantiate();
			AddChild(instance);
		}
	}
}
