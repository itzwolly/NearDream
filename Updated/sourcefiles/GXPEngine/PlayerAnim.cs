using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

public class PlayerAnim:AnimationSprite
{
    /*1 + 50 frames = idle
    2 to 49 frames = Walking
    50 to 90 frames = Jumping
    91 to 111 frames = Throwing
    */
    private Player _player;
    private int _wait;

    public PlayerAnim(Player pPlayer) : base("assets\\sprites\\playeranim.png", 13, 8)
    {
        _player = pPlayer;
        SetOrigin(width / 2-10, height / 2+35);
    }

    public void Update()
    {
        NextFrame();
    }
}

