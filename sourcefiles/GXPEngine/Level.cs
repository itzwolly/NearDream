using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GXPEngine;

//673,469 position where ball was on verge

public class Level:GameObject
{
    private List<GameTile> _destroyables;
    private const int SPEED = 10;
    private const int GRAVITY = 15;
    private int REPETITIONS=2;
    private const float ELASTICITY = 0.7f;
    private Vec2 _gravity = new Vec2(0, 1);

    private Ball _reticle;
    private const float BLASTSIZE=100;
    private const int WAITFORBOOM = 180;
    private float _yOffset;
    private float _xOffset;
    private int _explosionWait;

    private const string ASSET_FILE_PATH = "assets\\";

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

        _destroyables = new List<GameTile>();
        _colidables = new List<GameTile>();
        _lines = new List<LineSegment>();

        _startingBallVelocity = SPEED / 2;

        _stones = new List<Stone>();
        collision = new CollidedOption();

        CreateLevel();
        CreateStones();
        CreatePlayer();
        CreateBall();
        CreateForegroundTrees();
    }

    private void CreatePlayer() {
        _player = new Player(200, game.height / 2);
        AddChildAt(_player, 30);

        _player = new Player(300,300);
        AddChild(_player);

        _reticle = new Ball(7, new Vec2(game.width / 2, game.height / 2), null, Color.Green);
        AddChild(_reticle);
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
        Stone _stone = new Stone(25, new Vec2(400,990 ), null, Color.Blue, false);
        AddChild(_stone);
        _stones.Add(_stone);
        _stone.velocity = Vec2.zero;

        _stone = new Stone(25, new Vec2(300,990), null, Color.Blue, false);
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
            _destroyables.Add(_tile);
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

    public void Update()
    {
        _xOffset = game.x - this.x;
        _yOffset = game.y - this.y;

        _reticle.x = Input.mouseX + _xOffset;
        _reticle.y = Input.mouseY + _yOffset;
        PlayerCamera();
        //Console.WriteLine(_ball.velocity.Length());
        
        if (Input.GetKeyDown(Key.SPACE))
        {
            _debug = true;
            _player.position.y--;
            _player.velocity.y = -GRAVITY;
        }

        if (Input.GetKeyDown(Key.E))
        {
            _ball.IsExploding = !_ball.IsExploding;
        }

        if (Input.GetMouseButton(0) && _ball.OnPlayer)
        {
            _startingBallVelocity+=0.5f;
        }
        else if (Input.GetMouseButtonUp(0) && _ball.OnPlayer)
        {
            _ball.position.x = _player.x;
            _ball.position.y = _player.y;
            _ball.velocity.x = (Input.mouseX - _player.x + _xOffset);
            _ball.velocity.y = (Input.mouseY - _player.y + _yOffset);
            if (_startingBallVelocity > 30*REPETITIONS)
                _startingBallVelocity = 30*REPETITIONS;
            _ball.velocity.Normalize().Scale(_startingBallVelocity/2);
            _ball.OnPlayer = false;
            _startingBallVelocity = SPEED;
        }
        else if(!_ball.OnPlayer)
        {
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
    }
    
        

    

private void BallBoom()
{
    if (_ball.StartedTimer)
    {
        if (_explosionWait == WAITFORBOOM)
        {
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
            if (_player.HitTest(trophy)) {
                if (!trophy.IsDestroyed()) {
                    _player.AmountOfTrophies++;
                }
                trophy.Destroy();
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
                stone.velocity = new Vec2(1, 0).Scale(_ball.velocity.Length());
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
                CheckAllLines(stone);
                stone.Step();
            }
            for (int j=0;j<_stones.Count;j++)
            {
                Stone stone2 = _stones[j];
                float _tempDistance = stone.position.DistanceTo(stone2.position);
                if (j!=i &&  _tempDistance < stone.radius + stone.radius)
                {
                    //stone.position.x - ();
                    //stone.position.y - ();
                    stone2.active = true;
                    if (!stone2.started)
                    {
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
        if (Input.GetKey(Key.D))
            _player.position.x += SPEED / 2;
        if (Input.GetKey(Key.A))
            _player.position.x -= SPEED / 2;
        if (Input.GetKeyDown(Key.R))
        {
            ResetBall();   
        }

        CheckPlayerCollision(_player, ref collision);

        if (collision.dir != direction.none)
        {
            if (collision.dir == direction.above)
            {
                _player.position.y = collision.obj.y - collision.obj.height / 2 - _player.height / 2;
                _player.velocity = Vec2.zero;
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

        //foreach (Unmovable unmovable in _colidables)
        //{
        //    _distanceX = unmovable.width / 2 + pSprite.width / 2;
        //    _distanceY = unmovable.height / 2 + pSprite.height / 2;

        //    if (pSprite.x + _distanceX >= unmovable.x &&
        //        pSprite.x - _distanceX <= unmovable.x &&
        //        pSprite.y + _distanceY >= unmovable.y &&
        //        pSprite.y - _distanceY <= unmovable.y)
        //    {
        //        if (pSprite.y - _distanceY-10 < unmovable.y)
        //        {
        //            co.dir = direction.above;
        //            co.obj = unmovable;
        //            Console.WriteLine("above");
        //            return;
        //        }
        //        else if (pSprite.y + _distanceY +10 > unmovable.y)
        //        {
        //            co.dir = direction.below;
        //            co.obj = unmovable;
        //            Console.WriteLine("below");
        //            return;
        //        }
        //        if (pSprite.x - _distanceX - 10 < unmovable.x)
        //        {
        //            co.dir = direction.left;
        //            co.obj = unmovable;
        //            Console.WriteLine("left");
        //            return;
        //        }
        //        if (pSprite.x + _distanceX + 10 > unmovable.x)
        //        {
        //            co.dir = direction.right;
        //            co.obj = unmovable;
        //            Console.WriteLine("right");
        //            return;
        //        }
        //    }
        //}

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

    public Vec2 CheckIntersection(Vec2 v1, Vec2 v2, Vec2 v3, Vec2 v4, Vec2 addition)
    {
        Vec2 v1Back = v1.Clone();
        Vec2 v2Back = v2.Clone();
        v1.Add(addition);
        v2.Add(addition);
        v1Back.Subtract(addition);
        v2Back.Subtract(addition);
        float ua = ((v4.x - v3.x) * (v1.y - v3.y) - (v4.y - v3.y) * (v1.x - v3.x)) / ((v4.y - v3.y) * (v2.x - v1.x) - (v4.x - v3.x) * (v2.y - v1.y));
        float ub = ((v2.x - v1.x) * (v1.y - v3.y) - (v2.y - v1.y) * (v1.x - v3.x)) / ((v4.y - v3.y) * (v2.x - v1.x) - (v4.x - v3.x) * (v2.y - v1.y));
        //Console.WriteLine(ua+"||"+ub);
        Vec2 _tempIntersect = new Vec2(v1.x + ua * (v2.x - v1.x), v1.y + ua * (v2.y - v1.y));
        if (Mathf.Abs(ub) < 1 && Inside(v1, v2, _tempIntersect))
            return _tempIntersect;//.Add(addition.Normalize());
        else
        {
            ua = ((v4.x - v3.x) * (v1Back.y - v3.y) - (v4.y - v3.y) * (v1Back.x - v3.x)) / ((v4.y - v3.y) * (v2Back.x - v1Back.x) - (v4.x - v3.x) * (v2Back.y - v1Back.y));
            ub = ((v2Back.x - v1Back.x) * (v1Back.y - v3.y) - (v2Back.y - v1Back.y) * (v1Back.x - v3.x)) / ((v4.y - v3.y) * (v2Back.x - v1Back.x) - (v4.x - v3.x) * (v2Back.y - v1Back.y));
            //Console.WriteLine(ua+"||"+ub);
            _tempIntersect = new Vec2(v1Back.x + ua * (v2Back.x - v1Back.x), v1Back.y + ua * (v2Back.y - v1Back.y));
            if (Mathf.Abs(ub) < 1 && Inside(v1Back, v2Back, _tempIntersect))
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

        //Console.WriteLine(_distanceToStart + "||" + _distanceToEnd + "||" + ball.radius+ "||"+ball.velocity.Length());
        if (_intersection.y != 0)
        {
            if (stick)
            {
                ball.velocity = Vec2.zero;
                ball.StartedTimer = true;
                ball.OnPlayer = true;
            }
            else {
                ball.position = _intersection;
                ball.UpdateNextPosition();
                //ball.velocity = Vec2.zero;
                ball.velocity.Reflect(line.lineOnOriginNormalized, ELASTICITY);
                ball.UptadeInfo();
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
                ball.position.Subtract(ball.velocity.Clone().Normalize().Scale(ball.radius));
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
                ball.position.Subtract(ball.velocity.Clone().Normalize().Scale(ball.radius));
                ball.velocity.ReflectOnPoint(line.end, ball.position, ELASTICITY);
                ball.Step();
            }
        }

    }
}

