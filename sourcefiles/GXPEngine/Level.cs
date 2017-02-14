using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GXPEngine;
using System.Collections.ObjectModel;
using System.Collections.Specialized;


public class Level:GameObject
{
    const int SPEED = 8;
    private Sounds _sounds;
    const int GRAVITY = 15;
    const int REPETITIONS=2;
    const float ELASTICITY = 0.8f;
    const float COLLISION_FRICTION = 0.8f;
    const int MAXPOWER = 30;
    private const string ASSET_FILE_PATH = "assets\\";

    private List<Plank> _destroyables;
    private Vec2 _gravity = new Vec2(0, 1);

    private bool _goingUp;
    private Ball _reticle;
    private const float BLASTSIZE=100;
    private const int WAITFORBOOM = 180;
    private float _yOffset;
    private float _xOffset;
    private int _explosionWait;
    private Indicator _indicator;
    private Vec2 _indicatorVec;

    private bool _moveTile = true;
    private float _playerXOffset;
    private bool _debug;

    //private float _playerTotalXOffset;
    //private float _levelXOffset;

    private TileDirection _midGroundTileDirection = TileDirection.NONE;
    private TileDirection _foreGroundTileDirection = TileDirection.NONE;

    private enum TileDirection {
        NONE,
        LEFT,
        RIGHT
    }

    public enum direction
    {
        none, middle, left, right,below,above
    }

    public struct CollidedOption
    {
        public direction dir;
        public Sprite obj;
    }

    private List<GameTile> _collidables;
    private List<LineSegment> _lines;
    private List<Stone> _stones;
    private List<Trophy> _trophies = new List<Trophy>();
    private List<Rope> _ropes = new List<Rope>();
    private List<Bridge> _bridges = new List<Bridge>();
    private List<BridgeCollider> _bridgeColliders = new List<BridgeCollider>();
    private List<Pot> _pots = new List<Pot>();
    private List<Plank> _planks = new List<Plank>();
    private List<GravityChanger> _gravityChangers = new List<GravityChanger>();

    private ObservableCollection<GameTile> _foreGroundTiles = new ObservableCollection<GameTile>();
    private ObservableCollection<GameTile> _midGroundTiles = new ObservableCollection<GameTile>();
    private ObservableCollection<GameTile> _backGroundTiles = new ObservableCollection<GameTile>();
    

    private Random rnd = new Random();

    private Player _player;
    private float _startingBallVelocity;
    private TMXParser _tmxParser = new TMXParser();
    private Map _map;

    private CollidedOption collision;

    private Ball _ball;
    private Vec2 _ballToLineStart;
    private Vec2 _intersection;
    private LineSegment _ballToLine;
    private LineSegment _line;
    private float _distance;

    private int _currentLevel;
    private uint[,] _level;
    private GameTile _tile;
    private List<Canvas> _canvases = new List<Canvas>();

    private List<PressurePlate> _pressurePlates;

    public int CurrentLevel
    {
        get { return _currentLevel; }
        set { _currentLevel = value; }
    }

    public Level(MyGame pMyGame, int pCurrentLevel)
    {
        _pressurePlates = new List<PressurePlate>();
        _currentLevel = pCurrentLevel;
        _map = _tmxParser.ParseFile(ASSET_FILE_PATH + "level_" + _currentLevel + ".tmx");
        _sounds = new Sounds();
        _destroyables = new List<Plank>();
        _collidables = new List<GameTile>();
        _lines = new List<LineSegment>();
        _sounds.PlayMusic();
        _startingBallVelocity = SPEED / 2;

        _stones = new List<Stone>();
        collision = new CollidedOption();
        CreateGravityChangers();

        CreateLevel();
        CreateStones();
        CreatePlayer();
        CreateBall();
        CreateTiledObjects();
        CreateReticle();
        CreatePressurePlates();

        for (int i = 0; i < _foreGroundTiles.Count; i++) {
            _foreGroundTiles[i].TileIndex = i;
            Canvas canvas = new Canvas(64, 64);
            canvas.x = _foreGroundTiles[i].x;
            canvas.y = _foreGroundTiles[i].y;
            AddChild(canvas);
            _canvases.Add(canvas);
        }

        _midGroundTiles.CollectionChanged += _midGroundTiles_CollectionChanged;
        _foreGroundTiles.CollectionChanged += _foreGroundTiles_CollectionChanged;


    }

    private void CreateGravityChangers()
    {
        GravityChanger gravs = new GravityChanger(300, 1400, 128, 1000, "up");
        AddChild(gravs);
        _gravityChangers.Add(gravs);
    }

    private void CheckInGravityChangers(Ball ball)
    {
        foreach(GravityChanger gravchangers in _gravityChangers)
        {
            if(ball.position.x < gravchangers.x + gravchangers.width / 2 &&
               ball.position.x > gravchangers.x - gravchangers.width / 2 &&
               ball.position.y > gravchangers.y - gravchangers.height / 2 &&
               ball.position.y < gravchangers.y + gravchangers.height / 2)
            {
                ball.velocity.Add(gravchangers.changedGravity);
            }
        }
    }
    
    private void CreatePressurePlates()
    {
        PressurePlate _pressurePlate = new PressurePlate(832,1598+64,"first pressure plate",true,64,128);
        AddChild(_pressurePlate);
        _pressurePlates.Add(_pressurePlate);
    }

    private void CheckPressurePlatesCollision(Stone ball)
    {
        foreach(PressurePlate presspl in _pressurePlates)
        {
            if (ball.position.x < presspl.x + presspl.width / 2 &&
                ball.position.x > presspl.x - presspl.width / 2 &&
                ball.position.y < presspl.y && ball.position.y > presspl.y - ball.height/2)
            {
                presspl.OpenCoresponding();
                if (presspl.cover)
                {
                    ball.active = false;
                    ball.y = presspl.y - ball.height/2;
                    _lines.Add(presspl.coverLine);
                    AddChild(presspl.coverLine);
                }
            }
        }
    }

    private void _foreGroundTiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Move) {
            GameTile newestTile = (e.NewItems[0] as GameTile);
            HandleTileMovement(newestTile, _foreGroundTiles, _foreGroundTileDirection);
        }
    }

    private void _midGroundTiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Move) {
            GameTile newestTile = (e.NewItems[0] as GameTile);
            HandleTileMovement(newestTile, _midGroundTiles, _midGroundTileDirection);
        }
    }

    private void HandleTileMovement(GameTile pGameTile, ObservableCollection<GameTile> pGameTileList, TileDirection pTileDirection) {
        if (pTileDirection == TileDirection.RIGHT) {
            pGameTile.x = pGameTileList.ElementAt(pGameTileList.IndexOf(pGameTile) - 1).x + pGameTile.width;
            pGameTile.y = pGameTileList.ElementAt(pGameTileList.IndexOf(pGameTile) - 1).y;
        } else if (pTileDirection == TileDirection.LEFT) {
            pGameTile.x = pGameTileList.ElementAt(pGameTileList.IndexOf(pGameTile) + 1).x - pGameTile.width;
            pGameTile.y = pGameTileList.ElementAt(pGameTileList.IndexOf(pGameTile) + 1).y;
        }
    }

    public void Update()
    {
        _xOffset = game.x - this.x;
        _yOffset = game.y - this.y;
        _reticle.x = Input.mouseX + _xOffset;
        _reticle.y = Input.mouseY + _yOffset;
        PlayerCamera();
        HandleBall();
        //HandlePlayer();
        CheckStones();
        BallBoom();
        CheckTrophyCollision();
        CheckRopeCollision();
        CheckPotCollision();
        HandleExplosiveBallInteractionWithPlanks();
    }

    private void CreatePlayer() {
        _player = new Player(200, game.height / 2);
        AddChildAt(_player, 30);
    }

    private void CreateReticle() {
        _reticle = new Ball(7, new Vec2(game.width / 2, game.height / 2), null, Color.Green);
        AddChild(_reticle);
    }

    private void CreateTiledObjects() {
        foreach (ObjectGroup objGroup in _map.ObjectGroup) {
            if (objGroup.Name == "Bridge") {
                foreach (TiledObject obj in objGroup.Object) {
                    
                    Bridge bridge = new Bridge(315);
                    bridge.x = obj.X+bridge.width/2;
                    bridge.y = obj.Y;
                    bridge.BridgeName = obj.Properties.GetPropertyByName("bridge_name").Value;
                    _bridges.Add(bridge);
                    AddChild(bridge);

                    bridge.bridgeCollider = new BridgeCollider(obj.X+90, obj.Y-190, 400, 400);
                    _bridgeColliders.Add(bridge.bridgeCollider);
                    AddChild(bridge.bridgeCollider);
                }
            }
            if (objGroup.Name == "Rope") {
                foreach (TiledObject obj in objGroup.Object) {
                    Rope rope = new Rope();
                    rope.x = obj.X;
                    rope.y = obj.Y;
                    rope.rotation = 340;
                    rope.BridgeToDrop = obj.Properties.GetPropertyByName("bridge_to_drop").Value;
                    _ropes.Add(rope);
                    AddChild(rope);
                }
            }
            if (objGroup.Name == "Pots") {
                foreach (TiledObject obj in objGroup.Object) {
                    Pot pot = new Pot();
                    pot.x = obj.X + obj.Width / 2;
                    pot.y = obj.Y + obj.Height / 2;
                    _pots.Add(pot);
                    AddChildAt(pot, 0);
                    pot.Canvas.x = pot.x - pot.width / 2;
                    pot.Canvas.y = pot.y - pot.height * 0.8f;
                    AddChildAt(pot.Canvas, 101);
                }
            }
            if (objGroup.Name == "Planks") {
                foreach (TiledObject obj in objGroup.Object) {
                    Plank plank = new Plank();
                    plank.x = obj.X + obj.Width/2;
                    plank.y = obj.Y + obj.Height/2;// - obj.Height / 4;
                    plank.position.x = plank.x;
                    plank.position.y = plank.y;
                    _planks.Add(plank);
                    _destroyables.Add(plank);
                    AddChild(plank);
                    plank.collider.start = new Vec2(plank.x - plank.width / 2, plank.y);
                    plank.collider.end = new Vec2(plank.x + plank.width / 2, plank.y);
                    _lines.Add(plank.collider);
                    AddChild(plank.collider);
                }
            }
            if (objGroup.Name == "ForegroundTree") {
                foreach (TiledObject obj in objGroup.Object) {
                    Tree tree = new Tree(ASSET_FILE_PATH + "sprites\\tree_try.png"); // tree_1
                    tree.x = obj.X - obj.Width;
                    tree.y = obj.Y + obj.Height;
                    AddChildAt(tree, 100);
                }
            }
        }
    }

    private void CreateBall()
    {
        _ball = new Ball(25, new Vec2(game.width / 2, game.height / 2), null, Color.Coral);
        AddChildAt(_ball, 31);
        _ball.velocity = new Vec2();
        _ballToLine = new LineSegment(null, null);
        AddChild(_ballToLine);
    }

    private void CreateStones()
    {
        Stone _stone = new Stone(25, new Vec2(600,1450 ), null, Color.Blue, false);
        AddChild(_stone);
        _stones.Add(_stone);
        _stone.velocity = Vec2.zero;

        _stone = new Stone(25, new Vec2(750, 1500), null, Color.Blue, false);
        AddChild(_stone);
        _stones.Add(_stone);
        _stone.velocity = Vec2.zero;

        _stone = new Stone(25, new Vec2(1400, 1340), null, Color.Blue, false);
        AddChild(_stone);
        _stones.Add(_stone);
        _stone.velocity = Vec2.zero;

        _stone = new Stone(25, new Vec2(1500, 1340), null, Color.Blue, false);
        AddChild(_stone);
        _stones.Add(_stone);
        _stone.velocity = Vec2.zero;

        _line = new NLineSegment(new Vec2(300, 100), new Vec2(700, 100), 0xffffff00, 4);
        AddChild(_line);
        _lines.Add(_line);
    }

    private void CreateLevel()
    {
        /* For when we use tiles */
        foreach (Layer layer in _map.Layer)
        {
            layer.Data.SetLevelArray(_map.Height, _map.Width);
            _level = layer.Data.GetLevelArray();
            for (int row = 0; row < _level.GetLength(0); row++)
            {
                for (int col = 0; col < _level.GetLength(1); col++)
                {
                    uint tile = _level[row, col];
                    CreateTile(layer, row, col, tile);
                }
            }
        }

        foreach (ObjectGroup objGroup in _map.ObjectGroup)
        {
            if (objGroup.Name == "Points")
            {
                foreach (TiledObject obj in objGroup.Object)
                {
                    foreach (Vec2 points in obj.Polyline.GetPointsAsVectorList())
                    {
                        _line = new NLineSegment(new Vec2(obj.X, obj.Y), new Vec2(obj.X + points.x, obj.Y + points.y), 0xffffff00, 4);
                        _lines.Add(_line);
                    }
                }
            }
            if (objGroup.Name == "Trophies") {
                foreach (TiledObject obj in objGroup.Object) {
                    Trophy trophy = new Trophy(ASSET_FILE_PATH + "sprites\\PumpSprite.png", 8, 8);
                    trophy.x = obj.X + obj.Width / 4;
                    trophy.y = obj.Y + obj.Height / 4;
                    _trophies.Add(trophy);
                }
            }
        }

        //REMOVE HERE TO REMOVE RENDERING OF LINES
        foreach (NLineSegment line in _lines)
        {
            AddChild(line);
        }

        foreach (Trophy trophy in _trophies) {
            AddChild(trophy);
        }
    }

    private void CreateTile(Layer pLayer, int pRow, int pCol, uint pTile)
    {
        // It gets the first tileset in order to create the level.
        // so the designer has to make sure its the first one,
        // because otherwise every tileset will be created.

        // Unbreakable Wall
        if (pTile != 0) {
            foreach (TileSet tileSet in _map.TileSet) {
                if (pLayer.Name.ToLower() == tileSet.Name.ToLower()) {
                    _tile = new GameTile(this, pLayer, ASSET_FILE_PATH + tileSet.Image.Source, pTile - (uint)tileSet.FirstGId, tileSet.Columns, tileSet.TileCount / tileSet.Columns);
                    _tile.x = (pCol * _map.TileWidth) + (_tile.width / 2);
                    _tile.y = (pRow * _map.TileHeight) + (_tile.height / 2);

                    AddChild(_tile);

                    if (tileSet.Name.ToLower() == "background") {
                        _backGroundTiles.Add(_tile);
                    }
                    if (tileSet.Name.ToLower() == "midground") {
                        _collidables.Add(_tile);
                        _midGroundTiles.Add(_tile);
                    }
                    if (tileSet.Name.ToLower() == "foreground") {
                        _foreGroundTiles.Add(_tile);
                    }
                }
            }
        }

        if (_tile != null) {
            _tile.position = new Vec2(_tile.x, _tile.y);
        }
    }

    private void PlayerCamera()
    {
        y = game.height / 1.25f - _player.y;

        if (_player.IsMoving) {
            x = game.width / 2 - _player.x;
        }

        if (x > 0)
        {
            x = 0;
        }

        if (y > 0)
        {
            y = 0;
        }

        if (y < -(game.height))
        {
            y = -(game.height);
        }

        if (x < -((game.width * 3) - (game.width / 5)))
        {
            x = -((game.width * 3) - (game.width / 5));
        }

        if (x < 0) {
            SceneryCamera(-_playerXOffset * 2f);
            if (x % 64 == 0) {
                if (!_moveTile) {
                    return;
                }
                if (_moveTile) {
                    if (_playerXOffset < 0) {
                        if (_midGroundTileDirection == TileDirection.LEFT) {
                            _midGroundTiles.Move(25, 0); //25,0
                            _midGroundTiles.Move(51, 26);
                        }

                        if (_foreGroundTileDirection == TileDirection.LEFT) {
                            _foreGroundTiles.Move(25, 0);
                            _foreGroundTiles.Move(51, 26);
                            _foreGroundTiles.Move(77, 52);
                            _foreGroundTiles.Move(103, 78);
                        }
                    } else if (_playerXOffset > 0) {
                        if (_midGroundTileDirection == TileDirection.RIGHT) {
                            _midGroundTiles.Move(0, 25); // 0,25
                            _midGroundTiles.Move(26, 51);
                        }

                        if (_foreGroundTileDirection == TileDirection.RIGHT) {
                            _foreGroundTiles.Move(0, 25);
                            _foreGroundTiles.Move(26, 51);
                            _foreGroundTiles.Move(52, 77);
                            _foreGroundTiles.Move(78, 103);
                        }
                    }
                }
                _moveTile = false;
            } else {
                _moveTile = true;
            }
            
        }
    }

    private void SceneryCamera(float pXOffset) {
        foreach (GameTile tile in _foreGroundTiles) {
            tile.x += pXOffset;
        }
    }

    private void CreateIndicator()
    {
        //Console.WriteLine("CurrentFPS = " + game.currentFps);
        //Console.WriteLine(_midGroundTileDirection);
        for (int i = 0; i < _canvases.Count; i++) {
            _canvases[i].graphics.Clear(Color.Transparent);
            _canvases[i].graphics.DrawString("" + _foreGroundTiles[i].TileIndex, new Font(FontFamily.GenericMonospace, 8), Brushes.White, 0, 0);
        }

        _xOffset = game.x - this.x;
        _yOffset = game.y - this.y;

        _sounds.PlayCharge();
        _indicator = new Indicator();
        AddChild(_indicator);
    }

    private void HandleIndicator(int pPower)
    {
        _indicator.x = _player.x;
        _indicator.y = _player.y;
        _indicator.rotation = _indicatorVec.GetAngleDegrees()+90;
        _indicator.SetPower(pPower);
    }

    private void RemoveIndicator()
    {
        _goingUp = true;
        if(_indicator!=null)
        _indicator.Destroy();
        _indicator = null;
    }

    private void HandleBall()
    {

        if (Input.GetKeyDown(Key.E))
        {
            _sounds.PlaySwitch();
            _ball.IsExploding = !_ball.IsExploding;
        }
        if (_ball.OnPlayer)
        {
            _ball.x = _player.x-64;
            _ball.y = _player.y-64;
        }
        if (Input.GetMouseButton(0) && _ball.OnPlayer)
        {
            if (_indicator == null)
                CreateIndicator();

            if (_startingBallVelocity > MAXPOWER || _startingBallVelocity < 0)
                _goingUp=!_goingUp;

            if (_goingUp)
                _startingBallVelocity += 0.3f;
            else
                _startingBallVelocity -= 0.3f;

            _indicatorVec = new Vec2(Input.mouseX - _player.x + _xOffset, Input.mouseY - _player.y + _yOffset);
            HandleIndicator((int)_startingBallVelocity/4);
        }
        else if (Input.GetMouseButtonUp(0) && _ball.OnPlayer)
        {
            _ball.position.x = _player.x;
            _ball.position.y = _player.y;
            _ball.velocity = _indicatorVec.Clone();
            _ball.velocity.Normalize().Scale(_startingBallVelocity);
            _ball.OnPlayer = false;
            _startingBallVelocity = SPEED;
            RemoveIndicator();
            _sounds.StopCharge();
            _sounds.PlayShoot();
        }
        else if (!_ball.OnPlayer)
        {
            CheckInGravityChangers(_ball);
            _ball.velocity.Add(_gravity);
            for (int i = 0; i <= REPETITIONS; i++)
            {
                _ball.Step();
                CheckAllLines(_ball);
            }
        }
        HandlePlayer();
        CheckStones();
        BallBoom();
        CheckTrophyCollision();
        CheckRopeCollision();
        CheckPotCollision();
        HandleExplosiveBallInteractionWithPlanks();
    }

    private void HandleExplosiveBallInteractionWithPlanks() {
        if (_ball.IsExploding) {
            foreach (Plank plank in _planks) {
                if (_ball.HitTest(plank)) {
                    _ball.velocity = new Vec2();
                    _ball.StartedTimer = true;
                    _ball.OnPlayer = true;
                }
            }
        }
    }
    
    private void CheckRopeCollision() {
        foreach (Rope rope in _ropes) {
            if (_ball.HitTest(rope)) {
                if (!rope.IsDestroyed()) {
                    foreach (Bridge bridge in _bridges) {
                        if (bridge.BridgeName == rope.BridgeToDrop) {
                            bridge.y += bridge.height / 2-64;
                            bridge.rotation = 0;
                            bridge.Down = true;
                            bridge.bridgeCollider.y += 300;
                            bridge.bridgeCollider.x += 100;
                            _sounds.PlayBridgeFall();
                        }
                    }
                }
                _sounds.PlayCutRope();
                rope.Destroy();
            }
        }
    }

    private void BallBoom()
    {
        if (_ball.StartedTimer)
        {
            if (_explosionWait == WAITFORBOOM)
            {
                _sounds.PlayExplosion();
                for (int i=0; i < _destroyables.Count;i++) {
                    Plank plank = _destroyables[i];
                    if (_ball.position.DistanceTo(plank.position) < BLASTSIZE)
                    {
                        _sounds.PlayPlankBlow();
                        _destroyables.Remove(plank);
                        plank.Destroy();
                        i--;
                    }
                }
                ResetBall();
                _explosionWait = 0;
                _ball.StartedTimer = false;
            }
            _explosionWait++;
        }
    }

    private void CreateForegroundTrees() {
        foreach (ObjectGroup objGroup in _map.ObjectGroup) {
            if (objGroup.Name == "ForegroundTree") {
                foreach (TiledObject obj in objGroup.Object) {
                    Tree tree = new Tree(ASSET_FILE_PATH + "sprites\\tree_try.png");
                    tree.x = obj.X - obj.Width;
                    tree.y = obj.Y + obj.Height;
                    AddChildAt(tree, 100);
                }
            }
        }
    }

    private void CheckTrophyCollision() {
        foreach (Trophy trophy in _trophies) {
            if (_ball.HitTest(trophy)) {
                if (!trophy.IsDestroyed()) {
                    _player.AmountOfTrophies++;
                }
                trophy.Destroy();
                _sounds.PlayPickUp();
            }
        }
    }

    private void CheckPotCollision() {
        if (!_ball.OnPlayer) {
            foreach (Pot pot in _pots) {
                if (_ball.HitTest(pot)) {
                    if (!pot.IsDestroyed()) {
                        int score = rnd.Next(50, 750);
                        _player.Score += score;
                        pot.Canvas.graphics.DrawString("+" + score, new Font(FontFamily.GenericSansSerif, 18, FontStyle.Italic), Brushes.Green, 0, 0);
                        new Timer(10000, pot.Canvas.Destroy);
                    }
                    _sounds.PlayBreakPot();
                    pot.Destroy();
                }
            }
        }
    }

    private void CheckStones()
    {
        for (int i=0;i<_stones.Count;i++)
        {
            /// <summary>
            /// Fix stone distance it is now half of the distance it should be radius minus distance/2
            /// Add a check to see if after reset ball is still between lines
            /// </summary>

            if (_stones[i].position.DistanceTo(_ball.position) < _stones[i].radius + _ball.radius && !_stones[i].hitPlayer)
            {
                //_sounds.PlayBallRockCollision();
                float _tempdistance = _stones[i].position.DistanceTo(_ball.position); ;
                Vec2 _stoneToStone = _stones[i].position.Clone().Subtract(_ball.position).Normalize();
                //_stones[i].position.Add(_stoneToStone.Scale(0.5f));
                _ball.position.Subtract(_stoneToStone.Scale(_ball.radius-_tempdistance/2));
                _stones[i].velocity = _ball.velocity.Clone();//new Vec2(1, 0).Scale(_ball.velocity.Length());
                CheckAllLines(_stones[i]);
                _stones[i].Step(); 
                _ball.velocity = Vec2.zero;
                //_ball.position.Clone().Subtract(_stones[i].position).Normalize()
                //_ball.velocity.ReflectOnPoint(_ball.position.Clone().Subtract(_stones[i].position).Normalize(), 1);
                CheckAllLines(_ball);
                _ball.Step();
                //CollisionFix2Balls(stone, _ball);.Scale
                _stones[i].active = true;
                //stone.hitPlayer = true;
            }
            if (_stones[i].active)
            {
                CheckInGravityChangers(_stones[i]);
                _stones[i].velocity.Add(_gravity);
                for (int j = 0; j < REPETITIONS; j++)
                {
                    CheckPressurePlatesCollision(_stones[i]);
                    CheckAllLines(_stones[i]);
                    _stones[i].Step();
                    //_sounds.PlayRockBounce();
                }
            }
            for (int j=0;j<_stones.Count;j++)
            {
                float _tempDistance = _stones[i].position.DistanceTo(_stones[j].position);
                if (j!=i &&  _tempDistance < _stones[i].radius + _stones[j].radius)
                {
                    //stone.position.x - ();
                    //stone.position.y - ();
                    Vec2 _stoneToStone = _stones[i].position.Clone().Subtract(_stones[j].position).Normalize();
                    _stones[i].position.Add(_stoneToStone.Scale( _stones[i].radius-_tempDistance/2));
                    //_stones[j].position.Subtract(_stoneToStone.Scale(0.5f));
                    _stones[j].active = true;
                    //if (!stone2.started)
                    {
                        //_sounds.PlayRockBounce();
                        _stones[j].velocity = _stones[i].velocity.Clone();//new Vec2(1, 0).Scale(stone.velocity.Length());
                       // stone2.started = true;
                    }
                    _stones[i].hitPlayer = false;
                    _stones[i].velocity.Scale(0.0f);
                    //CheckAllLines(_stones[i]);
                    //CheckAllLines(_stones[j]);
                    _stones[i].Step();
                    _stones[j].Step();
                }
            }
            

        }
    }

    private void ResetBall()
    {
        _ball.position.x = _player.x;
        _ball.position.y = _player.y;
        _ball.velocity = Vec2.zero;
        _ball.OnPlayer = true;
        _ball.Step();
    }

    private void HandlePlayer()
    {
        if (Input.GetKey(Key.D)) {
            _midGroundTileDirection = TileDirection.RIGHT;
            _foreGroundTileDirection = TileDirection.RIGHT;
            _player.IsMoving = true;
            _player.position.x += SPEED / 2;
        } else if (Input.GetKey(Key.A)) {
            _midGroundTileDirection = TileDirection.LEFT;
            _foreGroundTileDirection = TileDirection.LEFT;
            _player.IsMoving = true;
            _player.position.x -= SPEED / 2;
        } else {
            _player.IsMoving = false;
        }

        if (Input.GetKeyDown(Key.SPACE))
        {
            if (!_player.jumped)
            {
                _sounds.PlayJump();
                _debug = true;
                _player.position.y--;
                _player.velocity.y = -GRAVITY;
                _player.jumped = true;
            }
        }
        if (Input.GetKeyDown(Key.R))
        {
            ResetBall();   
        }

        CheckPlayerCollision(_player, ref collision);

        if (collision.dir != direction.none)
        {
            //Console.WriteLine(collision.dir);
            if (collision.dir == direction.above)
            {
                //_sounds.PlayWalk();
                
                //Console.WriteLine(collision.obj);
                _player.position.y = collision.obj.y - collision.obj.height / 2 - _player.height / 2;
                _player.velocity = Vec2.zero;
                
                _player.jumped = false;
            }
            if (collision.dir == direction.below)
            {
                _player.position.y = collision.obj.y + collision.obj.height / 2 + _player.height / 2;
                _player.velocity = Vec2.zero;
            }
            if (collision.dir == direction.right)
            {
                _player.position.x = collision.obj.x + collision.obj.width / 2 + _player.width / 2;
                _player.velocity = Vec2.zero;
            }
            if (collision.dir == direction.left)
            {
                _player.position.x = collision.obj.x - collision.obj.width / 2 - _player.width / 2;
                _player.velocity = Vec2.zero;
            }
        }
        else if (collision.obj == null)
        {
            _player.velocity.y += _gravity.y;
        }

        if (_player.IsMoving) {
            _playerXOffset = _player.position.x - _player.x;
        } else {
            _playerXOffset = 0;
        }

        _player.Step();
    }

    public void CheckPlayerCollision(Player pPlayer, ref CollidedOption co)
    {
        co.dir = direction.none;
        co.obj = null;

        float _distanceX,_distanceY;

        for (int obj = 0; obj < _bridgeColliders.Count; obj++)
        {
            Sprite wall = _bridgeColliders[obj];
            _distanceX = wall.width / 2 + pPlayer.width / 2;
            _distanceY = wall.height / 2 + pPlayer.height / 2;
            if (pPlayer.position.x + _distanceX >= wall.x &&
                pPlayer.position.x - _distanceX <= wall.x &&
                pPlayer.position.y + _distanceY >= wall.y &&
                pPlayer.position.y - _distanceY <= wall.y)
            {
                if (pPlayer.position.x < wall.x - wall.width/2 + 20)//sees if who is on the left of the wall
                {
                    co.obj = wall;
                    co.dir = direction.left;
                    //Console.WriteLine("left");
                    return;
                }
                if (pPlayer.position.x > wall.x + wall.width/2 - 20)// sees if who is on the right of enemy5
                {
                    co.obj = wall;
                    co.dir = direction.right;
                    //Console.WriteLine("right");
                    return;
                }
            }
        }
        for (int obj = 0; obj < _bridgeColliders.Count; obj++)
        {
            Sprite wall = _bridgeColliders[obj];
            _distanceX = wall.width / 2 + pPlayer.width / 2;
            _distanceY = wall.height / 2 + pPlayer.height / 2;
            if (pPlayer.position.x + _distanceX >= wall.x &&
                pPlayer.position.x - _distanceX <= wall.x &&
                pPlayer.position.y + _distanceY >= wall.y &&
                pPlayer.position.y - _distanceY <= wall.y)
            {
                if (pPlayer.position.y < wall.y - wall.height / 2)
                {
                    co.obj = wall;
                    co.dir = direction.above;
                    //Console.WriteLine("above");
                    return;
                }
                if (pPlayer.position.y > wall.y + wall.height / 2)
                {
                    co.obj = wall;
                    co.dir = direction.below;
                    //Console.WriteLine("below");
                    return;
                }
            }
        }

        for (int obj = 0; obj < _collidables.Count; obj++)//goes through all the walls in the list
        {
            Sprite wall = _collidables[obj];//selects one of the walls
            _distanceX = wall.width / 2 + pPlayer.width / 2;//sets the horizontal distance between who and wall
            _distanceY = wall.height / 2 + pPlayer.height / 2;//sets the vertical distance between who and wall
            if (pPlayer.position.x + _distanceX >= wall.x &&
                pPlayer.position.x - _distanceX <= wall.x &&
                pPlayer.position.y + _distanceY - 20 >= wall.y &&
                pPlayer.position.y - _distanceY + 20 <= wall.y)//selects if who is inside the boundaries of the wall
            {
                if (pPlayer.position.x < wall.x - wall.width + 20)//sees if who is on the left of the wall
                {
                    co.obj = wall;
                    co.dir = direction.left;
                    //Console.WriteLine("left");
                    return;
                }

                if (pPlayer.position.x > wall.x + wall.width - 20)// sees if who is on the right of enemy5
                {
                    co.obj = wall;
                    co.dir = direction.right;
                    //Console.WriteLine("right");
                    return;
                }
            }
        }
        for (int obj = 0; obj < _collidables.Count; obj++)
        {
            Sprite wall = _collidables[obj];
            _distanceX = wall.width / 2 + pPlayer.width / 2;
            _distanceY = wall.height / 2 + pPlayer.height / 2;
            if (pPlayer.position.x + _distanceX >= wall.x &&
                pPlayer.position.x - _distanceX <= wall.x &&
                pPlayer.position.y + _distanceY >= wall.y &&
                pPlayer.position.y - _distanceY <= wall.y)
            {
                if (pPlayer.position.y < wall.y - wall.height / 2)
                {
                    co.obj = wall;
                    co.dir = direction.above;
                    //Console.WriteLine("above");
                    return;
                }

                if (pPlayer.position.y > wall.y + wall.height / 2)
                {
                    co.obj = wall;
                    co.dir = direction.below;
                    //Console.WriteLine("below");
                    return;
                }
            }

        }

        for (int obj = 0; obj < _destroyables.Count; obj++)//goes through all the walls in the list
        {
            Sprite wall = _destroyables[obj];//selects one of the walls
            _distanceX = wall.width / 2 + pPlayer.width / 2;//sets the horizontal distance between who and wall
            _distanceY = wall.height / 2 + pPlayer.height / 2;//sets the vertical distance between who and wall
            if (pPlayer.position.x + _distanceX >= wall.x &&
                pPlayer.position.x - _distanceX <= wall.x &&
                pPlayer.position.y + _distanceY - 20 >= wall.y &&
                pPlayer.position.y - _distanceY + 20 <= wall.y)//selects if who is inside the boundaries of the wall
            {
                if (pPlayer.position.x < wall.x - wall.width + 20)//sees if who is on the left of the wall
                {
                    co.obj = wall;
                    co.dir = direction.left;
                    //Console.WriteLine("left");
                    return;
                }

                if (pPlayer.position.x > wall.x + wall.width - 20)// sees if who is on the right of enemy5
                {
                    co.obj = wall;
                    co.dir = direction.right;
                    //Console.WriteLine("right");
                    return;
                }
            }
        }
        for (int obj = 0; obj < _destroyables.Count; obj++)
        {
            Sprite wall = _destroyables[obj];
            _distanceX = wall.width / 2 + pPlayer.width / 2;
            _distanceY = wall.height / 2 + pPlayer.height / 2;
            if (pPlayer.position.x + _distanceX >= wall.x &&
                pPlayer.position.x - _distanceX <= wall.x &&
                pPlayer.position.y + _distanceY >= wall.y &&
                pPlayer.position.y - _distanceY <= wall.y)
            {
                if (pPlayer.position.y < wall.y - wall.height / 2)
                {
                    co.obj = wall;
                    co.dir = direction.above;
                    //Console.WriteLine("above");
                    return;
                }

                if (pPlayer.position.y > wall.y + wall.height / 2)
                {
                    co.obj = wall;
                    co.dir = direction.below;
                    //Console.WriteLine("below");
                    return;
                }
            }

        }


        
       

        return;
    }

    private bool Inside(Vec2 v1, Vec2 v2, Vec2 v3)
    {
        if (v1.x <= v3.x && v2.x >= v3.x && v1.y <= v3.y && v2.y >= v3.y)
            return true;
        if (v1.x >= v3.x && v2.x <= v3.x && v1.y >= v3.y && v2.y <= v3.y)
            return true;
        return false;
    }

    private void CollisionFix2Balls(Ball ball1, Ball ball2)
    {
        Vec2 temp = ball2.velocity;
        ball2.velocity = ball1.velocity.Clone();
        ball1.velocity = temp.Clone();
    }

    void CheckAllLines(Ball ball)
    {
        ball.UpdateNextPosition();
        for (int i = 0; i < _lines.Count; i++)
        {
            ActualBounce(ball, _lines[i],ball.IsExploding);
        }
        bool noOverlap = false;
        int iterations = 0;
        int maxIterations = 20;
        do
        {
            noOverlap = true;
            for (int i = 0; i < _lines.Count; i++)
            {
                noOverlap = noOverlap & !CorrectOverlap(ball, _lines[i]); // ActualBounce(ball, _lines[i], ball.IsExploding);
            }
            iterations++;
        } while (!noOverlap && iterations < maxIterations);
        //if (iterations > 1)
        //    Console.WriteLine("Corrected {0} errors", iterations - 1);
    }

    // Return true if overlap was detected
    bool CorrectOverlap(Ball ball,LineSegment line)
    {
        Vec2 differenceVec = ball.position.Clone().Subtract(line.start);
        Vec2 normalizedLineVec = line.end.Clone().Subtract(line.start).Normalize();
        Vec2 lineNormal = normalizedLineVec.Normal();
        
        float distanceToLine = differenceVec.Dot(lineNormal);
        float distanceOnLine = differenceVec.Dot(normalizedLineVec);
        if(distanceOnLine<=line.lineLenght && distanceOnLine >= 0 && distanceToLine >= -ball.radius && distanceToLine <= ball.radius)
        {
            if (distanceToLine>0)
                ball.position.Add(lineNormal.Clone().Scale(ball.radius-distanceToLine+epsilon));
            else
                 ball.position.Add(lineNormal.Clone().Scale(-ball.radius-distanceToLine-epsilon));
            return true;
        }
        return false;
    }

    private void Debug()
    {
        if (_debug)
        {
            _ball.velocity = Vec2.zero;
            _ball.OnPlayer = true;
            _debug = false;
        }
    }

    const float epsilon = 0.1f;


    // Returns the point of impact, with parameters as named, where difference is the line normal scaled according to ball radius.
    public Vec2 CheckIntersection(Vec2 lineStart, Vec2 lineEnd, Vec2 ballPosition, Vec2 ballNextPosition, Vec2 difference)
    {


 
        // check which side of the line we collide with:
        Vec2 velocity = ballNextPosition.Clone().Subtract(ballPosition);
        float direction = velocity.Dot(difference); // if positive, we collide with the back of the line

        if (direction < 0)
        {
            lineStart.Add(difference);
            lineEnd.Add(difference);
            // ua is the percentage of the line where the intersection is:
            float ua = ((ballNextPosition.x - ballPosition.x) * (lineStart.y - ballPosition.y) - (ballNextPosition.y - ballPosition.y) * (lineStart.x - ballPosition.x)) / ((ballNextPosition.y - ballPosition.y) * (lineEnd.x - lineStart.x) - (ballNextPosition.x - ballPosition.x) * (lineEnd.y - lineStart.y));
            // ub is the percentage of the "velocity" until point of impact:
            float ub = ((lineEnd.x - lineStart.x) * (lineStart.y - ballPosition.y) - (lineEnd.y - lineStart.y) * (lineStart.x - ballPosition.x)) / ((ballNextPosition.y - ballPosition.y) * (lineEnd.x - lineStart.x) - (ballNextPosition.x - ballPosition.x) * (lineEnd.y - lineStart.y));
            //Console.WriteLine(ua+"||"+ub);
            Vec2 _tempIntersect = new Vec2(lineStart.x + ua * (lineEnd.x - lineStart.x), lineStart.y + ua * (lineEnd.y - lineStart.y));


            if (ub <= 1 && ub >= -epsilon && ua < 1 && ua >= 0)
            {
                //if (ub < 0)
                //    Console.WriteLine("WARNING: negative time of impact! : " + ub);
                return _tempIntersect;//.Add(addition.Normalize());
            }
        }
        else
        {
            Vec2 lineStartUnderneath = lineStart.Clone();
            Vec2 lineEndUnderneath = lineEnd.Clone();
            lineStartUnderneath.Subtract(difference);
            lineEndUnderneath.Subtract(difference);

            float ua = ((ballNextPosition.x - ballPosition.x) * (lineStartUnderneath.y - ballPosition.y) - (ballNextPosition.y - ballPosition.y) * (lineStartUnderneath.x - ballPosition.x)) / ((ballNextPosition.y - ballPosition.y) * (lineEndUnderneath.x - lineStartUnderneath.x) - (ballNextPosition.x - ballPosition.x) * (lineEndUnderneath.y - lineStartUnderneath.y));
            float ub = ((lineEndUnderneath.x - lineStartUnderneath.x) * (lineStartUnderneath.y - ballPosition.y) - (lineEndUnderneath.y - lineStartUnderneath.y) * (lineStartUnderneath.x - ballPosition.x)) / ((ballNextPosition.y - ballPosition.y) * (lineEndUnderneath.x - lineStartUnderneath.x) - (ballNextPosition.x - ballPosition.x) * (lineEndUnderneath.y - lineStartUnderneath.y));
            //Console.WriteLine(ua+"||"+ub);
            Vec2 _tempIntersect = new Vec2(lineStartUnderneath.x + ua * (lineEndUnderneath.x - lineStartUnderneath.x), lineStartUnderneath.y + ua * (lineEndUnderneath.y - lineStartUnderneath.y));
            if (ub <= 1 && ub >= -epsilon && ua < 1 && ua >= 0)
                return _tempIntersect;//.Subtract(addition.Normalize());
            //else return Vec2.zero;
        }
        return Vec2.zero;
    }

    void ActualBounce(Ball ball, LineSegment line, bool stick)
    {
        //_ballToLineStart = _ball.position.Clone().Subtract(line.start);
        //_distance = Mathf.Abs(_ballToLineStart.Dot(line.lineOnOriginNormalized.Normal().Clone()));
        _intersection = CheckIntersection(line.start.Clone(), line.end.Clone(), ball.position, ball.nextPosition, line.lineOnOriginNormalized.Normal().Scale(ball.radius-2));//try on border
        float _distanceToStart = line.start.DistanceTo(ball.nextPosition);
        float _distanceToEnd = line.end.DistanceTo(ball.nextPosition);
        //Console.WriteLine(_intersection);
        if (_intersection.y != 0)
        {
            if (stick)
            {
                ball.velocity = Vec2.zero;
                ball.StartedTimer = true;
                ball.OnPlayer = true;
            }
            else
            {
                // _sounds.PlayBallBounce();
                ball.position = _intersection;
                ball.UpdateNextPosition();
                //ball.velocity = Vec2.zero;
                ball.velocity.Reflect(line.lineOnOriginNormalized, ELASTICITY);
                ball.velocity.Scale(COLLISION_FRICTION);
                ball.UpdateInfo();
                ball.Step();
            }
        }
        //else
        //{
        //    if (line.start.y == 200) Console.WriteLine(ball.position); //here
        //}
        else {
            Vec2[] caps = new Vec2[] { line.start, line.end };
            foreach (Vec2 cap in caps)
            {
                _distanceToStart = cap.DistanceTo(ball.nextPosition);
                if (_distanceToStart < ball.radius)
                {
                    if (stick)
                    {
                        ball.velocity = Vec2.zero;
                        ball.StartedTimer = true;
                        ball.OnPlayer = true;
                    }
                    else
                    {
                        float tempDistance = cap.DistanceTo(ball.nextPosition); ;
                        Vec2 collisionNormal = ball.nextPosition.Clone().Subtract(cap).Normalize();
                        //_stones[i].position.Add(_stoneToStone.Scale(0.5f));
                        ball.position = ball.nextPosition.Clone().Add(collisionNormal.Clone().Scale(ball.radius - tempDistance)); //.Subtract(collisionNormal.Scale(_ball.radius - tempDistance / 2));
                                                                                                                   //_sounds.PlayBallBounce();
                                                                                                                   //ball.position.Subtract(ball.velocity.Clone().Normalize().Scale(ball.radius));
                        ball.UpdateNextPosition();
                        ball.velocity.ReflectOnPoint(collisionNormal, ELASTICITY);
                        Console.WriteLine(ball.velocity.Length()+"||"+collisionNormal.Length());
                        //ball.velocity = Vec2.zero;
                        //ball.velocity.ReflectOnPoint(ball.position.Clone().Subtract(line.start).Normalize(),ELASTICITY);//line.start, ball.position, ELASTICITY);
                        //ball.Step();
                    }
                    break;
                }
            }
        }/*
        else if (_distanceToEnd < ball.radius)
        {
            if (stick)
            {
                ball.velocity = Vec2.zero;
                ball.StartedTimer = true;
                ball.OnPlayer = true;
            }
            else
            {
                float _tempdistance = line.end.DistanceTo(_ball.nextPosition); ;
                Vec2 _stoneToStone = line.end.Clone().Subtract(_ball.nextPosition).Normalize();
                //_stones[i].position.Add(_stoneToStone.Scale(0.5f));
                ball.position.Subtract(_stoneToStone.Scale(_ball.radius - _tempdistance / 2));
                //_sounds.PlayBallBounce();
               // ball.position.Subtract(ball.velocity.Clone().Normalize().Scale(ball.radius));
                ball.Step();
                ball.velocity.ReflectOnPoint(ball.position.Clone().Subtract(line.end).Normalize(), ELASTICITY);
                ball.Step();
            }
        }*/
        
    }
}

