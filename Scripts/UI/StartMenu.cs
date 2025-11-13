using Godot;
using ShowNoMercy.Scripts.Network;

namespace ShowNoMercy.Scripts.UI;

public partial class StartMenu : Control
{
	[Export]
	private Button _startButton;
	[Export]
	private Button _quitButton;
	[Export] 
	private Panel _startPanel;
	[Export]
	private LineEdit _ipportLineEdit;
	[Export]
	private Button _connectButton;
	[Export]
	private Button _backButton;
	[Export] 
	private Panel _playPanel;
	
	public override void _Ready()
	{
		_startButton.Pressed += HandleStartButtonPressed;
		_quitButton.Pressed += HandleQuitButtonPressed;
		_backButton.Pressed += HandleBackButtonPressed;
		_connectButton.Pressed += HandleConnectButtonPressed;
		
		_startPanel.SetVisible(true);
		_playPanel.SetVisible(false);
	}
	
	private void HandleStartButtonPressed()
	{
		_startPanel.SetVisible(false);
		_playPanel.SetVisible(true);
	}

	private void HandleQuitButtonPressed()
	{
		GetTree().Quit();
	}

	private void HandleBackButtonPressed()
	{
		_startPanel.SetVisible(true);
		_playPanel.SetVisible(false);
	}

	private void HandleConnectButtonPressed()
	{
		var ipport = _ipportLineEdit.GetText();
		NetworkManager.Client.Start(ipport);
	}
}
