using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using GXPEngine;

class ExplodingBall : Ball
{

    private Color _ballColor;
    public bool active;
    public bool hitPlayer;
    public bool started;
    public bool aboutToHit;

    public ExplodingBall (int pRadius, Vec2 pPosition = null, Vec2 pVelocity = null, Color? pColor = null, bool? pActive = false) : base(pRadius, null, null, null)
    {
        aboutToHit = false;
        active = pActive ?? false;
        OnPlayer = true;
        started = false;
        SetOrigin(radius, radius);
        position = pPosition ?? Vec2.zero;
        velocity = pVelocity ?? Vec2.zero;
        nextPosition = position.Clone().Add(velocity);
        nextPositionBorder = position.Clone().Add(velocity.Clone().Normalize().Scale(velocity.Length() + radius));
        _ballColor = pColor ?? Color.Blue;

        draw();
        Step();
    }

    private void draw()
    {
        base.draw();
    }

    public void Step(bool skipVelocity = false)
    {
        if (position == null || velocity == null)
            return;
        //Console.WriteLine(velocity.Length());
        position.Add(velocity);
        UpdateNextPosition();
    }

    public void UpdateNextPosition()
    {
        x = position.x;
        y = position.y;

        UptadeInfo();
    }
    public void UptadeInfo()
    {
        nextPosition = position.Clone().Add(velocity);
        nextPositionBorder = position.Clone().Add(velocity.Clone().Normalize().Scale(velocity.Length() + radius));
    }

    public Color ballColor
    {
        get
        {
            return _ballColor;
        }

        set
        {
            _ballColor = value;
            draw();
        }
    }
}


