using System;

namespace GXPEngine
{
	/**
	 * Simple extension of MouseHandler, allows you to drag an object.
	 */
	public class DragHandler:MouseHandler
	{
		public DragHandler (GameObject target):base (target)
		{
			OnMouseDownOnTarget+= onTargetMouseDownHandler;
		}

		private void onTargetMouseDownHandler (GameObject target, MouseEventType type) {
			OnMouseMove += onMouseMoveHandler;
			OnMouseUp += onMouseUpHandler;
		}

		private void onMouseMoveHandler (GameObject target, MouseEventType type) {
			target.x = Input.mouseX + offsetToTarget.x;
			target.y = Input.mouseY + offsetToTarget.y;
		}

		private void onMouseUpHandler (GameObject target , MouseEventType type) {
			OnMouseUp -= onMouseUpHandler;
			OnMouseMove -= onMouseMoveHandler;
		}

	}
}

