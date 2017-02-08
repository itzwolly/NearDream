using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Plank : Sprite
{
    public Plank() : base("assets\\sprites\\plank.png") {
        SetOrigin(0, 0);
        scale = 2;
    }
}