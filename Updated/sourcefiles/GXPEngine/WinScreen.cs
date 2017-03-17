using System;
using System.Collections.Generic;
using System.Drawing;
using GXPEngine;

public class WinScreen : Canvas {
    private System.Drawing.Image _victoryScreen;
    private Canvas _canvas;
    private MyGame _myGame;
    private Level _level;
    private Font _font;
    private AnimationButton _btnNextLevel;
    private Sounds _sounds;


    public WinScreen(MyGame pMyGame, Level pLevel) : base (Game.main.width, Game.main.height) {
        _sounds = new Sounds();
        _sounds.PlayMedieval();
        _myGame = pMyGame;
        _level = pLevel;
        _victoryScreen = new Bitmap(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\win_screen.png");
        _canvas = new Canvas(500, 515);
        _canvas.x = game.width / 2 - _canvas.width / 2 + 10;
        _canvas.y = game.height / 2 - _canvas.height / 3;

        //_canvas.alpha = 0.3f;
        //_canvas.graphics.Clear(Color.White);

        _btnNextLevel = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\next_level_button.png", 2, 1);
        _btnNextLevel.currentFrame = 0;
        _btnNextLevel.scale = 0.085f;
        //_btnNextLevel.alpha = 0.6f;
        _btnNextLevel.x = game.width / 2 + 50;
        _btnNextLevel.y = game.height - _canvas.height / 2 + 30;
        AddChild(_btnNextLevel);

        AddChild(_canvas);
        
        _font = new Font(MyGame.GetFont(), 32);

        alpha = 0.8f;
        graphics.DrawImage(_victoryScreen, 0, 0);
        _canvas.graphics.DrawString("Score: " + _level.GetPlayer().Score, _font, Brushes.Black, 50, 40);
        _canvas.graphics.DrawString("Time: " + _level.GetHUD().GetFormattedTimer(), _font, Brushes.Black, 70, 100);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (_btnNextLevel.HitTestPoint(Input.mouseX, Input.mouseY)) {
                _btnNextLevel.currentFrame = 1;
             }
        }
        if (Input.GetKeyDown(Key.ENTER)) {
            _btnNextLevel.currentFrame = 1;
        }

        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(Key.ENTER)) {
            if (_btnNextLevel.HitTestPoint(Input.mouseX, Input.mouseY)) {
                _sounds.StopMedieval();
                _btnNextLevel.currentFrame = 0;
                Destroy();
                if (_level.CurrentLevel == 1) {
                    _myGame.LevelCounter = 1;
                    _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
                } else if (_level.CurrentLevel == 2) {
                    _myGame.LevelCounter = 2;
                    _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
                }
                else if (_level.CurrentLevel == 3)
                {
                    _myGame.LevelCounter = 3;
                    _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
                }
                else if (_level.CurrentLevel == 4)
                {
                    _myGame.LevelCounter = 4;
                    _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
                }
                else if (_level.CurrentLevel == 5)
                {
                    _myGame.LevelCounter = 5;
                    _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
                }
                else if (_level.CurrentLevel == 6)
                {
                    _myGame.LevelCounter = 6;
                    _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
                }
                else if (_level.CurrentLevel == 7)
                {
                    _myGame.LevelCounter = 7;
                    _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
                }
                else if (_level.CurrentLevel == 8)
                {
                    _myGame.LevelCounter = 8;
                    _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
                } else if (_level.CurrentLevel == 9) {
                    _myGame.LevelCounter = 9;
                    _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
                } else if (_level.CurrentLevel == 10) {
                    _myGame.LevelCounter = 10;
                    _myGame.SetState(MyGame.GameState.MAINMENU);
                }
            } else {
                _btnNextLevel.currentFrame = 0;
            }
        }
        if (Input.GetKeyUp(Key.ENTER)) {
            _btnNextLevel.currentFrame = 0;
            Destroy();
            if (_level.CurrentLevel == 1) {
                _myGame.LevelCounter = 1;
                _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
            } else if (_level.CurrentLevel == 2) {
                _myGame.LevelCounter = 2;
                _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
            } else if (_level.CurrentLevel == 3) {
                _myGame.LevelCounter = 3;
                _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
            } else if (_level.CurrentLevel == 4) {
                _myGame.LevelCounter = 4;
                _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
            } else if (_level.CurrentLevel == 5) {
                _myGame.LevelCounter = 5;
                _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
            } else if (_level.CurrentLevel == 6) {
                _myGame.LevelCounter = 6;
                _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
            } else if (_level.CurrentLevel == 7) {
                _myGame.LevelCounter = 7;
                _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
            } else if (_level.CurrentLevel == 8) {
                _myGame.LevelCounter = 8;
                _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
            } else if (_level.CurrentLevel == 9) {
                _myGame.LevelCounter = 9;
                _myGame.SetState(MyGame.GameState.LOADINGSCREEN);
            } else if (_level.CurrentLevel == 10) {
                _myGame.LevelCounter = 10;
                _myGame.SetState(MyGame.GameState.MAINMENU);
            }
        }
    }
}
