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
        if (_player.horizontalDirection == Player.Direction.NONE && _player.verticalDirection == Player.Direction.NONE)
            SetFrame(0);
        else if (_player.horizontalDirection == Player.Direction.RIGHT)
        {
            if(_player.verticalDirection==Player.Direction.NONE && (currentFrame < 2 || currentFrame > 49))
            SetFrame(1);
        }
        if (_player.verticalDirection == Player.Direction.UP)
            SetFrame(50);
        else if (_player.verticalDirection == Player.Direction.DOWN)
            SetFrame(69);
    }
}

