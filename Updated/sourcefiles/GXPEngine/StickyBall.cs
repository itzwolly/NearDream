using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class StickyBall : AnimationSprite {
    int _timer;
    bool _doAnim;

    public StickyBall() : base(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) +  "\\sticky_dragon.png", 8, 4) {
        SetOrigin(width / 2, height / 2);
    }

    private void Update() {
        _timer++;
        if (_timer == 3) {
            if (currentFrame < frameCount - 3) {
                NextFrame();
            } else {
                currentFrame = 0;
            }
            _timer = 0;
        }
    }
}
