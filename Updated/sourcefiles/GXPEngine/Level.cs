using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using GXPEngine;

public class Level : GameObject {
	private Map _map;
	private Layer _layer, _foreGround, _foreGroundPartTwo ,_midGround, _backGround;
	private Player _player;
	private Ball _ball;
	private LineSegment _line, _ballToLine;
	private PhysicsEngine _engine;
	private Player.Direction _playerDirection;
	private HUD _hud;
	
	private Dictionary<string, Layer> _layers = new Dictionary<string, Layer>();
	private List<LineSegment> _lines = new List<LineSegment>();
	private List<Bridge> _bridges = new List<Bridge>();
	private List<BridgeCollider> _bridgeColliders = new List<BridgeCollider>();
	private List<Rope> _ropes = new List<Rope>();
	private List<Trophy> _trophies = new List<Trophy>();
	private List<Pot> _pots = new List<Pot>();
	private List<Plank> _planks = new List<Plank>();
	private List<GameTile> _collidables = new List<GameTile>();
	private List<Plank> _destroyables = new List<Plank>();
	private List<Stone> _stones = new List<Stone>();
	private List<Tree> _trees = new List<Tree>();
	private List<GravityChanger> _gravityChangers = new List<GravityChanger>();
	private List<PressurePlate> _pressurePlates = new List<PressurePlate>();
	private List<GameObject> _pressurePlateObjects = new List<GameObject>();

	private TMXParser _tmxParser = new TMXParser();
	

	private int _currentLevel;
	private float _xOffset, _yOffset;
	private int[] _trophyArray = { 0, 0, 0};
	private bool _hasLoaded = false;


	public int CurrentLevel {
		get { return _currentLevel; }
		set { _currentLevel = value; }
	}

	public bool HasLoaded {
		get { return _hasLoaded; }
		set { _hasLoaded = value; }
	}

	public Level(int pCurrentLevel) {
		_currentLevel = pCurrentLevel;
		_map = _tmxParser.ParseFile(MyGame.GetAssetFilePath(MyGame.Asset.ROOT) + "\\level_" + _currentLevel + ".tmx");

		BuildLevel();
		CreatePlayer();
		CreateBall();
		//CreatePressurePlates();
		CreateTiledObjects();
		_engine = new PhysicsEngine(this);
		RenderLines();

		// Assign layers to variables for ease of access.
		_foreGround = GetLayerByName("Foreground");
		_foreGroundPartTwo = GetLayerByName("Foreground_2");
		_midGround = GetLayerByName("Midground"); // note midGround is the base layer.
		_backGround = GetLayerByName("Background");

		foreach (GameTile tile in _midGround.GetTiles()) {
			_collidables.Add(tile);
		}
	}

	private void Update() {
		_xOffset = game.x - this.x;
		_yOffset = game.y - this.y;

		_player.GetReticle().x = Input.mouseX + _xOffset;
		_player.GetReticle().y = Input.mouseY + _yOffset;

		PlayerCamera();
		_engine.HandlePlayer();
		_engine.HandleBall();
		_engine.CheckStones();
		_engine.HandleStickyBall();
		_engine.CheckPotCollision();
		_engine.CheckTrophyCollision();
		_engine.CheckRopeCollision();
		_engine.HandleDestructablePlanks();

		if (_playerDirection == Player.Direction.LEFT) {
		   // _player.Mirror(true, false);
			_player.scaleX = -1.0f;
		} else if (_playerDirection == Player.Direction.RIGHT) {
			//_player.Mirror(true, false);
			_player.scaleX = 1.0f;
		}


		Console.WriteLine(_playerDirection);
	}

	private void RenderLines() {
		foreach (NLineSegment line in _lines) {
			AddChild(line);
		}
	}

	private void PlayerCamera() {
		//y = game.height / 1.25f - _player.y;

		if (_player.IsMoving) {
			x = game.width / 2 - _player.x;
		}

		if (x > 0) {
			x = 0;
		}

		//if (y > 0) {
		//    y = 0;
		//}

		//if (y < -(game.height)) {
		//    y = -(game.height);
		//}

		//Console.WriteLine(x + " | " + -(_map.GetLevelWidth() - game.width)); // should be 2240 - 60 * 30 - game.width, (game.width being 1600)

		if (x < -(_map.GetLevelWidth() - game.width)) {
			x = -(_map.GetLevelWidth() - game.width);
		}

		if (x < 0 && x > -(_map.GetLevelWidth() - game.width)) {
			if (_player.IsMoving) {
				if (_playerDirection == Player.Direction.RIGHT) {
					_foreGround.MoveLayer(Layer.Direction.LEFT, 4.5f);
					_foreGroundPartTwo.MoveLayer(Layer.Direction.LEFT, 4.5f);
					MoveTrees(6.5f);
				} else if (_playerDirection == Player.Direction.LEFT) {
					_foreGround.MoveLayer(Layer.Direction.RIGHT, 4.5f);
					_foreGroundPartTwo.MoveLayer(Layer.Direction.RIGHT, 4.5f);
					MoveTrees(-6.5f);
				}
			}
		}
	}

	private void MoveTrees(float pAmount) {
		foreach (Tree tree in _trees) {
			tree.MoveTree(pAmount);
		}
	}

	private void BuildLevel() {
		foreach (TiledLayer layer in _map.Layer) {
			_layer = new Layer(this, layer);
			AddChild(_layer);
			_layers.Add(layer.Name, _layer);

			if (layer.Name == "Foreground") {
				_layer = new Layer(this, layer);
				_layer.x = _map.GetLevelWidth();
				AddChild(_layer);
				_layers.Add(layer.Name + "_2", _layer);
			}
		}
	}

	private void CreatePlayer() {
		_player = new Player(200, game.height / 2);
		AddChildAt(_player, 0);
		AddChildAt(_player.GetReticle(), 31);
	}

	public void CreateIndicator() {
		//_sounds.PlayCharge();
		_player.SetIndicator(new Indicator());
		AddChild(_player.GetIndicator());
	}

	private void CreateBall() {
		_ball = new Ball(25, new Vec2(game.width / 2, game.height / 2), null, Color.Coral);
		AddChildAt(_ball, 1);
		_ball.Velocity = new Vec2();
		_ballToLine = new LineSegment(null, null);
		AddChild(_ballToLine);
	}

	private void CreateTiledObjects() {
		foreach (ObjectGroup objGroup in _map.ObjectGroup) {
			// so that we don't dont have to give all the object groups an property
			if (objGroup.Name == "Bridge") {
				foreach (TiledObject obj in objGroup.Object) {
					Bridge bridge = new Bridge();
					bridge.x = obj.X + bridge.width / 2;
					bridge.y = obj.Y + bridge.height;
					bridge.BridgeName = obj.Properties.GetPropertyByName("bridge_name").Value;
					bridge.SpriteName = obj.Name;
					_bridges.Add(bridge);
					AddChildAt(bridge, 7);
					_pressurePlateObjects.Add(bridge);
					bridge.BridgeCollider = new BridgeCollider(obj.X + bridge.width / 2 , obj.Y - 180, 450, 400);
					_bridgeColliders.Add(bridge.BridgeCollider);
				}
			}
			if (objGroup.Name == "GravityChanger") {
				foreach (TiledObject obj in objGroup.Object) {
					//Console.WriteLine(Convert.ToInt32(obj.Properties.GetPropertyByName("Direction").Value));
					GravityChanger gravityChanger = new GravityChanger(obj.X, obj.Y, obj.Width, obj.Height, Convert.ToInt32(obj.Properties.GetPropertyByName("Direction").Value));
					AddChild(gravityChanger);
					_gravityChangers.Add(gravityChanger);
				}
			}
			if (objGroup.Name == "Rope") {
				foreach (TiledObject obj in objGroup.Object) {
					Rope rope = new Rope();
					rope.x = obj.X;
					rope.y = obj.Y;
					rope.rotation = 340;
					rope.BridgeToDrop = obj.Properties.GetPropertyByName("bridge_to_drop").Value;
					rope.SpriteName = obj.Name;
					_ropes.Add(rope);
					_pressurePlateObjects.Add(rope);
					AddChildAt(rope, 4);
				}
			}
			if (objGroup.Name == "Pots") {
				foreach (TiledObject obj in objGroup.Object) {
					Pot pot = new Pot();
					pot.x = obj.X + obj.Width / 2;
					pot.y = obj.Y + obj.Height / 2;
					pot.SpriteName = obj.Name;
					_pots.Add(pot);
					AddChildAt(pot, 0);
					_pressurePlateObjects.Add(pot);
					pot.Canvas.x = pot.x - pot.width / 2;
					pot.Canvas.y = pot.y - pot.height * 0.8f;
					AddChildAt(pot.Canvas, 50);
				}
			}
			if (objGroup.Name == "Planks") {
				foreach (TiledObject obj in objGroup.Object) {
					Plank plank = new Plank();
					plank.x = obj.X + obj.Width / 2;
					plank.y = obj.Y + obj.Height / 2;
					plank.Position.x = plank.x;
					plank.Position.y = plank.y;
					plank.SpriteName = obj.Name;
					_pressurePlateObjects.Add(plank);
					_planks.Add(plank);
					_destroyables.Add(plank);
					AddChildAt(plank, 2);
					_lines.Add(plank.GetLine());
				}
			}
			if (objGroup.Name == "Stones") {
				foreach (TiledObject obj in objGroup.Object) {
					//25, new Vec2(_ball.x, _ball.y), null, Color.Blue, false
					Stone stone = new Stone(25, new Vec2(obj.X + obj.Width / 2, obj.Y + obj.Height / 2), null, Color.Blue, false);
					AddChild(stone);
					_stones.Add(stone);
					stone.Velocity = Vec2.zero;
				}
			}
			if (objGroup.Name == "Pressureplates") {
				//Console.WriteLine("test");
				foreach (TiledObject obj in objGroup.Object) {
					PressurePlate _pressurePlate = new PressurePlate(this, obj.X + obj.Width / 2, obj.Y + obj.Height, obj.Properties.GetPropertyByName("ItemToInteract").Value, Convert.ToBoolean(obj.Properties.GetPropertyByName("HasCover").Value), 64, 128);
					AddChild(_pressurePlate);
					_pressurePlates.Add(_pressurePlate);
				}
			}
			if (objGroup.Name == "Trophies") {
				foreach (TiledObject obj in objGroup.Object) {
					Trophy trophy = new Trophy(Convert.ToInt32(obj.Properties.GetPropertyByName("number").Value), MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\trophy_sprite.png", 8, 8);
					trophy.x = obj.X + obj.Width / 4;
					trophy.y = obj.Y + obj.Height / 4;
					AddChildAt(trophy, 0);
					_trophies.Add(trophy);
					trophy.SpriteName = obj.Name;
					_pressurePlateObjects.Add(trophy);
				}
			}
			if (objGroup.Name == "Points") {
				foreach (TiledObject obj in objGroup.Object) {
					foreach (Vec2 points in obj.Polyline.GetPointsAsVectorList()) {
						_line = new NLineSegment(new Vec2(obj.X, obj.Y), new Vec2(obj.X + points.x, obj.Y + points.y), 0xffffff00, 4);
						_lines.Add(_line);
					}
				}
			}
			if (objGroup.Name == "ForegroundTree") {
				foreach (TiledObject obj in objGroup.Object) {
					CreateRandomTree(obj);
				}
			}
		}
	}

	private void CreateRandomTree(TiledObject pObj) {
		Random rnd = new Random();
		int number = rnd.Next(1, 13);
		if (number == 2 || number == 7 || number == 11) {
			Tree tree = new Tree(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\tree_1.png"); // tree_1
			tree.x = pObj.X - pObj.Width;
			tree.y = pObj.Y + pObj.Height;
			_trees.Add(tree);
			AddChildAt(tree, 50);
		} else {
			Tree tree = new Tree(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\tree_2.png"); // tree_2
			tree.x = pObj.X - pObj.Width;
			tree.y = pObj.Y + pObj.Height;
			_trees.Add(tree);
			AddChildAt(tree, 50);
		}
	}

	// This is case-sensitive
	private Layer GetLayerByName(string pName) {
		if (_layers.ContainsKey(pName)) {
			return _layers[pName];
		}
		return null;
	}

	public Dictionary<string, Layer> GetLayers() {
		return _layers;
	}

	public List<GameTile> GetCollidables() {
		return _collidables;
	}

	public List<Plank> GetDestroyables() {
		return _destroyables;
	}

	public List<Stone> GetStones() {
		return _stones;
	}

	public List<Bridge> GetBridges() {
		return _bridges;
	}

	public List<Trophy> GetTrophies() {
		return _trophies;
	}

	public List<Rope> GetRopes() {
		return _ropes;
	}

	public List<Plank> GetPlanks() {
		return _planks;
	}

	public List<GravityChanger> GetGravityChangers() {
		return _gravityChangers;
	}

	public List<PressurePlate> GetPressurePlates() {
		return _pressurePlates;
	}

	public List<BridgeCollider> GetBridgeColliders() {
		return _bridgeColliders;
	}

	public List<Pot> GetPots() {
		return _pots;
	}

	public void SetPlayerDirection(Player.Direction pDirection) {
		_playerDirection = pDirection;
	}

	public float GetXOffset() {
		return _xOffset;
	}

	public float GetYOffSet() {
		return _yOffset;
	}

	public Player GetPlayer() {
		return _player;
	}

	public List<LineSegment> GetLines() {
		return _lines;
	}

	public Ball GetBall() {
		return _ball;
	}

	public Map GetMap() {
		return _map;
	}

	public int[] GetTrophyArray() {
		return _trophyArray;
	}

	public void CreateHUD() {
		_hud = new HUD(this);
		game.AddChild(_hud);
	}

	public HUD GetHUD() {
		return _hud;
	}

	public List<GameObject> GetPressurePlateObjects() {
		return _pressurePlateObjects;
	}
}
