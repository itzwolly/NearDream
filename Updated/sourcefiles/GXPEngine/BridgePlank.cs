using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class BridgePlank : AnimationSprite
{
    private bool _startAnim;
    private Bridge _bridge;

    public bool StartAnimation {
        get { return _startAnim; }
        set { _startAnim = value; }
    }

    public BridgePlank(Bridge pBridge) : base(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\falling_bridge_sprite.png", 7, 6) {
        _bridge = pBridge;
    }

    private void Update() {
        if (_startAnim == true) {
            NextFrame();
            if (currentFrame == 40) {
                _bridge.GetBridgeCollider().y += 190;
                _bridge.GetBridgeCollider().height = 20;
                _startAnim = false;
            }
        }
    }
}
