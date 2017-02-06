using System;
using System.Drawing;
using GXPEngine;

public class MyGame : Game //MyGame is a Game
{	
	//initialize game here
	public MyGame () : base(800, 600, false)
	{
        //----------------------------------------------------example-code----------------------------
        //create a canvas
        Canvas canvas = new Canvas(800, 600);

        //add some content
        canvas.graphics.FillRectangle(new SolidBrush(Color.Red), new Rectangle(0, 0, 400, 300));
        canvas.graphics.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(400, 0, 400, 300));
        canvas.graphics.FillRectangle(new SolidBrush(Color.Yellow), new Rectangle(0, 300, 400, 300));
        canvas.graphics.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(400, 300, 400, 300));

        //add canvas to display list
        AddChild(canvas);
        //------------------------------------------------end-of-example-code-------------------------
    }

    //update game here
    void Update ()
	{
		//empty
	}
	
	//system starts here
	static void Main() 
	{
		new MyGame().Start();
	}
}
