using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class Player:Sprite
{
    public Vec2 position;
    public Vec2 velocity;
    public Player(float pX,float pY) : base("assets\\sprites\\square.png")
    {
        x = pX;
        y = pY;
        position = new Vec2(pX, pY);
        velocity = Vec2.zero;
       SetOrigin(width/2,height/2);
    }

    public void Step()
    {
        position.Add(velocity);
        x = position.x;
        y = position.y;
    }
}

