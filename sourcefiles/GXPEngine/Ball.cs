using System;
using System.Drawing;

namespace GXPEngine
{
	public class Ball : Canvas
	{
		public Vec2 position;
		public Vec2 velocity;
		public Vec2 nextPosition;
		private bool _OnPlayer;
		public bool IsExploding;
		public bool StartedTimer;
        public bool AddGravity;
		public Vec2 nextPositionBorder;
		public readonly int radius;
		private Color _ballColor;

		//public bool stationary=false;

		public bool OnPlayer {
			get { return _OnPlayer; }
			set { _OnPlayer = value; }
		}

		

		public Ball (int pRadius, Vec2 pPosition = null, Vec2 pVelocity = null, Color? pColor = null):base (pRadius*2, pRadius*2)
		{
			_OnPlayer = true;
			radius = pRadius;
			SetOrigin (radius, radius);
			position = pPosition ?? Vec2.zero;
			velocity = pVelocity ?? Vec2.zero;
			nextPosition = position.Clone().Add(velocity);
			nextPositionBorder = position.Clone().Add(velocity.Clone().Normalize().Scale(velocity.Length() + radius));
			_ballColor = pColor ?? Color.Blue;

			draw ();
			Step ();
		}

		protected void draw() {
            Console.WriteLine("lol");
			graphics.Clear (Color.Empty);
			graphics.FillEllipse (
				new SolidBrush (_ballColor),
				0, 0, 2 * radius, 2 * radius
			);
		}

		public void Step(bool skipVelocity = false) {
			if (position == null || velocity == null)
				return;
			//Console.WriteLine(velocity.Length());
			//if (velocity.x < 0.01f)
			//    velocity.x = 0;
			//if (velocity.y < 0.01f)
			//    velocity.y = 0;
			position.Add(velocity);
			UpdateNextPosition();
		}

		public void UpdateNextPosition()
		{ 
			x = position.x;
			y = position.y;

			UpdateInfo();
		}
		public void UpdateInfo()
		{
			nextPosition = position.Clone().Add(velocity);
			//nextPositionBorder = position.Clone().Add(velocity.Clone().Normalize().Scale(velocity.Length() + radius));
		}

		public Color ballColor {
			get {
				return _ballColor;
			}

			set {
				_ballColor = value;
				draw ();
			}
		}

	}
}

