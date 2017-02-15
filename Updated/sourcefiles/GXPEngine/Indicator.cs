using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Indicator : AnimationSprite
{
    private Vec2 _indicatorVec = new Vec2(0,0);

    public Vec2 IndicatorVec {
        get { return _indicatorVec; }
        set { _indicatorVec = value; }
    }
    public Indicator() : base(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\indicator.png", 5, 1)
    {
        SetFrame(1);
        SetOrigin(width/2,height);
    }

    public void SetPower(int pFrame)
    {
        SetFrame(pFrame-1);
    }
}

