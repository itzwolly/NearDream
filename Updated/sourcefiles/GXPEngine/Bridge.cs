using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Bridge : Sprite
{
    private BridgePlank _bridgePlank;
    private BridgeCollider _bridgeCollider;

    string _bridgeName;
    public bool Down;

    public string BridgeName {
        get { return _bridgeName; }
        set { _bridgeName = value; }
    }

    public BridgeCollider BridgeCollider {
        get { return _bridgeCollider; }
        set { _bridgeCollider = value; }
    }

    public Bridge() : base(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\bridge.png") {
        _bridgePlank = new BridgePlank(this);
        _bridgePlank.x -= width / 2.55f;
        _bridgePlank.y -= height * 2.495f;
        AddChild(_bridgePlank);

        SetOrigin(width / 2, 3 * height / 2);
    }

    public BridgePlank GetBridgePlank() {
        return _bridgePlank;
    }

    public BridgeCollider GetBridgeCollider() {
        return _bridgeCollider;
    }
}
