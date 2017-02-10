using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GXPEngine;


public class Level:GameObject
{
    private Sounds _sounds;
    const int SPEED = 10;
    const int GRAVITY = 15;
    const int REPETITIONS=2;
    const float ELASTICITY = 0.7f;
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

    private bool _debug;

    public enum direction
    {
        none, middle, left, right,below,above
    }

    public struct CollidedOption
    {
        public direction dir;
        public Sprite obj;
    }

    private List<GameTile> _colidables;
    private List<LineSegment> _lines;
    private List<Stone> _stones;
    private List<Trophy> _trophies = new List<Trophy>();
    private List<Rope> _ropes = new List<Rope>();
    private List<Bridge> _bridges = new List<Bridge>();
    private List<Pot> _pots = new List<Pot>();
    private List<Plank> _planks = new List<Plank>();

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

    public int CurrentLevel
    {
        get { return _currentLevel; }
        set { _currentLevel = value; }
    }

    public Level(MyGame pMyGame, int pCurrentLevel)
    {
        _currentLevel = pCurrentLevel;
        _map = _tmxParser.ParseFile(ASSET_FILE_PATH + "level_" + _currentLevel + ".tmx");
        _sounds = new Sounds();
        _destroyables = new List<Plank>();
        _colidables = new List<GameTile>();
        _lines = new List<LineSegment>();
        _sounds.PlayMusic();
        _startingBallVelocity = SPEED / 2;

        _stones = new List<Stone>();
        collision = new CollidedOption();

        CreateLevel();
        CreateStones();
        CreatePlayer();
        CreateBall();
        CreateTiledObjects();
        CreateReticle();
    }

    public void Update()
    {
        _xOffset = game.x - this.x;
        _yOffset = game.y - this.y;

        _reticle.x = Input.mouseX + _xOffset;
        _reticle.y = Input.mouseY + _yOffset;
        PlayerCamera();

        HandleBall();
        HandlePlayer();
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
                }
            }
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
        Stone _stone = new Stone(25, new Vec2(2400,600 ), null, Color.Blue, false);
        AddChild(_stone);
        _stones.Add(_stone);
        _stone.velocity = Vec2.zero;

        //_stone = new Stone(25, new Vec2(450,800), null, Color.Blue, false);
        //AddChild(_stone);
        //_stones.Add(_stone);
        //_stone.velocity = Vec2.zero;

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
                    CreateTile(row, col, tile);
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
                    Trophy trophy = new Trophy(ASSET_FILE_PATH + "sprites\\trophy_animation_test.png", 7, 7);
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

    private void CreateTile(int pRow, int pCol, uint pTile)
    {
        // It gets the first tileset in order to create the level.
        // so the designer has to make sure its the first one,
        // because otherwise every tileset will be created.

        // Unbreakable Wall
        if (pTile == 1) { // assets\\sprites\sprites/circle.png cannot be 
            _tile = new Unmovable(this, ASSET_FILE_PATH + _map.TileSet[0].Image.Source, pTile, 1, 1);
            _tile.x = (pCol * _map.TileWidth) + (_tile.width / 2);
            _tile.y = (pRow * _map.TileHeight) + (_tile.height / 2);
            _colidables.Add(_tile);
            AddChild(_tile);
        }
        if(_tile!=null)
        _tile.position = new Vec2(_tile.x, _tile.y);
    }

    private void PlayerCamera()
    {
        x = game.width / 2 - _player.x;
        y = game.height / 1.25f - _player.y;
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
    }

    private void CreateIndicator()
    {
        _sounds.PlayCharge();
        _indicator = new Indicator();
        AddChild(_indicator);
    }

    private void HandleIndicator(int pPower)
    {
        _indicator.x = _player.x;
        _indicator.y = _player.y;
        _indicator.SetPower(pPower);
    }

    private void RemoveIndicator()
    {
        _goingUp = true;
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

            
            HandleIndicator((int)_startingBallVelocity/4);
        }
        else if (Input.GetMouseButtonUp(0) && _ball.OnPlayer)
        {
            _ball.position.x = _player.x;
            _ball.position.y = _player.y;
            _ball.velocity.x = (Input.mouseX - _player.x + _xOffset);
            _ball.velocity.y = (Input.mouseY - _player.y + _yOffset);
            _ball.velocity.Normalize().Scale(_startingBallVelocity);
            _ball.OnPlayer = false;
            _startingBallVelocity = SPEED;
            RemoveIndicator();
            _sounds.StopCharge();
            _sounds.PlayShoot();
        }
        else if (!_ball.OnPlayer)
        {
            _ball.velocity.Add(_gravity);
            for (int i = 0; i <= REPETITIONS; i++)
            {
                _ball.Step();
                CheckAllLines(_ball);
            }
        }
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
                        new Timer(1000, pot.Canvas.Destroy);
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
            Stone stone = _stones[i];
           
            if (stone.position.DistanceTo(_ball.position) < stone.radius + _ball.radius && !stone.hitPlayer)
            {
                _sounds.PlayBallRockCollision();
                stone.velocity = _ball.velocity.Clone();//new Vec2(1, 0).Scale(_ball.velocity.Length());
                stone.Step();
                _ball.velocity = Vec2.zero;
                _ball.velocity.ReflectOnPoint(stone.position,_ball.position,1);

                
                _ball.Step();
                //CollisionFix2Balls(stone, _ball);.Scale
                stone.active = true;
                //stone.hitPlayer = true;
            }
            if (stone.active)
            {

                stone.velocity.Add(_gravity);
                for (int j = 0; j < REPETITIONS; j++)
                {
                    CheckAllLines(stone);
                    stone.Step();
                    //_sounds.PlayRockBounce();
                }
            }
            for (int j=0;j<_stones.Count;j++)
            {
                Stone stone2 = _stones[j];
                float _tempDistance = stone.position.DistanceTo(stone2.position);
                if (j!=i &&  _tempDistance < stone.radius + stone2.radius)
                {
                    //stone.position.x - ();
                    //stone.position.y - ();
                    stone2.active = true;
                    if (!stone2.started)
                    {
                        _sounds.PlayRockBounce();
                        stone2.velocity = new Vec2(1, 0).Scale(stone.velocity.Length());
                        stone2.started = true;
                    }
                    stone.hitPlayer = false;
                    stone.velocity.Scale(0.0f);
                    stone.Step();
                    stone2.Step();
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
        if (Input.GetKeyDown(Key.SPACE))
        {
            _sounds.PlayJump();
            _debug = true;
            _player.position.y--;
            _player.velocity.y = -GRAVITY;
        }
        if (Input.GetKey(Key.D))
        {
            _player.position.x += SPEED / 2;
        }
        if (Input.GetKey(Key.A))
        {
            _player.position.x -= SPEED / 2;
        }
        if (Input.GetKeyDown(Key.R))
        {
            ResetBall();   
        }

        CheckPlayerCollision(_player, ref collision);

        if (collision.dir != direction.none)
        {
            if (collision.dir == direction.above)
            {
                //_sounds.PlayWalk();
                if (collision.obj is Bridge)
                {
                    _player.position.y = collision.obj.y - collision.obj.height / 2 - _player.height / 2;
                    _player.velocity = Vec2.zero;
                }
                else
                {
                    //Console.WriteLine(collision.obj);
                    _player.position.y = collision.obj.y - collision.obj.height / 2 - _player.height / 2;
                    _player.velocity = Vec2.zero;
                }
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

        _player.Step();
    }

    public void CheckPlayerCollision(Player pPlayer, ref CollidedOption co)
    {
        co.dir = direction.none;
        co.obj = null;

        float _distanceX,_distanceY;

        for (int obj = 0; obj < _colidables.Count; obj++)//goes through all the walls in the list
        {
            Sprite wall = _colidables[obj];//selects one of the walls
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
        for (int obj = 0; obj < _colidables.Count; obj++)
        {
            Sprite wall = _colidables[obj];
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


        
        for (int obj = 0; obj < _bridges.Count; obj++)
        {
            if (_bridges[obj].Down)
            {
                Sprite wall = _bridges[obj];
                _distanceX = wall.width / 2 + pPlayer.width / 2;
                _distanceY = wall.height / 2 + pPlayer.height / 2;
                if (pPlayer.position.x + _distanceX >= wall.x &&
                    pPlayer.position.x - _distanceX <= wall.x &&
                    pPlayer.position.y + _distanceY >= wall.y &&
                    pPlayer.position.y - _distanceY <= wall.y)
                {
                    if (pPlayer.position.y < wall.y)
                    {
                        co.obj = wall;
                        co.dir = direction.above;
                        //Console.WriteLine("above");
                        return;
                    }
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

    const float epsilon = 0.8f;

    public Vec2 CheckIntersection(Vec2 lineStart, Vec2 lineEnd, Vec2 ballPosition, Vec2 ballNextPosition, Vec2 difference)
    {
        Vec2 lineStartUnderneath = lineStart.Clone();
        Vec2 lineEndUnderneath = lineEnd.Clone();
        lineStart.Add(difference);
        lineEnd.Add(difference);
        lineStartUnderneath.Subtract(difference);
        lineEndUnderneath.Subtract(difference);
        float ua = ((ballNextPosition.x - ballPosition.x) * (lineStart.y - ballPosition.y) - (ballNextPosition.y - ballPosition.y) * (lineStart.x - ballPosition.x)) / ((ballNextPosition.y - ballPosition.y) * (lineEnd.x - lineStart.x) - (ballNextPosition.x - ballPosition.x) * (lineEnd.y - lineStart.y));
        float ub = ((lineEnd.x - lineStart.x) * (lineStart.y - ballPosition.y) - (lineEnd.y - lineStart.y) * (lineStart.x - ballPosition.x)) / ((ballNextPosition.y - ballPosition.y) * (lineEnd.x - lineStart.x) - (ballNextPosition.x - ballPosition.x) * (lineEnd.y - lineStart.y));
        //Console.WriteLine(ua+"||"+ub);
        Vec2 _tempIntersect = new Vec2(lineStart.x + ua * (lineEnd.x - lineStart.x), lineStart.y + ua * (lineEnd.y - lineStart.y));
        if (ub <= 1 && ub >= -epsilon && ua < 1 && ua >= 0)
            return _tempIntersect;//.Add(addition.Normalize());
        else
        {
            ua = ((ballNextPosition.x - ballPosition.x) * (lineStartUnderneath.y - ballPosition.y) - (ballNextPosition.y - ballPosition.y) * (lineStartUnderneath.x - ballPosition.x)) / ((ballNextPosition.y - ballPosition.y) * (lineEndUnderneath.x - lineStartUnderneath.x) - (ballNextPosition.x - ballPosition.x) * (lineEndUnderneath.y - lineStartUnderneath.y));
            ub = ((lineEndUnderneath.x - lineStartUnderneath.x) * (lineStartUnderneath.y - ballPosition.y) - (lineEndUnderneath.y - lineStartUnderneath.y) * (lineStartUnderneath.x - ballPosition.x)) / ((ballNextPosition.y - ballPosition.y) * (lineEndUnderneath.x - lineStartUnderneath.x) - (ballNextPosition.x - ballPosition.x) * (lineEndUnderneath.y - lineStartUnderneath.y));
            //Console.WriteLine(ua+"||"+ub);
            _tempIntersect = new Vec2(lineStartUnderneath.x + ua * (lineEndUnderneath.x - lineStartUnderneath.x), lineStartUnderneath.y + ua * (lineEndUnderneath.y - lineStartUnderneath.y));
            if (ub <= 1 && ub >= -epsilon && ua < 1 && ua >= 0)
                return _tempIntersect;//.Subtract(addition.Normalize());
            else return Vec2.zero;
        }
    }

    void ActualBounce(Ball ball, LineSegment line, bool stick)
    {
        _ballToLineStart = _ball.position.Clone().Subtract(line.start);
        _distance = Mathf.Abs(_ballToLineStart.Dot(line.lineOnOriginNormalized.Normal().Clone()));
        _intersection = CheckIntersection(line.start.Clone(), line.end.Clone(), ball.position, ball.nextPosition, line.lineOnOriginNormalized.Normal().Scale(ball.radius-2));//try on border
        float _distanceToStart = line.start.DistanceTo(ball.position);
        float _distanceToEnd = line.end.DistanceTo(ball.position);
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
                ball.UpdateInfo();
                ball.Step();
            }
        }
        //else
        //{
        //    if (line.start.y == 200) Console.WriteLine(ball.position); //here
        //}
        else if (_distanceToStart < ball.radius)
        {
            if (stick)
            {
                ball.velocity = Vec2.zero;
                ball.StartedTimer = true;
                ball.OnPlayer = true;
            }
            else
            {
                //_sounds.PlayBallBounce();
                ball.position.Subtract(ball.velocity.Clone().Normalize().Scale(ball.radius));
                ball.Step();
                ball.velocity.ReflectOnPoint(line.start, ball.position, ELASTICITY);
                ball.Step();
            }
        }
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
                //_sounds.PlayBallBounce();
                ball.position.Subtract(ball.velocity.Clone().Normalize().Scale(ball.radius));
                ball.Step();
                ball.velocity.ReflectOnPoint(line.end, ball.position, ELASTICITY);
                ball.Step();
            }
        }
    }
}

