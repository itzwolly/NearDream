using System;
using System.Drawing;
using GXPEngine;

public class MyGame : Game //MyGame is a Game
{
    Level level;
	//initialize game here
	public MyGame () : base(1900, 1000, false,false)
	{
        targetFps=60;
        level = new Level();
        AddChild(level);
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
