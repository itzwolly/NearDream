using System;
using GXPEngine;
using System.Drawing;

public class MainMenu : GameObject {
    private AnimationButton btnPlay, btnHowTo, btnQuit;
    private MyGame _myGame;
    private Canvas _backgroundCanvas;
    private System.Drawing.Image _backgroundImage;
    private Sounds _sounds = new Sounds();
    private ControlsScreen _controlsScreen;

    private bool _controlsShown = false;

    public bool ControlsShown {
        get { return _controlsShown; }
        set { _controlsShown = value; }
    }

    public MainMenu(MyGame myGame) {
        _myGame = myGame;
        _sounds.PlayMenuMusic();
        _backgroundCanvas = new Canvas(game.width, game.height);
        AddChild(_backgroundCanvas);

        _backgroundImage = new Bitmap(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\main_menu_background.png");
        _backgroundCanvas.graphics.DrawImage(_backgroundImage, 0, 0);

        btnPlay = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\play_buttons.png", 2, 1);
        btnPlay.scale = 0.1f;
        AddChild(btnPlay);
        btnPlay.x = game.width / 4;
        btnPlay.y = (game.height - btnPlay.height) * 0.5f;

        btnHowTo = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\control_buttons.png", 2, 1);
        btnHowTo.scale = 0.1f;
        AddChild(btnHowTo);
        btnHowTo.x = game.width / 4;
        btnHowTo.y = (game.height + (btnHowTo.height * 0.75f)) * 0.5f;

        btnQuit = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\quit_buttons.png", 2, 1);
        btnQuit.scale = 0.1f;
        AddChild(btnQuit);
        btnQuit.x = (game.width / 4f);
        btnQuit.y = (game.height + (btnQuit.height * 2.185f)) * 0.5f;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (!_controlsShown) {
                if (btnPlay.HitTestPoint(Input.mouseX, Input.mouseY)) {
                    btnPlay.currentFrame = 1;
                } else if (btnHowTo.HitTestPoint(Input.mouseX, Input.mouseY)) {
                    btnHowTo.currentFrame = 1;
                } else if (btnQuit.HitTestPoint(Input.mouseX, Input.mouseY)) {
                    btnQuit.currentFrame = 1;
                }
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            if (!_controlsShown) {
                if (btnPlay.HitTestPoint(Input.mouseX, Input.mouseY)) {
                    btnPlay.currentFrame = 0;
                    _sounds.StopMenuMusic();
                    Destroy();
                    _myGame.LevelCounter = 0;
                    _myGame.StartGame();
                } else {
                    btnPlay.currentFrame = 0;
                }
                    
                if(btnHowTo.HitTestPoint(Input.mouseX, Input.mouseY)) {
                    btnHowTo.currentFrame = 0;
                    _controlsScreen = new ControlsScreen(this);
                    game.AddChild(_controlsScreen);
                    _controlsScreen.x = game.width - _controlsScreen.width / 2;
                    _controlsScreen.y = game.height / 2 + _controlsScreen.height * 0.1f;
                    _controlsShown = true;
                } else {
                    btnHowTo.currentFrame = 0;
                }
                
                if (btnQuit.HitTestPoint(Input.mouseX, Input.mouseY)) {
                    btnQuit.currentFrame = 0;
                    Environment.Exit(0);
                } else {
                    btnQuit.currentFrame = 0;
                }
            }
        }
    }
}
