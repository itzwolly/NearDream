using System;
using GXPEngine;

public class MainMenu : GameObject {
    AnimationButton btnLevelSelect, btnQuit;
    MyGame _myGame;

    public MainMenu(MyGame myGame) {
        _myGame = myGame;
        btnLevelSelect = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\play_button00.png", 2, 1);
        AddChild(btnLevelSelect);
        btnLevelSelect.x = (game.width - btnLevelSelect.width) / 2;
        btnLevelSelect.y = (game.height - btnLevelSelect.height) / 2;

        btnQuit = new AnimationButton(MyGame.GetAssetFilePath(MyGame.Asset.UI) + "\\quit_button00.png", 2, 1);
        AddChild(btnQuit);
        btnQuit.x = (game.width - btnQuit.width) / 2;
        btnQuit.y = (game.height + (btnQuit.height * 2)) / 2;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (btnLevelSelect.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnLevelSelect.currentFrame = 1;
                btnLevelSelect.y += 4;
            } else if (btnQuit.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnQuit.currentFrame = 1;
                btnQuit.y += 4;
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            if (btnLevelSelect.HitTestPoint(Input.mouseX, Input.mouseY)) {
                btnLevelSelect.currentFrame = 0;
                btnLevelSelect.y -= 4;
                Destroy();
                _myGame.SetState(MyGame.GameState.LEVEL1);
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
