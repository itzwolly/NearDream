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
    private Sprite _cursor;

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
        LEVEL6,
        LEVEL7,
        LEVEL8,
        LEVEL9,
        LEVEL10,
        WINSCREEN
	}

	//initialize game here
	public MyGame () : base(1600, 960, false, false) {
		targetFps = 60;
		_pfc = new PrivateFontCollection();
		_pfc.AddFontFile(MyGame.GetAssetFilePath(MyGame.Asset.FONT) + "\\Augusta.ttf");
		SetState(GameState.MAINMENU);
        _cursor = new Sprite("assets\\sprites\\crosshair.png");
        _cursor.scale = 0.1f;
        _cursor.SetOrigin(width / 2, height / 2);
        AddChild(_cursor);
    }
	
	//update game here
	private void Update () {
        if (_cursor != null)
        {
            _cursor.x = Input.mouseX;
            _cursor.y = Input.mouseY;
        }
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
                _cursor = new Sprite("assets\\sprites\\crosshair.png");
                _cursor.scale = 0.1f;
                _cursor.SetOrigin(width / 2, height / 2);
                break;
			case GameState.LOADINGSCREEN:
				_loadingScreen = new LoadingScreen(this);
				AddChild(_loadingScreen);
				break;
			case GameState.LEVEL1:
				_level = new Level(this, 1);
				break;
            case GameState.LEVEL2:
                _level = new Level(this, 2);
                break;
            case GameState.LEVEL3:
                _level = new Level(this, 3);
                break;
            case GameState.LEVEL4:
                _level = new Level(this, 4);
                break;
            case GameState.LEVEL5:
                _level = new Level(this, 5);
                break;
            case GameState.LEVEL6:
                _level = new Level(this, 6);
                break;
            case GameState.LEVEL7:
                _level = new Level(this, 7);
                break;
            case GameState.LEVEL8:
                _level = new Level(this, 8);
                break;
            case GameState.LEVEL9:
                _level = new Level(this, 9);
                break;
            case GameState.LEVEL10:
                _level = new Level(this, 10);
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
    public void LoadLevelSix()
    {
        SetState(GameState.LEVEL6);
        StartLevel();
    }
    public void LoadLevelSeven()
    {
        SetState(GameState.LEVEL7);
        StartLevel();
    }
    public void LoadLevelEight()
    {
        SetState(GameState.LEVEL8);
        StartLevel();
    }
    public void LoadLevelNine()
    {
        SetState(GameState.LEVEL9);
        StartLevel();
    }
    public void LoadLevelTen()
    {
        SetState(GameState.LEVEL10);
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
                    _cursor.Destroy();
                    _cursor = null;
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
            case GameState.LEVEL6:
            case GameState.LEVEL7:
            case GameState.LEVEL8:
            case GameState.LEVEL9:
            case GameState.LEVEL10:
            case GameState.LEVEL3:
				if (_level != null) {
					_level.HasLoaded = false;
					if (_level.GetHUD() != null) {
						_level.GetHUD().Destroy();
					}
					_level.Destroy();
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

	public Level GetLevel() {
		return _level;
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
