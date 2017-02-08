using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Plank : Sprite
{
    public Plank() : base("assets\\sprites\\plank.png") {
        SetOrigin(width/2, height/2);
        scale = 2;
    }
}