using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;
using System.Drawing;
using System.IO;

public class ControlsScreen : Canvas {
    private Sounds _sounds = new Sounds();
    private AnimationButton btnBack;
    private Canvas _canvas = new Canvas(Game.main.width, Game.main.height);
    private Canvas _hoverRegularDragon = new Canvas(64, 64);
    private Canvas _hoverStickyDragon = new Canvas(64, 64);
    private Canvas _stickyPopUp = new Canvas(760, 300);
    private Canvas _regularPopUp = new Canvas(760, 300);
    private bool _shown;
    private MainMenu _mainMenu;
    private System.Drawing.Image _regularDragonImage, _stickyDragonImage;
    
        
    public bool Shown {
        get { return _shown; }
        set { _shown = value; }
    }

    public ControlsScreen(MainMenu pMainMenu) : base(Game.main.width, Game.main.height) {
        _mainMenu = pMainMenu;

        Sprite sprite = new Sprite(MyGame.GetAssetFilePath(MyGame.Asset.UI) +  "\\controlsscreen.png");
        sprite.SetOrigin(width / 2, height / 2);
        sprite.scale = 0.25f;
        sprite.y -= 200;
        AddChild(sprite);
        
        //_sounds.PlayMenuMusic();

        game.AddChild(_canvas);
        _canvas.graphics.Clear(Color.Black);
        _canvas.alpha = 0.6f;

        btnBack = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\close_buttons.png", 2, 1); //change to back button
        AddChild(btnBack);
        btnBack.scale = 0.15f;
        btnBack.SetOrigin(width / 2, height / 2);
        btnBack.x = width / 2 ;
        btnBack.y = btnBack.y - height / 7 - 25;

        _regularDragonImage = new Bitmap(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\normal_dragon_popup.png");
        _stickyDragonImage = new Bitmap(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\enraged_dragon_popup.png");

        _hoverRegularDragon.visible = false;
        _hoverRegularDragon.x = game.width / 2 + game.width / 3 + 3;
        _hoverRegularDragon.y = game.height / 2 - 32;
        game.AddChild(_hoverRegularDragon);

        _hoverStickyDragon.visible = false;
        _hoverStickyDragon.x = game.width / 2 + game.width / 3 + 3;
        _hoverStickyDragon.y = game.height / 2 + 51;
        game.AddChild(_hoverStickyDragon);

        //_regularPopUp.graphics.Clear(Color.White);
        _regularPopUp.visible = false;
        _regularPopUp.graphics.DrawImage(_regularDragonImage, 0, 0);
        _regularPopUp.x -= 230;
        _regularPopUp.y -= 170;
        AddChild(_regularPopUp);

        _stickyPopUp.visible = false;
        _stickyPopUp.graphics.DrawImage(_stickyDragonImage, 0, 0);
        _stickyPopUp.x -= 230;
        _stickyPopUp.y -= 170;
        AddChild(_stickyPopUp);
    }

    private void Update() {
        if (Input.mouseX > _hoverRegularDragon.x && Input.mouseX < _hoverRegularDragon.x + _hoverRegularDragon.width
            && Input.mouseY > _hoverRegularDragon.y && Input.mouseY < _hoverRegularDragon.y + _hoverRegularDragon.height) {
                _regularPopUp.visible = true;
        } else if (Input.mouseX > _hoverStickyDragon.x && Input.mouseX < _hoverStickyDragon.x + _hoverStickyDragon.width
            && Input.mouseY > _hoverStickyDragon.y && Input.mouseY < _hoverStickyDragon.y + _hoverStickyDragon.height) {
                _stickyPopUp.visible = true;
        } else {
            _regularPopUp.visible = false;
            _stickyPopUp.visible = false;
        }

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

