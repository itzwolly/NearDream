using System;
using System.Drawing;

namespace GXPEngine
{
	public class Ball : Canvas
	{
		public const int SPEED = 8;
		public const int REPETITIONS = 2;
		public const float ELASTICITY = 0.7f;
		public const float EPSILON = 0.8f;
		public const float COLLISION_FRICTION = 0.8f;

		/* Boom ball */
		public const float BLASTSIZE = 500;
		public const int WAITFORBOOM = 180;
		/* */

		public readonly int radius;
		private Vec2 _position;
		private Vec2 _velocity;
		private Vec2 _nextPosition;
		private Vec2 _nextPositionBorder;

		private bool _isExploding;
		private bool _startedTimer;
		public bool _addGravity;
        public bool charge;

		private bool _OnPlayer;
		private Color _ballColor;
		private float _startingBallVelocity;
        protected BallAnim _animation;


        public Vec2 Velocity {
			get { return _velocity; }
			set { _velocity = value; }
		}
		public Vec2 Position {
			get { return _position; }
			set { _position = value; }
		}
		public Vec2 NextPosition {
			get { return _nextPosition; }
			set { _nextPosition = value; }
		}
		public Vec2 NextPositionBorder {
			get { return _nextPositionBorder; }
			set { _nextPositionBorder = value; }
		}
		public bool IsExploding {
			get { return _isExploding; }
			set { _isExploding = value; }
		}
		public bool StartedTimer {
			get { return _startedTimer; }
			set { _startedTimer = value; }
		}
		public bool AddGravity {
			get { return _addGravity; }
			set { _addGravity = value; }
		}
		public bool OnPlayer {
			get { return _OnPlayer; }
			set { _OnPlayer = value; }
		}
		public float StartingBallVelocity {
			get { return _startingBallVelocity; }
			set { _startingBallVelocity = value; }
		}

        public Ball(int pRadius, Vec2 pPosition = null, Vec2 pVelocity = null, Color? pColor = null) : base(pRadius * 2, pRadius * 2)
        {
            _OnPlayer = true;
            radius = pRadius;
            SetOrigin(radius, radius);
            _position = pPosition ?? Vec2.zero;
            _velocity = pVelocity ?? Vec2.zero;
            _nextPosition = _position.Clone().Add(_velocity);
            _nextPositionBorder = _position.Clone().Add(_velocity.Clone().Normalize().Scale(_velocity.Length() + radius));
            _ballColor = pColor ?? Color.Blue;
            _startingBallVelocity = SPEED / 2;
            _animation = new BallAnim(this);
            AddChild(_animation);
            
			draw ();
			Step ();
		}

		protected void draw() {
			graphics.Clear (Color.Empty);
			graphics.FillEllipse (
				new SolidBrush (_ballColor),
				0, 0, 2 * radius, 2 * radius
			);
		}

		public void Step(bool skipVelocity = false) {
			if (_position == null || _velocity == null)
				return;
			//Console.WriteLine(velocity.Length());
			//if (velocity.x < 0.01f)
			//    velocity.x = 0;
			//if (velocity.y < 0.01f)
			//    velocity.y = 0;
			_position.Add(_velocity);
			UpdateNextPosition();
		}

		public void UpdateNextPosition()
		{ 
			x = _position.x;
			y = _position.y;

			UpdateInfo();
		}
		public void UpdateInfo()
		{
			_nextPosition = _position.Clone().Add(_velocity);
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

