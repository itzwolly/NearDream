using System;
using System.Drawing;
using GXPEngine;

public class MyGame : Game //MyGame is a Game
{
	private Level _level;
	private STATE _state;

	public enum STATE {
		MAINMENU,
		LEVELSELECTMENU,
		PAUSEMENU,
		LEVEL1,
		LEVEL2,
		LEVEL3,
		WINSCREEN
	}

	//initialize game here
	public MyGame () : base(1024, 768, false, false)
	{
		SetState(STATE.LEVEL2);
		targetFps = 60;
	}

	//update game here
	void Update ()
	{
		if (Input.GetKeyDown(Key.H))
			targetFps = 1;
		if (Input.GetKeyDown(Key.J))
			targetFps = 60;
	}

	public void SetState(STATE pGameState) {
		StopState(_state);
		_state = pGameState;
		StartState(_state);
	}

	void StartState(STATE pGameState) {
		switch (pGameState) {
			case STATE.LEVEL1:
				_level = new Level(this, 1);
				AddChild(_level);
				break;
			case STATE.LEVEL2:
				_level = new Level(this, 2);
				AddChild(_level);
				ShowMouse(false);
				break;
			case STATE.LEVEL3:
				_level = new Level(this, 3);
				AddChild(_level);
				ShowMouse(false);
				break;
			default:
				break;
		}
	}

	public STATE GetState() {
		return _state;
	}

	public void StopState(STATE pGameState) {
		switch (pGameState) {
			case STATE.LEVEL1:
			case STATE.LEVEL2:
			case STATE.LEVEL3:
				if (_level != null) {
					_level.Destroy();
					_level = null;
				}
				break;
			default:
				break;
		}
	}
	
	//system starts here
	static void Main() 
	{
		new MyGame().Start();
	}
}
