using System;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace GXPEngine
{
	/// <summary>
	/// Implements a line with normal representation
	/// </summary>
	public class NLineSegment : LineSegment
	{
		private Arrow _normal;
	
		public NLineSegment (float pStartX, float pStartY, float pEndX, float pEndY, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false)
			: this (new Vec2 (pStartX, pStartY), new Vec2 (pEndX, pEndY), pColor, pLineWidth)
		{
		}

		public NLineSegment (Vec2 pStart, Vec2 pEnd, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false)
			: base (pStart, pEnd, pColor, pLineWidth, pGlobalCoords)
		{
			lineOnOriginNormalized = end.Clone().Subtract(start).Normalize();
			lineLenght = Mathf.Sqrt((start.x - end.x) * (start.x - end.x) + (start.y - end.y) * (start.y - end.y));
			_normal = new Arrow (null, null, 40, 0xffff0000, 1);
			AddChild (_normal);
		}

		//------------------------------------------------------------------------------------------------------------------------
		//														RenderSelf()
		//------------------------------------------------------------------------------------------------------------------------
		override protected void RenderSelf(GLContext glContext) {
			if (game != null && start != null && end != null) {
				recalc ();
				RenderLine (start, end, color, lineWidth);

			}
		}

		private void recalc() {
			_normal.startPoint = start.Clone ().Add (end).Scale (0.5f);
			_normal.vector = end.Clone ().Subtract (start).Normal ();
		}

	}
}

