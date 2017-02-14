using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using GXPEngine;

	public class Stone : Ball
	{
	
	private Color _ballColor;
	public bool active;
	public bool hitPlayer;
	public bool started;
	public bool aboutToHit;

	public Stone(int pRadius, Vec2 pPosition = null, Vec2 pVelocity = null, Color? pColor = null,bool? pActive = false):base (pRadius,null,null,null)
		{
		aboutToHit = false;
		active = pActive ?? false;
		OnPlayer = true;
		started = false;
		SetOrigin(radius, radius);
		Position = pPosition ?? Vec2.zero;
		Velocity = pVelocity ?? Vec2.zero;
		NextPosition = Position.Clone().Add(Velocity);
		NextPositionBorder = Position.Clone().Add(Velocity.Clone().Normalize().Scale(Velocity.Length() + radius));
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
		if (Position == null || Velocity == null)
			return;
		//Console.WriteLine(velocity.Length());
		Position.Add(Velocity);
		UpdateNextPosition();
	}

	public void UpdateNextPosition()
	{
		x = Position.x;
		y = Position.y;

		UptadeInfo();
	}
	public void UptadeInfo()
	{
		NextPosition = Position.Clone().Add(Velocity);
		NextPositionBorder = Position.Clone().Add(Velocity.Clone().Normalize().Scale(Velocity.Length() + radius));
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

