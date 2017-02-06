using System;
using System.Drawing;

namespace GXPEngine
{
	/// <summary>
	/// Simple TextField class with very limited options.
	/// </summary>
	public class TextField : Canvas
	{
		//visible properties
		private string _text = "";							
		private Color _backgroundColor = Color.Empty;		
		private Color _borderColor = Color.Empty;
		private Color _textColor = Color.Black;

		//underlying properties to draw the visible properties
		private Pen _borderPen = null;
		private int _borderSize = 0;
		private Brush _textBrush = null;

		public TextField (int width, int height):base (width, height)
		{
			_textBrush = new SolidBrush (_textColor);
			_borderPen = new Pen (_borderColor, _borderSize);
		}

		public string text {
			get {
				return _text;
			}

			set {
				_text = value;
				_invalidate = true;
			}
		}

		public Font font {
			get {
				return _font;
			}

			set {
				_font = value;
				_invalidate = true;
			}
		}

		public Color backgroundColor {
			get {
				return _backgroundColor;
			}

			set {
				_backgroundColor = value;
				_invalidate = true;
			}
		}

		public int borderSize {
			get {
				return _borderSize;
			}

			set {
				_borderSize = Math.Max (value, 0); 
				_borderPen.Width = _borderSize;
				_invalidate = true;
			}
		}

		public Color borderColor {
			get {
				return _borderColor;
			}

			set {
				_borderColor = value;
				_borderPen.Color = value;
				_invalidate = true;
			}
		}

		protected override void RenderSelf (GXPEngine.Core.GLContext glContext)
		{
			if (_invalidate) {
				graphics.Clear (backgroundColor);
				if (_borderSize > 0 && _borderColor != Color.Empty) {
					graphics.DrawRectangle (_borderPen, 0, 0, width, height);
				}
				graphics.DrawString (_text, _font, _textBrush, 0, 0); 
			}

			base.RenderSelf (glContext);
		}

		// HELPER FUNCTION TO CREATE SIZED TEXTFIELD
		private static Image _stub = new Bitmap(1,1);
		private static Graphics _stubGraphics = Graphics.FromImage(_stub);
		private static Font _font = new Font ("Times New Roman", 16);

		/// <summary>
		/// Creates a text field based on a standard Arial font and font size 16 to contain the given characters.
		/// </summary>
		/// <returns>The text field.</returns>
		/// <param name="text">Text.</param>
		public static TextField CreateTextField (string text) {
			SizeF size = _stubGraphics.MeasureString (text, _font);
			TextField result = new TextField ((int)(size.Width + 5), (int)(size.Height+5));
			result.backgroundColor = Color.White;
			result.text = text;
			return result;
		}
	}
}

