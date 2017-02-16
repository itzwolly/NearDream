using System;
using GXPEngine;
using System.Drawing;

public class MainMenu : GameObject {
    private AnimationButton btnPlay, btnHowTo, btnQuit;
    private MyGame _myGame;
    private Canvas _backgroundCanvas;
    private System.Drawing.Image _backgroundImage;

    public MainMenu(MyGame myGame) {
        _myGame = myGame;

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

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (btnPlay.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnPlay.currentFrame = 1;
                btnPlay.y += 7;
            } else if (btnHowTo.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnHowTo.currentFrame = 1;
                btnHowTo.y += 7;
            } else if (btnQuit.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnQuit.currentFrame = 1;
                btnQuit.y += 7;
            } 
        }
        if (Input.GetMouseButtonUp(0)) {
            if (btnPlay.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnPlay.currentFrame = 0;
                btnPlay.y -= 7;
                Destroy();
                _myGame.StartGame();
            } else if (btnHowTo.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnHowTo.currentFrame = 0;
                btnHowTo.y -= 7;
                //Destroy();
                // show controls 
            } else if (btnQuit.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnQuit.currentFrame = 0;
                btnQuit.y -= 7;
                Environment.Exit(0);
            }
        }
    }

    void hideMenu() {
        //
    }
}
