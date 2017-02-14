using System;
using System.Drawing;
using GXPEngine;
using System.IO;
using System.Collections.Generic;

public class MyGame : Game //MyGame is a Game
{
	private const string ASSETS_FOLDER_NAME = "assets";

	private GameState _state;
	private Level _level;

	public enum Asset {
		NONE,
		ROOT,
		SPRITES,
		SFX,
		VFX,
		MUSIC,
		HUD,
		FONT,
	}

	public enum GameState {
		MAINMENU,
		LEVELSELECTMENU,
		PAUSEMENU,
		LEVEL1,
		LEVEL2,
		LEVEL3,
		WINSCREEN
	}

	//initialize game here
	public MyGame () : base(1600, 960, false, false) {
		targetFps = 60;
		SetState(GameState.LEVEL1);

	}
	
	//update game here
	private void Update () {
		//empty
		if (Input.GetKeyDown(Key.TILDE)) {
			SetState(GameState.LEVEL2);
		}
	}

	public void SetState(GameState pGameState) {
		StopState(_state);
		_state = pGameState;
		StartState(_state);
	}

	void StartState(GameState pGameState) {
		switch (pGameState) {
			case GameState.LEVEL1:
				_level = new Level(1);
				AddChild(_level);
				_level.CreateHUD();
				
				break;
			case GameState.LEVEL2:
				_level = new Level(2);
				AddChild(_level);
				_level.CreateHUD();
				break;
			default:
				break;
		}
	}

	public void StopState(GameState pGameState) {
		
		switch (pGameState) {
			case GameState.LEVEL1:
			case GameState.LEVEL2:
			case GameState.LEVEL3:
				if (_level != null) {
					_level.Destroy();
					_level.GetHUD().Destroy();
					_level = null;
				}
				break;
			default:
				break;
		}
	}

	public static string GetAssetFilePath(Asset pAsset) {
		string path = Directory.GetCurrentDirectory() + "\\" + ASSETS_FOLDER_NAME;
		foreach (string s in Directory.GetDirectories(path)) {
			if (s.Remove(0, path.Length - ASSETS_FOLDER_NAME.Length).ToLower() == ASSETS_FOLDER_NAME + "\\" + pAsset.ToString().ToLower()) {
				return s.Remove(0, path.Length - ASSETS_FOLDER_NAME.Length).ToLower();
			} else if (pAsset == Asset.ROOT) {
				return ASSETS_FOLDER_NAME;
			} else if (pAsset == Asset.NONE) {
				return "";
			}
		}
		return "";
	}

	public GameState GetState() {
		return _state;
	}

	//system starts here
	static void Main()
	{
		new MyGame().Start();
	}
}
