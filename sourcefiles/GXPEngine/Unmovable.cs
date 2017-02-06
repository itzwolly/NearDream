using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class Unmovable:Sprite
{
    public Unmovable(float pX, float pY) : base("assets\\sprites\\colors.png")
    {
    SetOrigin(width / 2, height / 2);
        x = pX;
        y = pY;
    }
}

