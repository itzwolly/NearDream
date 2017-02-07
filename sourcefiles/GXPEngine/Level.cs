using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GXPEngine;


class Level:GameObject
{
    const int SPEED = 10;
    const int GRAVITY = 15;
    int REPETITIONS=1;
    const float ELASTICITY = 0.9f;
    private Vec2 _gravity = new Vec2(0, 1);
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

    private List<Unmovable> _colidables;
    private List<LineSegment> _lines;
    private List<Stone> _stones;
    private Player _player;
    private float _startingBallVelocity;

    private CollidedOption collision;

    private Ball _ball;
    private Vec2 _ballToLineStart;
    private Vec2 _intersection;
    private LineSegment _ballToLine;
    private LineSegment _line;
    private float _distance;


    public Level()
    {

        _colidables = new List<Unmovable>();
        _lines = new List<LineSegment>();
        _stones = new List<Stone>();
        CreateLevel();
        _player = new Player(300,300);
        AddChild(_player);
        _startingBallVelocity = SPEED / 2;


        collision = new CollidedOption();
    }

    private void CreateLevel()
    {
        _ball = new Ball(25, new Vec2(673, 469 /*game.width / 2, game.height / 2*/), null, Color.Red);
        AddChild(_ball);
        _ball.velocity = Vec2.zero;
        _ballToLine = new LineSegment(null, null);
        AddChild(_ballToLine);

        Stone _stone = new Stone(25, new Vec2(game.width / 2, game.height / 9), null, Color.Blue, false);
        AddChild(_stone);
        _stones.Add(_stone);
        _stone.velocity = Vec2.zero;

        _stone = new Stone(25, new Vec2(game.width / 2+100, game.height / 9), null, Color.Blue, false);
        AddChild(_stone);
        _stones.Add(_stone);
        _stone.velocity = Vec2.zero;

        _line = new NLineSegment(new Vec2(300, 100), new Vec2(700, 100), 0xffffff00, 4);
        AddChild(_line);
        _lines.Add(_line);

        _line = new NLineSegment(new Vec2(200, 400), new Vec2(200, 0), 0xffffff00, 4);
        AddChild(_line);
        _lines.Add(_line);

        //_ball.velocity = new Vec2(1, 1);
        //_ball.UpdateNextPosition();

        _line = new NLineSegment(new Vec2(700, 200), new Vec2(700, 500), 0xffffff00, 4);
        AddChild(_line);
        _lines.Add(_line);

        //_intersection = CheckIntersection(_line.start.Clone(), _line.end.Clone(), _ball.position, _ball.nextPosition, _line.lineOnOriginNormalized.Normal().Scale(_ball.radius+_line.lineWidth/2));//try on border
        //ActualBounce(_ball, _line);
        //Console.WriteLine(_intersection +"||"+_distance);

        //  Destroy();
        //  return;


        _line = new NLineSegment(new Vec2(0, 0), new Vec2(game.width, 0), 0xffffffff, 4);
        AddChild(_line);
        _lines.Add(_line);

        _line = new NLineSegment(new Vec2(200, 400), new Vec2(700, 500), 0xffffffff, 4);
        AddChild(_line);
        _lines.Add(_line);

        _line = new NLineSegment(new Vec2(800, 0), new Vec2(800, 500), 0xffffff00, 4);
        AddChild(_line);
        _lines.Add(_line);

        _line = new NLineSegment(new Vec2(700, 500), new Vec2(800, 500), 0xffffff00, 4);
        AddChild(_line);
        _lines.Add(_line);

        Unmovable wall = new Unmovable(400, 400);
        AddChild(wall);
        _colidables.Add(wall);

        wall = new Unmovable(336, 436);
        AddChild(wall);
        _colidables.Add(wall);
        
    }

    public void Update()
    {
        //Console.WriteLine(_ball.velocity.Length());
        if (Input.GetKeyDown(Key.SPACE))
        {
            _debug = true;
            _player.position.y--;
            _player.velocity.y = -GRAVITY;
        }
        if (Input.GetMouseButton(0) && _ball.OnPlayer)
        {
            _startingBallVelocity+=0.3f;
        }
        else if (Input.GetMouseButtonUp(0) && _ball.OnPlayer)
        {
            _ball.position.x = _player.x;
            _ball.position.y = _player.y;
            _ball.velocity.x = (Input.mouseX - _player.x);
            _ball.velocity.y = (Input.mouseY - _player.y);
            if (_startingBallVelocity > 20)
                _startingBallVelocity = 20;
            _ball.velocity.Normalize().Scale(_startingBallVelocity);
            _ball.OnPlayer = false;
            _startingBallVelocity = SPEED / 2;
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
       
    }

    private void CheckStones()
    {
        for (int i=0;i<_stones.Count;i++)
        {
            Stone stone = _stones[i];
            
            if (stone.active)
            {
                CheckAllLines(stone);
                stone.velocity.Add(_gravity);
                stone.Step();
            }
            if (stone.position.DistanceTo(_ball.position) < stone.radius + _ball.radius && !stone.hitPlayer)
            {
                stone.velocity = new Vec2(1, 0).Scale(_ball.velocity.Length());
                //stone.Step();
                _ball.velocity = Vec2.zero;
                _ball.velocity.ReflectOnPoint(stone.position,_ball.position,1);
                _ball.Step();
                //CollisionFix2Balls(stone, _ball);.Scale
                stone.active = true;
                stone.hitPlayer = true;
            }

            for(int j=0;j<_stones.Count;j++)
            {
                Stone stone2 = _stones[j];
                if(j!=i && stone.position.DistanceTo(stone2.position) < stone.radius + stone.radius)
                {
                    stone2.active = true;
                    stone2.velocity = new Vec2(1, 0).Scale(stone.velocity.Length());
                    stone.velocity.Scale(0.0f);
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
            ActualBounce(ball, _lines[i]);
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

    void ActualBounce(Ball ball, LineSegment line)
    {
        _ballToLineStart = _ball.position.Clone().Subtract(line.start);
        _distance = Mathf.Abs(_ballToLineStart.Dot(line.lineOnOriginNormalized.Normal().Clone()));
        _intersection = CheckIntersection(line.start.Clone(), line.end.Clone(), ball.position, ball.nextPosition, line.lineOnOriginNormalized.Normal().Scale(ball.radius-2));//try on border
        float _distanceToStart = line.start.DistanceTo(ball.position);
        float _distanceToEnd = line.end.DistanceTo(ball.position);

        //Console.WriteLine(_distanceToStart + "||" + _distanceToEnd + "||" + ball.radius+ "||"+ball.velocity.Length());
        if (_intersection.y != 0)
        {
            ball.position = _intersection;
            ball.UpdateNextPosition();
            //ball.velocity = Vec2.zero;
            ball.velocity.Reflect(line.lineOnOriginNormalized, ELASTICITY);
            ball.UptadeInfo();
            ball.Step();
        }
        //else
        //{
        //    if (line.start.y == 200) Console.WriteLine(ball.position); //here
        //}
        else if (_distanceToStart < ball.radius)
        {
            ball.position.Subtract(ball.velocity.Clone().Normalize().Scale(ball.radius - _distanceToStart));
            ball.velocity.ReflectOnPoint(line.start, ball.position, ELASTICITY);
            ball.Step();
        }
        else if (_distanceToEnd < ball.radius)
        {
            ball.position.Subtract(ball.velocity.Clone().Normalize().Scale(ball.radius - _distanceToEnd));
            ball.velocity.ReflectOnPoint(line.end, ball.position, ELASTICITY);
            ball.Step();
        }

    }
}

