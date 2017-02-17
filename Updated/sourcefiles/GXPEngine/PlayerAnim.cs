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

    public PlayerAnim(Player pPlayer) : base("assets\\sprites\\player_anim.png", 8, 9)
    {
        _player = pPlayer;
        SetOrigin(width / 2-10, height / 2+35);
    }

    public void Update()
    {
        if(_player.horizontalDirection == Player.Direction.NONE && _player.verticalDirection == Player.Direction.NONE)
        {
            if (_wait > 3)
            {
                NextFrame();
                _wait = 0;
            }
            _wait++;
        }
        else
        NextFrame();

        if (_player.horizontalDirection == Player.Direction.NONE && _player.verticalDirection == Player.Direction.NONE && (currentFrame < 53 || currentFrame > 71))
        {
            SetFrame(53);
        }
        else if (_player.horizontalDirection == Player.Direction.RIGHT || _player.horizontalDirection == Player.Direction.LEFT)
        {
            if (_player.verticalDirection == Player.Direction.NONE && (currentFrame < 2 || currentFrame > 50))
                SetFrame(2);
        }
        if (_player.verticalDirection == Player.Direction.UP)
            SetFrame(52);
        else if (_player.verticalDirection == Player.Direction.DOWN)
            SetFrame(51);
    }
}