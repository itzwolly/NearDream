using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using GXPEngine;

public class Level : GameObject {
	private Map _map;
	private MyGame _myGame;
	#region Layers
	private Layer _layer, _foreGround, _foreGroundPartTwo, _foreGroundPartThree ,_midGround, _backGround, _cloudLayer, _cloudLayerPartTwo, _skyLines, _skyLinesPartTwo, _groundTiles, _groundTilesPartTwo, _backGroundFar, _backGroundFarPartTwo, _stonesBackground, _stonesBackgroundPartTwo;
	#endregion
	private Player _player;
	private Ball _ball;
	private LineSegment _line, _ballToLine;
	private PhysicsEngine _engine;
	private Player.Direction _playerDirection;
	private HUD _hud;
	private Sounds _sounds;
	private Dictionary<string, Layer> _layers = new Dictionary<string, Layer>();
	private List<LineSegment> _lines = new List<LineSegment>();
	private List<Bridge> _bridges = new List<Bridge>();
	private List<BridgeCollider> _bridgeColliders = new List<BridgeCollider>();
	private List<Rope> _ropes = new List<Rope>();
	private List<Item> _trophies = new List<Item>();
	private List<Pot> _pots = new List<Pot>();
	private List<Plank> _planks = new List<Plank>();
	private List<GameTile> _collidables = new List<GameTile>();
	private List<Plank> _destroyables = new List<Plank>();
	private List<Stone> _stones = new List<Stone>();
	private List<Tree> _trees = new List<Tree>();
	private List<GravityChanger> _gravityChangers = new List<GravityChanger>();
	private List<PressurePlate> _pressurePlates = new List<PressurePlate>();
	private List<GameObject> _pressurePlateObjects = new List<GameObject>();
	private List<StickyBall> _stickyBalls = new List<StickyBall>();
	private TMXParser _tmxParser = new TMXParser();
	private PauseMenu _pauseMenu;

	private bool _isPaused;
	public int wait;
	private int _currentLevel;
	private float _xOffset, _yOffset;
	private int[] _trophyArray = { 0, 0, 0};
	private bool _hasLoaded = false;
	private bool _finishedLevel;
	
	public bool FinishedLevel {
		get { return _finishedLevel; }
		set { _finishedLevel = value; }
	}
	public int CurrentLevel {
		get { return _currentLevel; }
		set { _currentLevel = value; }
	}
	public bool HasLoaded {
		get { return _hasLoaded; }
		set { _hasLoaded = value; }
	}
	public bool IsPaused {
		get { return _isPaused; }
		set { _isPaused = value; }
	}

	public Level(MyGame pMyGame, int pCurrentLevel) {
		_myGame = pMyGame;
		_finishedLevel = false;
		_currentLevel = pCurrentLevel;
		_map = _tmxParser.ParseFile(MyGame.GetAssetFilePath(MyGame.Asset.ROOT) + "\\level_" + _currentLevel + ".tmx");
		_sounds = new Sounds();
		_sounds.PlayMusic();
		BuildLevel();
		CreatePlayer();
		CreateBall();
		//CreatePressurePlates();
		CreateTiledObjects();
		_engine = new PhysicsEngine(this);
		CreateReticle();
		RenderLines();
		
		// Assign layers to variables for ease of access.
		_foreGround = GetLayerByName("Foreground");
		_foreGroundPartTwo = GetLayerByName("Foreground_2");
        _foreGroundPartThree = GetLayerByName("Foreground_3");
        _midGround = GetLayerByName("Midground"); // note midGround is the base layer.
		_backGround = GetLayerByName("Background");
		_skyLines = GetLayerByName("Skylines");
		_skyLinesPartTwo = GetLayerByName("Skylines_2");
		_cloudLayer = GetLayerByName("Cloud");
		_cloudLayerPartTwo = GetLayerByName("Cloud_2");
		_groundTiles = GetLayerByName("Groundtiles");
		_groundTilesPartTwo = GetLayerByName("Groundtiles_2");
		_backGroundFar = GetLayerByName("Backgroundfar");
		_backGroundFarPartTwo = GetLayerByName("Backgroundfar_2");
		_stonesBackground = GetLayerByName("Stonesbackground");
		_stonesBackgroundPartTwo = GetLayerByName("Stonesbackground_2");

		foreach (GameTile tile in _midGround.GetTiles()) {
			_collidables.Add(tile);
		}
	}

	private void Update() {
		if (!_finishedLevel) {
			if (!_isPaused) {
				wait++;
				_xOffset = game.x - this.x;
				_yOffset = game.y - this.y;

				_player.GetReticle().x = Input.mouseX + _xOffset;
				_player.GetReticle().y = Input.mouseY + _yOffset;

				PlayerCamera();
                HandleForegroundAlpha();
                _engine.HandlePlayer();
				_engine.HandleBall();
				_engine.CheckStones();
				_engine.HandleStickyBall();
				_engine.CheckPotCollision();
				_engine.CheckTrophyCollision();
				_engine.CheckRopeCollision();
				_engine.HandleDestructablePlanks();
				_engine.CheckStickyBall();

                if (_playerDirection == Player.Direction.LEFT) {
					// _player.Mirror(true, false);
					_ball.scaleX = -1.0f;
					_player.scaleX = -1.0f;
				} else if (_playerDirection == Player.Direction.RIGHT) {
					//_player.Mirror(true, false);
					_player.scaleX = 1.0f;
					_ball.scaleX = 1.0f;
				}
			}
		}

		if (!_finishedLevel) {
			if (Input.GetKeyUp(Key.UP)) {
				_finishedLevel = true;
				WinScreen ws = new WinScreen(_myGame, this);
				AddChild(ws);
                _sounds.StopMusic();
			}
			if (Input.GetKeyUp(Key.TILDE)) {
				if (!_isPaused) {
					_pauseMenu = new PauseMenu(_myGame, this);
					AddChild(_pauseMenu);
					Pausable.Pause();
					_isPaused = true;
				} else {
					_pauseMenu.Destroy();
					Pausable.UnPause();
					_isPaused = false;
				}
			}
		}
        _sounds.StopMusic();
	}

    private void LevelTenWinningCondition() {
        if (_player.AmountOfTrophies == 3) {
            foreach (NLineSegment line in _lines.Where(s => s.LineName == "level_finish_point")) {
                line.Destroy();
            }
        }
    }

    private void HandleForegroundAlpha () {
        NLineSegment hellPlankLine = _planks.First(s => s.SpriteName == "hell_plank").PlankLine;
        if (_player.x > (hellPlankLine.start.x) - 64 && _player.x < (hellPlankLine.end.x) + 64) {
            foreach (GameTile tile in _foreGround.GetTiles()) {
                tile.ChangeAlpha(0.1f, true);
            }
            foreach (GameTile tile in _foreGroundPartTwo.GetTiles()) {
                tile.ChangeAlpha(0.1f, true);
            }
        } else {
            foreach (GameTile tile in _foreGround.GetTiles()) {
                tile.ChangeAlpha(0.1f, false);
            }
            foreach (GameTile tile in _foreGroundPartTwo.GetTiles()) {
                tile.ChangeAlpha(0.1f, false);
            }
        }
    }

	public void CreateHUD() {
		_hud = new HUD(this);
		game.AddChild(_hud);
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

        if (_currentLevel != 10) {
            if (x < -(_map.GetLevelWidth() - game.width)) {
                x = -(_map.GetLevelWidth() - game.width);
            }
        } else {
            if (x < -((_map.GetLevelWidth() - 7 * 64) - game.width)) {
                x = -((_map.GetLevelWidth() - 7 * 64) - game.width);
            }
        }
		

		if (x < 0 && x > -(_map.GetLevelWidth() - game.width)) {
			//_cloudLayer, _skyLines, _groundTiles, _backGroundFar, _stonesBackground;
			if (_player.IsMoving) {
				if (_playerDirection == Player.Direction.RIGHT && _engine.collision.dir != CollidedOption.Direction.LEFT) {
					_foreGround.MoveLayer(Layer.Direction.LEFT, 4.5f);
					_foreGroundPartTwo.MoveLayer(Layer.Direction.LEFT, 4.5f);
                    _foreGroundPartThree.MoveLayer(Layer.Direction.LEFT, 4.5f);

                    _groundTiles.MoveLayer(Layer.Direction.LEFT, 0.075f);
					_groundTilesPartTwo.MoveLayer(Layer.Direction.LEFT, 0.075f);
					_stonesBackground.MoveLayer(Layer.Direction.LEFT, 0.006f);
					_stonesBackgroundPartTwo.MoveLayer(Layer.Direction.LEFT, 0.006f);
					MoveTrees(6.5f);
				} else if (_playerDirection == Player.Direction.LEFT && _engine.collision.dir != CollidedOption.Direction.RIGHT) {
					_foreGround.MoveLayer(Layer.Direction.RIGHT, 4.5f);
					_foreGroundPartTwo.MoveLayer(Layer.Direction.RIGHT, 4.5f);
                    _foreGroundPartThree.MoveLayer(Layer.Direction.RIGHT, 4.5f);


                    _groundTiles.MoveLayer(Layer.Direction.RIGHT, 0.075f);
					_groundTilesPartTwo.MoveLayer(Layer.Direction.RIGHT, 0.075f);
					_stonesBackground.MoveLayer(Layer.Direction.RIGHT, 0.006f);
					_stonesBackgroundPartTwo.MoveLayer(Layer.Direction.RIGHT, 0.006f);
					MoveTrees(-6.5f);
				}
			}
		}
		_skyLines.MoveLayer(Layer.Direction.LEFT, 0.05f);
		_skyLinesPartTwo.MoveLayer(Layer.Direction.LEFT, 0.05f);
		_cloudLayer.MoveLayer(Layer.Direction.LEFT, 0.5f);
		_cloudLayerPartTwo.MoveLayer(Layer.Direction.LEFT, 0.5f);
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

            if (_currentLevel != 10) {
                if (layer.Name == "Foreground" || layer.Name == "Skylines" || layer.Name == "Cloud" || layer.Name == "Groundtiles" || layer.Name == "Stonesbackground") {
                    Layer nLayer = new Layer(this, layer);
                    nLayer.x = _map.GetLevelWidth();
                    AddChild(nLayer);
                    _layers.Add(layer.Name + "_2", nLayer);
                }
            } else {
                if (layer.Name == "Skylines" || layer.Name == "Cloud" || layer.Name == "Groundtiles" || layer.Name == "Stonesbackground") {
                    Layer nLayer = new Layer(this, layer);
                    nLayer.x = _map.GetLevelWidth();
                    AddChild(nLayer);
                    _layers.Add(layer.Name + "_2", nLayer);
                }

                if (layer.Name == "Foreground") {
                    for (int i = 1; i <= 2; i++) {
                        Layer nLayer = new Layer(this, layer);
                        nLayer.x = (_map.GetLevelWidth() - (15 * 64)) * i;
                        AddChild(nLayer);
                        _layers.Add(layer.Name + "_" + (i + 1), nLayer);
                    }
                    
                }
            }
			
		}
	}

	private void CreatePlayer() {
		_player = new Player(200, game.height / 2 + 64);
		AddChildAt(_player, 8);
	}

	private void CreateReticle() {
		AddChild(_player.GetReticle());
	}

	public void CreateIndicator() {
		//_sounds.PlayCharge();
		_player.SetIndicator(new Indicator());
		AddChild(_player.GetIndicator());
	}

	private void CreateBall() {
		_ball = new Ball(28, new Vec2(game.width / 2, game.height / 2), null, Color.Coral);
		AddChildAt(_ball, 100);
		_ball.Velocity = new Vec2();
		_ballToLine = new LineSegment(null, null);
		AddChild(_ballToLine);
	}

	private void CreateTiledObjects() {
		foreach (ObjectGroup objGroup in _map.ObjectGroup) {
			if (objGroup.Name == "Bridge") {
				foreach (TiledObject obj in objGroup.Object) {
					Bridge bridge = new Bridge();
					bridge.x = obj.X + bridge.width / 2;
					bridge.y = obj.Y + bridge.height;
					bridge.BridgeName = obj.Properties.GetPropertyByName("bridge_name").Value;
					bridge.SpriteName = obj.Name;
					_bridges.Add(bridge);
                    if (_currentLevel != 10) {
                        AddChildAt(bridge, 12);
                    } else {
                        AddChildAt(bridge, 29);
                    }
					_pressurePlateObjects.Add(bridge);
					bridge.BridgeCollider = new BridgeCollider(obj.X + bridge.width / 2 , obj.Y - 180, 450, 400);
					_bridgeColliders.Add(bridge.BridgeCollider);
				}
			}
			if (objGroup.Name == "GravityChanger") {
				try {
					foreach (TiledObject obj in objGroup.Object) {
						//Console.WriteLine(Convert.ToInt32(obj.Properties.GetPropertyByName("Direction").Value));
						GravityChanger gravityChanger = new GravityChanger(obj.X, obj.Y, obj.Width, obj.Height, Convert.ToInt32(obj.Properties.GetPropertyByName("Direction").Value));
						gravityChanger.Name = obj.Name;
						AddChild(gravityChanger);
						_gravityChangers.Add(gravityChanger);
					}
				} catch {

				}
			}
			if (objGroup.Name == "Fan") {
				try
				{
					foreach (TiledObject obj in objGroup.Object)
					{
						string fanConnectedTo = obj.Properties.GetPropertyByName("connected_to").Value;
						Fan fan = null;
						if (_gravityChangers.First(s => s.Name == fanConnectedTo).Direction == 2)
						{ // right
							fan = new Fan(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\fanright.png", 2, 15);
						}
						else if (_gravityChangers.First(s => s.Name == fanConnectedTo).Direction == 3)
						{ // up
							fan = new Fan(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\fanup.png", 15, 2);
						}
						else if (_gravityChangers.First(s => s.Name == fanConnectedTo).Direction == 1)
						{ // down
							fan = new Fan(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\fandown.png", 2, 15);
						}
						else if (_gravityChangers.First(s => s.Name == fanConnectedTo).Direction == 4)
						{ // left
							fan = new Fan(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\fanleft.png", 2, 15);
						}

						fan.x = obj.X + obj.Width / 2;
						fan.y = obj.Y + obj.Height / 2;
						AddChildAt(fan, 5);
					}
				}
				catch { }

			}
			if (objGroup.Name == "Rope") {
				foreach (TiledObject obj in objGroup.Object) {
					if (_currentLevel == 3 || _currentLevel == 5 || _currentLevel == 6 || _currentLevel == 7 || _currentLevel == 8 || _currentLevel == 9 || _currentLevel == 10) {
						Rope rope = new Rope(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\ropelong.png");
						rope.x = obj.X;
						rope.y = obj.Y + 4;
                        rope.BridgeToDrop = obj.Properties.GetPropertyByName("bridge_to_drop").Value;
                        rope.SpriteName = obj.Name;
                        if (_currentLevel != 10) {
                            rope.rotation = 330;
                        } else {
                            if (rope.SpriteName == "Rope_1") {
                                rope.rotation = 280;
                            } else if (rope.SpriteName == "Rope_2") {
                                rope.rotation = 0;
                            } else {
                                rope.rotation = 330;
                            }
                        }
						_ropes.Add(rope);
						_pressurePlateObjects.Add(rope);
                        if (_currentLevel != 10) {
                            AddChildAt(rope, 20);
                        } else {
                            if (rope.SpriteName == "Rope_2") {
                                AddChildAt(rope, 35);
                            } else {
                                AddChildAt(rope, 20);
                            }
                        }
						
						rope.PathBlockName = obj.Properties.GetPropertyByName("path_blocker_name").Value;
					} 
				}
			}
			if (objGroup.Name == "Pots") {
				try {
					foreach (TiledObject obj in objGroup.Object) {
						Pot pot = new Pot();
						pot.x = obj.X + obj.Width / 2;
						pot.y = obj.Y + obj.Height / 2;
						pot.SpriteName = obj.Name;
						_pots.Add(pot);
                        if (_currentLevel != 10) {
                            AddChildAt(pot, 8);
                        } else {
                            if (obj.Name == "hell_pot_1" || obj.Name == "hell_pot_2" || obj.Name == "hell_pot_3" || obj.Name == "hell_pot_4") {
                                AddChildAt(pot, 40);
                            } else {
                                AddChildAt(pot, 8);
                            }
                        }
						_pressurePlateObjects.Add(pot);
						pot.Canvas.x = pot.x - pot.width / 2;
						pot.Canvas.y = pot.y - pot.height * 0.8f;
						AddChildAt(pot.Canvas, 100);
					}
				}
				catch { }
			}
			if (objGroup.Name == "Planks") {
				try
				{
					foreach (TiledObject obj in objGroup.Object)
					{
						Plank plank = new Plank();
						plank.x = obj.X + obj.Width / 2;
						plank.y = obj.Y + obj.Height / 2;
						plank.Position.x = plank.x;
						plank.Position.y = plank.y;
						plank.SpriteName = obj.Name;
						_pressurePlateObjects.Add(plank);
						_planks.Add(plank);
						_destroyables.Add(plank);
                        if (_currentLevel != 10) {
                            AddChildAt(plank, 5);
                        } else { // hell_plank
                            if (obj.Name == "plank_4") {
                                plank.rotation = 90;
                            }
                            if (obj.Name == "hell_plank") {
                                plank.x -= 32;
                                AddChildAt(plank, 40);
                            } else if (obj.Name == "plank_7") {
                                AddChildAt(plank, 35);
                            } else {
                                AddChildAt(plank, 5);
                            }
                        }
						_lines.Add(plank.GetLine());
						AddChild(plank.PlankLine);
					}
				}
				catch { }
			}
			if (objGroup.Name == "Stones") {
				try {
					foreach (TiledObject obj in objGroup.Object) {
						//25, new Vec2(_ball.x, _ball.y), null, Color.Blue, false
						Stone stone = new Stone(28, new Vec2(obj.X + obj.Width / 2, obj.Y + obj.Height / 2), null, Color.Blue, Convert.ToBoolean(obj.Properties.GetPropertyByName("Active").Value));
						AddChildAt(stone, 5);
						_stones.Add(stone);
						stone.Velocity = Vec2.zero;
					}
				} catch {

				}
			}
			if (objGroup.Name == "Pressureplates") {
				//Console.WriteLine("test");
				try
				{
                    foreach (TiledObject obj in objGroup.Object)
					{
						PressurePlate _pressurePlate = new PressurePlate(this, obj.X + obj.Width / 2, obj.Y + obj.Height, obj.Properties.GetPropertyByName("ItemToInteract").Value, Convert.ToBoolean(obj.Properties.GetPropertyByName("HasCover").Value), 64, 128, obj.Name);
                        AddChildAt(_pressurePlate, 5);
						_pressurePlates.Add(_pressurePlate);
					}
				} catch {

				}
			}
			if (objGroup.Name == "Trophies") {
				try {
					foreach (TiledObject obj in objGroup.Object) {
						Trophy trophy = new Trophy(Convert.ToInt32(obj.Properties.GetPropertyByName("number").Value), MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\trophy_sprite.png", 8, 8);
						trophy.x = obj.X + obj.Width / 4;
						trophy.y = obj.Y + obj.Height / 4;
                        if (_currentLevel == 10) {
                            AddChildAt(trophy, 100);
                        } else {
                            AddChild(trophy);
                        }
						_trophies.Add(trophy);
						trophy.SpriteName = obj.Name;
						_pressurePlateObjects.Add(trophy);
					}
				} catch {

				}
			}
			if (objGroup.Name == "Finish") {
				try {
					foreach (TiledObject obj in objGroup.Object) {
						Finish finish = new Finish(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\finish_sprite.png", 8, 8);
						finish.x = obj.X + obj.Width / 4;
						finish.y = obj.Y + obj.Height / 4;
						AddChild(finish);
						_trophies.Add(finish);
						finish.SpriteName = obj.Name;
					}
				} catch {

				}
			}
			if (objGroup.Name == "Points") {
					foreach (TiledObject obj in objGroup.Object) {
						foreach (Vec2 points in obj.Polyline.GetPointsAsVectorList()) {
							_line = new NLineSegment(new Vec2(obj.X, obj.Y), new Vec2(obj.X + points.x, obj.Y + points.y), 0xff252a2d, 4);
							_line.LineName = obj.Name;
							_lines.Add(_line);
						}
					}
			}
			if (objGroup.Name == "Stickyball") {
				try {
					foreach (TiledObject obj in objGroup.Object) {
						StickyBall stickyBall = new StickyBall();
						stickyBall.x = obj.X + obj.Width / 2;
						stickyBall.y = obj.Y + obj.Height / 2;
						AddChildAt(stickyBall, 5);
						_stickyBalls.Add(stickyBall);
					}
				}
				catch { }
			}
			if (objGroup.Name == "ForegroundTree") {
				//foreach (TiledObject obj in objGroup.Object) {
				//    CreateRandomTree(obj);
				//}
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
			AddChild(tree);
		} else {
			Tree tree = new Tree(MyGame.GetAssetFilePath(MyGame.Asset.SPRITES) + "\\tree_2.png"); // tree_2
			tree.x = pObj.X - pObj.Width;
			tree.y = pObj.Y + pObj.Height;
			_trees.Add(tree);
			AddChild(tree);
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

	public List<Item> GetTrophies() {
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

	public HUD GetHUD() {
		return _hud;
	}

	public List<GameObject> GetPressurePlateObjects() {
		return _pressurePlateObjects;
	}

	public List<StickyBall> GetStickyBalls() {
		return _stickyBalls;
	}

	public MyGame GetMyGame() {
		return _myGame;
	}
}
