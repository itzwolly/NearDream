using System;
using System.Collections.Generic;
using GXPEngine;
using System.Drawing;


public class PauseMenu : GameObject {
    MyGame          _myGame;
    AnimationButton _btnResume,
                    _btnRestart,
                    _btnReturnToMainMenu,
                    _btnQuit;
    Canvas          _canvas;
    Level           _level;

    public PauseMenu(MyGame pMyGame, Level pMyLevel) {
        _myGame = pMyGame;
        _level = pMyLevel;
        createCanvas();

        _btnResume = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\resume_pause.png", 2, 1);
        _btnResume.scale = 0.5f;
        AddChild(_btnResume);
        _btnResume.x = (game.width - _btnResume.width) / 2 + _btnResume.width / 2;
        _btnResume.y = ((game.height / 2) - (_btnResume.height * 2));

        _btnRestart = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\restart_pause.png", 2, 1);
        _btnRestart.scale = 0.5f;
        AddChild(_btnRestart);
        _btnRestart.x = (game.width - _btnRestart.width) / 2 + _btnRestart.width / 2;
        _btnRestart.y = ((game.height / 2) - (_btnRestart.height / 2));

        _btnReturnToMainMenu = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\main_menu_pause.png", 2, 1);
        _btnReturnToMainMenu.scale = 0.5f;
        AddChild(_btnReturnToMainMenu);
        _btnReturnToMainMenu.x = (game.width - _btnReturnToMainMenu.width) / 2 + _btnReturnToMainMenu.width / 2;
        _btnReturnToMainMenu.y = ((game.height / 2) + (_btnReturnToMainMenu.height));

        _btnQuit = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\quit_pause.png", 2, 1);
        _btnQuit.scale = 0.5f;
        AddChild(_btnQuit);
        _btnQuit.x = (game.width - _btnQuit.width) / 2 + _btnQuit.width / 2;
        _btnQuit.y = ((game.height / 2) + ((_btnQuit.height * 2) + (_btnQuit.height / 2)));
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (_btnResume.HitTestPoint(Input.mouseX, Input.mouseY)) {
                _btnResume.currentFrame = 1;
            } else if (_btnRestart.HitTestPoint(Input.mouseX, Input.mouseY)) {
                _btnRestart.currentFrame = 1;
            } else if (_btnReturnToMainMenu.HitTestPoint(Input.mouseX, Input.mouseY)) {
                _btnReturnToMainMenu.currentFrame = 1;
            } else if (_btnQuit.HitTestPoint(Input.mouseX, Input.mouseY)) {
                _btnQuit.currentFrame = 1;
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            if (_btnResume.HitTestPoint(Input.mouseX, Input.mouseY)) {
                _btnResume.currentFrame = 0;
                _level.IsPaused = false;
                //_myGame.ShowMouse(false);
                Pausable.UnPause();
                this.Destroy();
            } else if (_btnRestart.HitTestPoint(Input.mouseX, Input.mouseY)) {
                _btnRestart.currentFrame = 0;
                switch (_level.CurrentLevel) {
                    case 1:
                        _level.IsPaused = false;
                        Pausable.UnPause();
                        this.Destroy();
                        //_myGame.SetState(MyGame.GameState.LEVEL1);
                        _myGame.LoadLevelOne();
                        break;
                    case 2:
                        _level.IsPaused = false;
                        Pausable.UnPause();
                        this.Destroy();
                        //_myGame.SetState(MyGame.GameState.LEVEL2);
                        _myGame.LoadLevelTwo();
                        break;
                    case 3:
                        _level.IsPaused = false;
                        Pausable.UnPause();
                        this.Destroy();
                        //_myGame.SetState(MyGame.GameState.LEVEL3);
                        _myGame.LoadLevelThree();
                        break;
                    case 4:
                        _level.IsPaused = false;
                        Pausable.UnPause();
                        this.Destroy();
                        //_myGame.SetState(MyGame.GameState.LEVEL4);
                        _myGame.LoadLevelFour();
                        break;
                    case 5:
                        _level.IsPaused = false;
                        Pausable.UnPause();
                        this.Destroy();
                        //_myGame.SetState(MyGame.GameState.LEVEL5);
                        _myGame.LoadLevelFive();
                        break;
                    case 6:
                        _level.IsPaused = false;
                        Pausable.UnPause();
                        this.Destroy();
                        //_myGame.SetState(MyGame.GameState.LEVEL6);
                        _myGame.LoadLevelSix();
                        break;
                    case 7:
                        _level.IsPaused = false;
                        Pausable.UnPause();
                        this.Destroy();
                        //_myGame.SetState(MyGame.GameState.LEVEL7);
                        _myGame.LoadLevelSeven();
                        break;
                    case 8:
                        _level.IsPaused = false;
                        Pausable.UnPause();
                        this.Destroy();
                        //_myGame.SetState(MyGame.GameState.LEVEL8);
                        _myGame.LoadLevelEight();
                        break;
                    case 9:
                        _level.IsPaused = false;
                        Pausable.UnPause();
                        this.Destroy();
                        //_myGame.SetState(MyGame.GameState.LEVEL9);
                        _myGame.LoadLevelNine();
                        break;
                    case 10:
                        _level.IsPaused = false;
                        Pausable.UnPause();
                        this.Destroy();
                        //_myGame.SetState(MyGame.GameState.LEVEL10);
                        _myGame.LoadLevelTen();
                        break;
                    default:
                        break;
                }
            } else if (_btnReturnToMainMenu.HitTestPoint(Input.mouseX, Input.mouseY)) {
                _btnReturnToMainMenu.currentFrame = 0;
                _level.IsPaused = false;
                Pausable.UnPause();
                this.Destroy();
                _myGame.SetState(MyGame.GameState.MAINMENU);
            } else if (_btnQuit.HitTestPoint(Input.mouseX, Input.mouseY)) {
                _btnQuit.currentFrame = 0;
                Environment.Exit(0);
            }
        }
    }

    private void createCanvas() {
        _canvas = new Canvas(game.width, game.height);
        _canvas.graphics.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(0,0, game.width, game.height));
        _canvas.alpha = 0.6f;
        AddChild(_canvas);
    }
}

