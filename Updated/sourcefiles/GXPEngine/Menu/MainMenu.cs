using System;
using GXPEngine;
using System.Drawing;

public class MainMenu : GameObject {
    private AnimationButton btnPlay, btnQuit;
    private MyGame _myGame;
    private Canvas _backgroundCanvas;
    private System.Drawing.Image _backgroundImage;

    public MainMenu(MyGame myGame) {
        _myGame = myGame;

        _backgroundCanvas = new Canvas(game.width, game.height);
        AddChild(_backgroundCanvas);

        _backgroundImage = new Bitmap(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\main_menu_background.png");
        _backgroundCanvas.graphics.DrawImage(_backgroundImage, 0, 0);

        btnPlay = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\play_button00.png", 2, 1);
        AddChild(btnPlay);
        btnPlay.x = (game.width / 4f) - btnPlay.width * 0.8195f;
        btnPlay.y = (game.height - btnPlay.height) * 0.5f;

        btnQuit = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\quit_button00.png", 2, 1);
        AddChild(btnQuit);
        btnQuit.x = (game.width / 4f) - btnQuit.width;
        btnQuit.y = (game.height + (btnQuit.height * 2)) * 0.50f;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (btnPlay.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnPlay.currentFrame = 1;
                btnPlay.y += 4;
            } else if (btnQuit.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnQuit.currentFrame = 1;
                btnQuit.y += 4;
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            if (btnPlay.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnPlay.currentFrame = 0;
                btnPlay.y -= 4;
                Destroy();
                //_myGame.SetState(MyGame.GameState.LEVEL1);
                _myGame.StartGame();
            } else if (btnQuit.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnQuit.currentFrame = 0;
                btnQuit.y -= 4;
                Environment.Exit(0);
            }
        }
    }

    void hideMenu() {
        //
    }
}
