using System;
using System.Drawing;
using GXPEngine;

public class MyGame : Game //MyGame is a Game
{
    Level level;
	//initialize game here
	public MyGame () : base(800, 600, false,false)
	{
        targetFps=60;
        level = new Level();
        AddChild(level);
    }

    //update game here
    void Update ()
	{
        if (Input.GetKeyDown(Key.H))
            targetFps = 1;
        if (Input.GetKeyDown(Key.J))
            targetFps = 60;
    }
	
	//system starts here
	static void Main() 
	{
		new MyGame().Start();
	}
}
