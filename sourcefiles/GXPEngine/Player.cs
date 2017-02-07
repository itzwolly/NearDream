using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class Player:Sprite
{
    public Vec2 position;
    public Vec2 velocity;
    private int _amountOfTrophies = 0;

    public int AmountOfTrophies
    {
        get { return _amountOfTrophies; }
        set { _amountOfTrophies = value; }
    }

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
        if (velocity.y > 19)
            velocity.y = 19;
        position.Add(velocity);
        x = position.x;
        y = position.y;
    }
}

