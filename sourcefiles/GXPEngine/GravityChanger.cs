using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GXPEngine;

class GravityChanger:Canvas
{
   public Vec2 changedGravity;

    public GravityChanger(int pX, int pY, int pWidth, int pHeight,string direction):base(pWidth,pHeight)
    {
        SetOrigin(width / 2, height / 2);
        x = pX;
        y = pY;
        graphics.Clear(Color.Gray);
        if (direction.ToLower() == "down")
        {
            changedGravity = new Vec2(0,1);
        }
        if (direction.ToLower() == "up")
        {
            changedGravity = new Vec2(0, -2);//to counteract the actual gravity
        }
        if (direction.ToLower() == "left")
        {
            changedGravity = new Vec2(-1, 0);
        }
        if (direction.ToLower() == "right")
        {
            changedGravity = new Vec2(1, 0);
        }
    }

    
}

