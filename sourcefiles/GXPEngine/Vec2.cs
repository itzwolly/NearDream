using System;

namespace GXPEngine
{
	public class Vec2 
	{
		public static Vec2 zero { get { return new Vec2(0,0); }}
		public static Vec2 temp = new Vec2 ();

		public float x = 0;
		public float y = 0;

		public Vec2 (float pX = 0, float pY = 0)
		{
			x = pX;
			y = pY;
		}

		public override string ToString ()
		{
			return String.Format ("({0}, {1})", x, y);
		}
			
		public Vec2 Add (Vec2 other) {
			x += other.x;
			y += other.y;
			return this;
		}
		public Vec2 Subtract (Vec2 other) {
			x -= other.x;
			y -= other.y;
			return this;
		}
		public float Length() {
			return (float)Math.Sqrt (x * x + y * y);
		}

		public float DistanceTo(Vec2 other)
		{
			return other.Clone().Subtract(this).Length();
		}

		public Vec2 Normalize () {
			if (x == 0 && y == 0) {
				return this;
			} else {
				return Scale (1/Length ());
			}
		}

		public Vec2 Clone() {
			return new Vec2 (x, y);
		}
	
		public Vec2 Scale (float scalar) {
			x *= scalar;
			y *= scalar;
			return this;
		}

		public Vec2 SetXY(float pX, float pY)
		{
			x = pX;
			y = pY;
			return this;
		}
		public static float Deg2Rad(float degrees)
		{
			return degrees / 180 * Mathf.PI;
		}

		public static float Rad2Deg(float radians)
		{
			return radians / Mathf.PI * 180;
		}

		public static Vec2 GetUnitVectorDegrees(float degrees)
		{
			degrees = Deg2Rad(degrees);
			Vec2 NewVector = new Vec2();
			NewVector.x = Mathf.Cos(degrees);
			NewVector.y = Mathf.Sin(degrees);
			return NewVector;
		}

		public static Vec2 GetUnitVectorRadians(float radians)
		{
			Vec2 NewVector = new Vec2();
			NewVector.x = Mathf.Cos(radians);
			NewVector.y = Mathf.Sin(radians);
			return NewVector;
		}

		public static Vec2 RandomUnitVector()
		{
			Vec2 newVector = new Vec2(1, 0);
			int angle = Utils.Random(0, 361);
			newVector.SetAngleDegrees(angle);
			return newVector;
		}

		public void SetAngleDegrees(float angle)
		{
			angle = Deg2Rad(angle);
			SetAngleRadians(angle);
		}

		public void SetAngleRadians(float radian)
		{
			float howlong = Length();
			x = Mathf.Cos(radian) * howlong;
			y = Mathf.Sin(radian) * howlong;
		}

		public float GetAngleRadians()
		{
			return Mathf.Atan(y / x);
		}

		public float GetAngleDegrees()
		{
			return Rad2Deg(GetAngleRadians());
		}

		public void RotateDegrees(float angle)
		{
			angle = angle + GetAngleDegrees();
			SetAngleDegrees(angle);
		}

		public void RotateRadians(float radian)
		{
			radian = radian + GetAngleRadians();
			SetAngleRadians(radian);
		}

		public void RotateAroundDegrees(float X, float Y, float degrees)
		{
			degrees = Deg2Rad(degrees);
			float sn = Mathf.Sin(degrees);
			float cs = Mathf.Cos(degrees);

			x -= X;
			y -= Y;

			float newx = x * cs - y * sn;
			float newy = x * sn + y * cs;

			x = newx + X;
			y = newy + Y;
		}

		public void RotateAroundRadians(float X, float Y, float radians)
		{
			float sn = Mathf.Sin(radians);
			float cs = Mathf.Cos(radians);

			x -= X;
			y -= Y;

			float newx = x * cs - y * sn;
			float newy = x * sn + y * cs;

			x = newx + X;
			y = newy + Y;
		}

		public void ReflectOnPoint(Vec2 normal, float elasticity)
		{
			//Vec2 _tempVec = other1.Clone().Subtract(other).Normalize();
			this.Subtract(normal.Scale(this.Dot(normal)*2));
            this.Scale(elasticity);
		}

		public Vec2 Normal()
		{
			return new Vec2(-y, x).Normalize();
		}

        public Vec2 NormalNotNormalized()
        {
            return new Vec2(-y, x);
        }

        public void Reflect(Vec2 other,float elasticity)
		{
			this.Subtract(other.Normal().Clone().Scale( 2*this.Dot(other.Normal().Clone())));
			this.Scale(elasticity);
		}

		public float Dot(Vec2 pOther)
		{
			return x * pOther.x + y * pOther.y;
		}


	}
}

