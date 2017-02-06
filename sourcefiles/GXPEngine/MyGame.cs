using System;
using System.Drawing;
using GXPEngine;

public class MyGame : Game //MyGame is a Game
{	
	//initialize game here
	public MyGame () : base(800, 600, false)
	{
		
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
