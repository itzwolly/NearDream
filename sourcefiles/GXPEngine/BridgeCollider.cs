using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GXPEngine;

public class BridgeCollider:Canvas
{
    public BridgeCollider(float pX, float pY ,int pWidth, int pHeight) : base(pWidth, pHeight)
    {
        SetOrigin(width / 2, height / 2);
        x = pX;
        y = pY;
        graphics.Clear(Color.Aquamarine);
        alpha = 0.5f;
    }

}

