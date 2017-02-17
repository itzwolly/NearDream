using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using System.Drawing;
using System.IO;

public class LoadingScreen : Canvas
{
    private const string LOADING_SCREEN_FOLDER_NAME = "loadingscreen";

    private Font _font;
    private AnimationSprite _loadingBall;
    private int _timer;
    private MyGame _myGame;
    private int _amountEmptyFrames;
    private bool _startAnim;
    private Random _rnd = new Random();

    public LoadingScreen(MyGame pMygame) : base(Game.main.width, Game.main.height) {
        _myGame = pMygame;
        _font = new Font(MyGame.GetFont(), 44);

        AssignLoadingSymbol();
        _loadingBall.x = game.width - _loadingBall.width;
        _loadingBall.y = game.height - _loadingBall.height;
        _loadingBall.currentFrame = 38;
        AddChild(_loadingBall);

        graphics.DrawString("Loading...", _font, new SolidBrush(Color.FromArgb(255, 127, 129, 65)), game.width / 2 - 60, game.height / 2 - 50);

        if (_myGame.LevelCounter == 0) {
            new Timer(1000, _myGame.LoadLevelOne);
        } else if (_myGame.LevelCounter == 1) {
            new Timer(1000, _myGame.LoadLevelTwo);
        } else if (_myGame.LevelCounter == 2) {
            new Timer(1000, _myGame.LoadLevelThree);
        }
        else if (_myGame.LevelCounter == 3)
        {
            new Timer(1000, _myGame.LoadLevelFour);
        }
        else if (_myGame.LevelCounter == 4)
        {
            new Timer(1000, _myGame.LoadMainMenu);
        }
    }

    private void Update() {
        if (_startAnim) {
            AnimateLoadingBall();
        }
    }

    private void AnimateLoadingBall() {
        _timer++;
        if (_timer == 3) {
            _loadingBall.NextFrame();
            _timer = 0;
            if (_loadingBall.currentFrame == _loadingBall.frameCount - _amountEmptyFrames) {
                _loadingBall.currentFrame = 0;
                _timer = 0;
            }
        }
    }

    private void AssignLoadingSymbol() {
        string combinedName = "\\" + MyGame.ASSETS_FOLDER_NAME + "\\" + LOADING_SCREEN_FOLDER_NAME + "\\";
        string path = Directory.GetCurrentDirectory() + combinedName;
        List<string> fileNames = new List<string>();

        foreach (string s in Directory.GetFiles(path)) {
            fileNames.Add(s.Remove(0, path.Length).ToLower());
        }
        int number = _rnd.Next(0, fileNames.Count);
        string file = fileNames[number];
        string[] colRow = file.Split('_', '.', '-');

        int row = Convert.ToInt32(colRow[1]);
        int col = Convert.ToInt32(colRow[2]);
        _amountEmptyFrames = Convert.ToInt32(colRow[3]);
        bool isReversed = (colRow[4].ToString() == "r");


        if (isReversed) {
            _loadingBall = new AnimationSprite(MyGame.GetAssetFilePath(MyGame.Asset.LOADINGSCREEN) + "\\" + file, row, col);
        } else {
            _loadingBall = new AnimationSprite(MyGame.GetAssetFilePath(MyGame.Asset.LOADINGSCREEN) + "\\" + file, col, row);
        }
        
        _startAnim = true;
    }
}
