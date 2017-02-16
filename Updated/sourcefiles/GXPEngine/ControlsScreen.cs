using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using System.Drawing;
using System.IO;

class ControlsScreen : Sprite
{
    MyGame _game;
    private AnimationButton btnBack;
    public ControlsScreen(MyGame pGame) : base("assets//UI//controlsscreen.png")
    {
        _game = pGame;
        SetOrigin(width / 2, height / 2);
        this.scale = 0.4f;
        btnBack = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI)+"//quit_buttons.png",2,1);//change to back button
        AddChild(btnBack);
        btnBack.scale = 0.5f;
        btnBack.x = width-200;
        btnBack.y = height-100;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (btnBack.HitTestPoint(Input.mouseX, Input.mouseY))
            {
                btnBack.currentFrame = 1;
                btnBack.y += 7;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (btnBack.HitTestPoint(Input.mouseX, Input.mouseY))
            {
                btnBack.currentFrame = 0;
                btnBack.y -= 7;
                Destroy();
                _game.SetState(MyGame.GameState.MAINMENU);
            }

        }
    }
}

