using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GXPEngine;

public class GravityChanger : Canvas
{
   public Vec2 changedGravity;

    public GravityChanger(float pX, float pY, int pWidth, int pHeight, int pDirection):base(pWidth,pHeight)
    {
        SetOrigin(width / 2, height / 2);
        x = pX;
        y = pY;
        
        graphics.Clear(Color.Yellow);

        if (pDirection == 3)
        {
            changedGravity = new Vec2(0,0.5f);
        }
        if (pDirection == 1)
        {
            changedGravity = new Vec2(0, -1.5f);//to counteract the actual gravity
        }
        if (pDirection == 4)
        {
            changedGravity = new Vec2(-0.5f, 0);
        }
        if (pDirection == 2)
        {
            changedGravity = new Vec2(0.5f, 0);
        }
        if (pDirection == 0) {
            changedGravity = new Vec2(0, 0);
        }
        alpha = 0.5f;
    }
}

