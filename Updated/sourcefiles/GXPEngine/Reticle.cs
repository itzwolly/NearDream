using System;
using System.Collections.Generic;
using GXPEngine;

public class Reticle : Sprite {
    // Constructor
    public Reticle() : base(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\crosshair.png") {
        SetOrigin(width / 2, height / 2);

        width = 48;
        height = 48;
    }
}
