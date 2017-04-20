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
    private Rectangle _rect;
    private StringFormat _stringFormat = new StringFormat();
    private Map _map;
    

    public LoadingScreen(MyGame pMygame) : base(Game.main.width, Game.main.height) {
        _myGame = pMygame;
        if (_myGame.LevelCounter < 10) {
            TMXParser _tmxParser = new TMXParser();
            _map = _tmxParser.ParseFile(MyGame.GetAssetFilePath(MyGame.Asset.ROOT) + "\\level_" + (_myGame.LevelCounter + 1) + ".tmx");
        }
        

        _stringFormat.Alignment = StringAlignment.Near;
        _stringFormat.LineAlignment = StringAlignment.Near;
        _rect = new Rectangle(game.width / 6, game.height / 10, game.width - game.width / 4, game.height / 2);

        _font = new Font(MyGame.GetFont(), 44);

        AssignLoadingSymbol();
        _loadingBall.x = game.width - _loadingBall.width;
        _loadingBall.y = game.height - _loadingBall.height;
        _loadingBall.currentFrame = 38;
        AddChild(_loadingBall);

        graphics.DrawString("Loading...", _font, new SolidBrush(Color.FromArgb(255, 127, 129, 65)), 50, game.height - 100);

        if (_myGame.LevelCounter == 0) {
            // add text for level one
            SetDidYouKnowText(_map.Properties.GetPropertyByName("Didyouknow").Value);
            new Timer(3000, _myGame.LoadLevelNine);
        } else if (_myGame.LevelCounter == 1) {
            SetDidYouKnowText(_map.Properties.GetPropertyByName("Didyouknow").Value);
            new Timer(5000, _myGame.LoadLevelTwo);
        } else if (_myGame.LevelCounter == 2) {
            SetDidYouKnowText(_map.Properties.GetPropertyByName("Didyouknow").Value);
            new Timer(5000, _myGame.LoadLevelThree);
        }
        else if (_myGame.LevelCounter == 3)
        {
            SetDidYouKnowText(_map.Properties.GetPropertyByName("Didyouknow").Value);
            new Timer(5000, _myGame.LoadLevelFour);
        }
        else if (_myGame.LevelCounter == 4)
        {
            SetDidYouKnowText(_map.Properties.GetPropertyByName("Didyouknow").Value);
            new Timer(5000, _myGame.LoadLevelFive);
        }
        else if (_myGame.LevelCounter == 5)
        {
            SetDidYouKnowText(_map.Properties.GetPropertyByName("Didyouknow").Value);
            new Timer(5000, _myGame.LoadLevelSix);
        }
        else if (_myGame.LevelCounter == 6)
        {
            SetDidYouKnowText(_map.Properties.GetPropertyByName("Didyouknow").Value);
            new Timer(5000, _myGame.LoadLevelSeven);
        }
        else if (_myGame.LevelCounter == 7)
        {
            SetDidYouKnowText(_map.Properties.GetPropertyByName("Didyouknow").Value);
            new Timer(5000, _myGame.LoadLevelEight);
        }
        else if (_myGame.LevelCounter == 8)
        {
            SetDidYouKnowText(_map.Properties.GetPropertyByName("Didyouknow").Value);
            new Timer(1000, _myGame.LoadLevelNine);
        }
        else if (_myGame.LevelCounter == 9)
        {
            SetDidYouKnowText(_map.Properties.GetPropertyByName("Didyouknow").Value);
            new Timer(1000, _myGame.LoadLevelTen);
        }
        else if (_myGame.LevelCounter == 10)
        {
            SetDidYouKnowText(_map.Properties.GetPropertyByName("Didyouknow").Value);
            new Timer(1000, _myGame.LoadMainMenu);
        }
    }

    private void Update() {
        if (_startAnim) {
            AnimateLoadingBall();
        }
    }

    private void SetDidYouKnowText(string pText) {
        graphics.DrawString(pText, _font, Brushes.White, _rect, _stringFormat);
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
