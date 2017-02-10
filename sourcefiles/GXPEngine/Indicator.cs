using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class Indicator:AnimationSprite
{
    public Indicator() : base("indicator.png", 5, 1)
    {
        SetFrame(1);
        SetOrigin(width/2,height);
    }

    public void SetPower(int pFrame)
    {
        SetFrame(pFrame-1);
    }
}

