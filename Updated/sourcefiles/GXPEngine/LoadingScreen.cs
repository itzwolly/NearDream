using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using System.Drawing;

public class LoadingScreen : Canvas
{
    private Font _font;
    private AnimationSprite _loadingBall;
    private int _timer;
    private MyGame _myGame;

    public LoadingScreen(MyGame pMygame) : base(Game.main.width, Game.main.height) {
        _myGame = pMygame;
        _font = new Font(MyGame.GetFont(), 44);
        _loadingBall = new AnimationSprite(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\dragonanim.png", 8, 12);
        _loadingBall.x = game.width - _loadingBall.width;
        _loadingBall.y = game.height - _loadingBall.height;
        _loadingBall.currentFrame = 38;
        AddChild(_loadingBall);

        graphics.DrawString("Loading...", _font, new SolidBrush(Color.FromArgb(255, 127, 129, 65)), game.width / 2 - 50, game.height / 2 - 50);

        new Timer(1000, _myGame.LoadLevel);
        new Timer(3000, _myGame.StartLevel);
    }

    private void Update() {
        AnimateLoadingBall();
    }

    private void AnimateLoadingBall() {
        _timer++;
        if (_timer == 3) {
            _loadingBall.NextFrame();
            _timer = 0;
            if (_loadingBall.currentFrame == 48) {
                _loadingBall.currentFrame = 38;
                _timer = 0;
            }
        }
        
    }
}
