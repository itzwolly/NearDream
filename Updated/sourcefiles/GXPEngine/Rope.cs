using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;


public class Rope : Sprite {
    private string _bridgeToDrop;

    public string BridgeToDrop {
        get { return _bridgeToDrop; }
        set { _bridgeToDrop = value; }
    }

    public Rope() : base(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\rope.png") {
        
    }
}
