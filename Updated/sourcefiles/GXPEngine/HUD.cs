using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GXPEngine;
using System.Drawing.Text;

public class HUD : Canvas {
    private System.Drawing.Image _timerContainer, _trophyContainer, _trophy, _currentBallContainer, _currentBall;
    private Canvas _timerCanvas, _scoreCanvas, _trophyCanvas, _currentBallCanvas, _changeBallCanvas;

    private List<Canvas> _trophyCanvases = new List<Canvas>();
    private Font _font, _changeBallFont;
    private Level _level;

    private int _timer;

    public HUD(Level pLevel) : base(MyGame.main.width, 100) {
        _level = pLevel;

        _font = new Font(MyGame.GetFont(), 58);
        _changeBallFont = new Font(MyGame.GetFont(), 21);

        _timerContainer = new Bitmap(MyGame.GetAssetFilePath(MyGame.Asset.HUD) + "\\score_container.png");
        _trophyContainer = new Bitmap(MyGame.GetAssetFilePath(MyGame.Asset.HUD) + "\\trophy_container.png");
        _trophy = new Bitmap(MyGame.GetAssetFilePath(MyGame.Asset.HUD) + "\\trophy_dragon.png");
        _currentBallContainer = new Bitmap(MyGame.GetAssetFilePath(MyGame.Asset.HUD) + "\\currentball_container.png");
        _currentBall = new Bitmap(MyGame.GetAssetFilePath(MyGame.Asset.HUD) + "\\regular_ball.png");

        _timerCanvas = new Canvas(200, 100);
        _timerCanvas.x = game.width - 200;
        _timerCanvas.y = y;
        AddChild(_timerCanvas);

        _scoreCanvas = new Canvas(200, 100);
        _scoreCanvas.x = game.width - 400;
        _scoreCanvas.y = y;
        AddChild(_scoreCanvas);

        _currentBallCanvas = new Canvas(100, 100);
        _currentBallCanvas.x = game.width - 600;
        _currentBallCanvas.y = y;
        AddChild(_currentBallCanvas);

        _changeBallCanvas = new Canvas(100, 100);
        _changeBallCanvas.x = game.width - 600;
        _changeBallCanvas.y = y;
        AddChild(_changeBallCanvas);

        if (Convert.ToBoolean(_level.GetMap().Properties.GetPropertyByName("HasTrophies").Value)) {
            for (int i = 0; i < _level.GetTrophyArray().Length; i++) {
                _trophyCanvas = new Canvas(200 / 3, 100);
                _trophyCanvases.Add(_trophyCanvas);
                _trophyCanvases[i].x = (200 / 3) * i;
                _trophyCanvases[i].y = y;
                _trophyCanvases[i].alpha = 0.2f;
                AddChild(_trophyCanvas);
            }
            graphics.DrawImage(_trophyContainer, 0, 0, 200, 100);
            DrawTrophies();
        }
        
        graphics.DrawImage(_timerContainer, game.width - 400, 0, 200, 100);
        graphics.DrawImage(_timerContainer, game.width - 200, 0, 200, 100);
        graphics.DrawImage(_currentBallContainer, game.width - 600, 0, 100, 100);

        DrawCurrentBall(_currentBall);
    }

    private void Update() {
        if (_level.HasLoaded) {
            if (!_level.FinishedLevel) {
                if (!_level.IsPaused) {
                    _timerCanvas.graphics.Clear(Color.Transparent);
                    _scoreCanvas.graphics.Clear(Color.Transparent);
                    _timerCanvas.graphics.DrawString(FormatTimer(), _font, Brushes.Black, 0, 5);
                    _scoreCanvas.graphics.DrawString(_level.GetPlayer().Score.ToString(), _font, Brushes.Black, 0, 5);

                    if (_level.GetBall().IsExploding && _level.GetPlayer().StickyAmount > 0) {
                        _changeBallCanvas.graphics.Clear(Color.Transparent);
                        _changeBallCanvas.graphics.DrawString("E", _changeBallFont, Brushes.Black, 70, 0);
                        _changeBallCanvas.graphics.DrawString("x" + _level.GetPlayer().StickyAmount, _changeBallFont, Brushes.WhiteSmoke, _changeBallCanvas.width - 39, _changeBallCanvas.height - 31);
                    } else {
                        _changeBallCanvas.graphics.Clear(Color.Transparent);
                        _changeBallCanvas.graphics.DrawString("E", _changeBallFont, Brushes.Black, 70, 0);
                    }

                    IncreaseTimer();
                }
            }
        }
    }

    private void DrawCurrentBall(System.Drawing.Image pImage) {
        _currentBallCanvas.graphics.DrawImage(pImage, 15, 32);
    }

    public void ReDrawCurrentBall(bool pIsExploding, int pStickyAmount) {
        if (pIsExploding && pStickyAmount > 0) {
            _currentBall = new Bitmap(MyGame.GetAssetFilePath(MyGame.Asset.HUD) + "\\sticky_ball.png");
        } else {
            _currentBall = new Bitmap(MyGame.GetAssetFilePath(MyGame.Asset.HUD) + "\\regular_ball.png");
        }
        _currentBallCanvas.graphics.Clear(Color.Transparent);
        DrawCurrentBall(_currentBall);
    }

    private void DrawTrophies() {
        for (int i = 0; i < _trophyCanvases.Count; i++) {
            _trophyCanvases[i].graphics.DrawImage(_trophy, 10, 15);
        }
    }

    private void DrawTrophy(int pAmount) {
        for (int i = 0; i < _trophyCanvases.Count; i++) {
            if (i == pAmount - 1) {
                _trophyCanvases[pAmount - 1].graphics.Clear(Color.Transparent);
                _trophyCanvases[pAmount - 1].graphics.DrawImage(_trophy, 10, 15);
            }
        }
    }

    public void ReDrawTrophy(int pAmount) {
        _trophyCanvases[pAmount].alpha = 1f;
        DrawTrophy(pAmount);
    }

    private void IncreaseTimer() {
        _timer += Time.deltaTime;
    }

    public string GetFormattedTimer() {
        return FormatTimer();
    }

    private string FormatTimer() {
        int seconds = (_timer / 1000) % 60;
        int minutes = (_timer / 1000) / 60;
        return (minutes.ToString("00") + ":" + seconds.ToString("00"));
    }

    public List<Canvas> GetCanvases() {
        return _trophyCanvases;
    }
}
