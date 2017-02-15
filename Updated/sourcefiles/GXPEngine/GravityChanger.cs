using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GXPEngine;

public class GravityChanger : Canvas
{
   public Vec2 changedGravity;

    public GravityChanger(int pX, int pY, int pWidth, int pHeight,string direction):base(pWidth,pHeight)
    {
        SetOrigin(width / 2, height / 2);
        x = pX;
        y = pY;

        graphics.Clear(Color.Yellow);

        if (direction.ToLower() == "down")
        {
            changedGravity = new Vec2(0,0.5f);
        }
        if (direction.ToLower() == "up")
        {
            changedGravity = new Vec2(0, -1.5f);//to counteract the actual gravity
        }
        if (direction.ToLower() == "left")
        {
            changedGravity = new Vec2(-0.5f, 0);
        }
        if (direction.ToLower() == "right")
        {
            changedGravity = new Vec2(0.5f, 0);
        }
        if (direction.ToLower() == "none") {
            changedGravity = new Vec2(0, 0);
        }
    }
}

