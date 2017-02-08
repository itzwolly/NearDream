using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using GXPEngine;

    class Stone:Ball
    {
    
    private Color _ballColor;
    public bool active;
    public bool hitPlayer;

    public Stone(int pRadius, Vec2 pPosition = null, Vec2 pVelocity = null, Color? pColor = null,bool? pActive = false):base (pRadius,null,null,null)
		{
        active = pActive ?? false;
        OnPlayer = true;
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
        graphics.Clear(Color.Empty);
        graphics.FillEllipse(
            new SolidBrush(_ballColor),
            0, 0, 2 * radius, 2 * radius
        );
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

