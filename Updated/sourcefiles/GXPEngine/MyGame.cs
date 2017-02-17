using System;
using System.Drawing;
using GXPEngine;
using System.IO;
using System.Collections.Generic;
using System.Drawing.Text;

public class MyGame : Game //MyGame is a Game
{
	public const string ASSETS_FOLDER_NAME = "assets";

	private GameState _state;
	private Level _level;
	private MainMenu _menu;
	private LoadingScreen _loadingScreen;
	private static PrivateFontCollection _pfc;
    private int _levelCounter;

    public int LevelCounter {
        get { return _levelCounter; }
        set { _levelCounter = value; }
    }

    public enum Asset {
		NONE,
		ROOT,
		SPRITES,
		SFX,
		VFX,
		MUSIC,
		HUD,
		FONT,
		UI,
		LOADINGSCREEN
	}

	public enum GameState {
		MAINMENU,
		LOADINGSCREEN,
		LEVELSELECTMENU,
		PAUSEMENU,
		LEVEL1,
		LEVEL2,
        LEVEL3,
        LEVEL4,
        LEVEL5,
        WINSCREEN
	}

	//initialize game here
	public MyGame () : base(1600, 960, false, false) {
		targetFps = 60;
		_pfc = new PrivateFontCollection();
		_pfc.AddFontFile(MyGame.GetAssetFilePath(MyGame.Asset.FONT) + "\\Augusta.ttf");
		SetState(GameState.MAINMENU);
	}
	
	//update game here
	private void Update () {
		//empty
	}

	public void SetState(GameState pGameState) {
		StopState(_state);
		_state = pGameState;
		StartState(_state);
	}

	private void StartState(GameState pGameState) {
		switch (pGameState) {
			case GameState.MAINMENU:
				_menu = new MainMenu(this);
				AddChild(_menu);
				break;
			case GameState.LOADINGSCREEN:
				_loadingScreen = new LoadingScreen(this);
				AddChild(_loadingScreen);
				break;
			case GameState.LEVEL1:
				_level = new Level(this, 1);
				//AddChild(_level);
				break;
            case GameState.LEVEL2:
                _level = new Level(this, 2);
                //AddChild(_level);
                break;
            case GameState.LEVEL3:
                _level = new Level(this, 3);
                //AddChild(_level);
                break;
            case GameState.LEVEL4:
                _level = new Level(this, 4);
                //AddChild(_level);
                break;
            case GameState.LEVEL5:
                _level = new Level(this, 5);
                //AddChild(_level);
                break;
            default:
				break;
		}
	}

	public void StartGame() {
		SetState(GameState.LOADINGSCREEN);
	}

	public void LoadLevelOne() {
        SetState(GameState.LEVEL1);
        StartLevel();
	}

    public void LoadLevelTwo() {
        SetState(GameState.LEVEL2);
        StartLevel();
    }

    public void LoadLevelThree() {
        SetState(GameState.LEVEL3);
        StartLevel();
    }

    public void LoadLevelFour()
    {
        SetState(GameState.LEVEL4);
        StartLevel();
    }
    public void LoadLevelFive()
    {
        SetState(GameState.LEVEL5);
        StartLevel();
    }


    public void LoadMainMenu() {
        SetState(GameState.MAINMENU);
    }

    public void StartLevel() {
		AddChild(_level);
		_level.CreateHUD();

        new Timer(1000, LoadData);
	}

	private void LoadData() {
		_level.HasLoaded = true;
	}

	public void StopState(GameState pGameState) {
		switch (pGameState) {
			case GameState.MAINMENU:
				if (_menu != null) {
					_menu.Destroy();
					_menu = null;
				}
				break;
			case GameState.LOADINGSCREEN:
				if (_loadingScreen != null) {
					_loadingScreen.Destroy();
					_loadingScreen = null;
				}
				break;
			case GameState.LEVEL1:
			case GameState.LEVEL2:
            case GameState.LEVEL4:
            case GameState.LEVEL5:
			case GameState.LEVEL3:
				if (_level != null) {
					_level.HasLoaded = false;
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

	public static FontFamily GetFont() {
		return _pfc.Families[0];
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
