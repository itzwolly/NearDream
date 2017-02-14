using System;
using GXPEngine;

public class Player : Sprite
{
    private const float MAX_SPEED = 19;
    public const float SPEED = 8;
    public const int MAXPOWER = 30;

    private Vec2 _position;
    private Vec2 _velocity;
    private Reticle _reticle;
    private Indicator _indicator;

    private int _amountOfTrophies = 0;
    private int _score;
    private bool _isMoving = false;
    private bool _jumped;

    public Vec2 Position {
        get { return _position; }
        set { _position = value; }
    }

    public Vec2 Velocity {
        get { return _velocity; }
        set { _velocity = value; }
    }

    public bool IsMoving {
        get { return _isMoving; }
        set { _isMoving = value; }
    }

    public int AmountOfTrophies {
        get { return _amountOfTrophies; }
        set { _amountOfTrophies = value; }
    }

    public int Score {
        get { return _score; }
        set { _score = value; }
    }

    public bool Jumped {
        get { return _jumped; }
        set { _jumped = value; }
    }

    public enum Direction {
        NONE, LEFT,
        RIGHT
    }

    public Player(float pX,float pY) : base(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\square.png")
    {
        x = pX;
        y = pY;

        _position = new Vec2(pX, pY);
        _velocity = Vec2.zero;

        SetOrigin(width/2,height/2);
        PlayerAnim animation = new PlayerAnim(this);
        AddChild(animation);
        _reticle = new Reticle();
    }

    public void Step()
    {
        if (_velocity.y > MAX_SPEED) {
            _velocity.y = MAX_SPEED;
        }

        _position.Add(_velocity);

        x = _position.x;
        y = _position.y;
    }

    public Reticle GetReticle() {
        return _reticle;
    }

    public Indicator GetIndicator() {
        return _indicator;
    }

    public void SetIndicator(Indicator pIndicator) {
        _indicator = pIndicator;
    }
}

