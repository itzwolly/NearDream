using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using System.Drawing;
using System.IO;

public class ControlsScreen : Sprite {
    private Sounds _sounds = new Sounds();
    private AnimationButton btnBack;
    private Canvas _canvas = new Canvas(Game.main.width, Game.main.height);
    private bool _shown;
    private MainMenu _mainMenu;

    public bool Shown {
        get { return _shown; }
        set { _shown = value; }
    }

    public ControlsScreen(MainMenu pMainMenu) : base(MyGame.GetAssetFilePath(MyGame.Asset.UI) +  "\\controlsscreen.png") {
        _mainMenu = pMainMenu;
        //_sounds.PlayMenuMusic();
        SetOrigin(width / 2, height / 2);
        scale = 0.25f;

        game.AddChild(_canvas);
        _canvas.graphics.Clear(Color.Black);
        _canvas.alpha = 0.6f;

        btnBack = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\close_buttons.png", 2, 1); //change to back button
        AddChild(btnBack);
        btnBack.scale = 0.65f;
        btnBack.SetOrigin(width / 2, height / 2);
        btnBack.x = width * 2 - width / 5;
        btnBack.y = (-height + height / 8) - 13;

        //Canvas canvas = new Canvas()
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (btnBack.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnBack.currentFrame = 1;
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            if (btnBack.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnBack.currentFrame = 0;
                //_sounds.StopMenuMusic();
                Destroy();
                _canvas.Destroy();
                _mainMenu.ControlsShown = false;
            } else {
                btnBack.currentFrame = 0;
            }
        }
    }
}

