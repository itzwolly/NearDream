using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GXPEngine;


class Level:GameObject
{
    const int SPEED = 10;
    const int DIVIDER = 20;
    private Vec2 _gravity = new Vec2(0, 1);
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
    private Player _player;

    private CollidedOption collision;

    private Ball _ball;
    private Vec2 _ballToLineStart;
    private Vec2 _intersection;
    private LineSegment _ballToLine;
    private float _distance;


    public Level()
    {

        _colidables = new List<Unmovable>();

        _player = new Player(300,300);
        AddChild(_player);

        _ball = new Ball(25, new Vec2(game.width / 2, game.height / 2), null, Color.Red);
        AddChild(_ball);
        _ball.velocity = Vec2.zero;
        _ballToLine = new LineSegment(null, null);
        AddChild(_ballToLine);


        Unmovable wall = new Unmovable(400,300);
        AddChild(wall);
        _colidables.Add(wall);

        wall = new Unmovable(336, 336);
        AddChild(wall);
        _colidables.Add(wall);
        collision = new CollidedOption();
    }

    public void Update()
    {
        if (Input.GetKey(Key.D))
            _player.position.x += 10;
        if (Input.GetKey(Key.A))
            _player.position.x -= 10;
        if (Input.GetKeyDown(Key.SPACE))
        {
            
        }
        if (Input.GetMouseButton(0))
        {
            _ball.position.x = _player.x;
            _ball.position.y = _player.y;
            _ball.velocity.x = (Input.mouseX - _player.x)/DIVIDER;
            _ball.velocity.y = (Input.mouseY - _player.y)/DIVIDER;
        }
        //if (Input.GetKey(Key.S))
        //    _player.y += 10;
        //if (Input.GetKey(Key.W))
        //    _player.y -= 10;


        _ball.Step();

        CheckPlayerCollision(_player, ref collision);

        if (collision.dir != direction.none)
        {
            if (collision.dir == direction.above)
                _player.position.y = collision.obj.y - collision.obj.height / 2 - _player.height / 2;
            if (collision.dir == direction.below)
                _player.position.y = collision.obj.y + collision.obj.height / 2 + _player.height / 2;

            if (collision.dir == direction.right)
                _player.position.x = collision.obj.x + collision.obj.width / 2 + _player.width / 2 + 1;
            if (collision.dir == direction.left)
                _player.position.x = collision.obj.x - collision.obj.width / 2 - _player.width / 2 - 1;
        }
        else _player.position.y += _gravity.y*SPEED;

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

                if (pPlayer.position.x > wall.x + wall.width - 20)// sees if who is on the right of enemy
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

    public Vec2 CheckIntersection(Vec2 v1, Vec2 v2, Vec2 v3, Vec2 v4, Vec2 addition)
    {
        Vec2 v1Back = v1.Clone();
        Vec2 v2Back = v2.Clone();
        v1.Add(addition);
        v2.Add(addition);
        float ua = ((v4.x - v3.x) * (v1.y - v3.y) - (v4.y - v3.y) * (v1.x - v3.x)) / ((v4.y - v3.y) * (v2.x - v1.x) - (v4.x - v3.x) * (v2.y - v1.y));
        float ub = ((v2.x - v1.x) * (v1.y - v3.y) - (v2.y - v1.y) * (v1.x - v3.x)) / ((v4.y - v3.y) * (v2.x - v1.x) - (v4.x - v3.x) * (v2.y - v1.y));
        //Console.WriteLine(ua+"||"+ub);
        Vec2 _tempIntersect = new Vec2(v1.x + ua * (v2.x - v1.x), v1.y + ua * (v2.y - v1.y));
        if (Mathf.Abs(ub) < 1 && Inside(v1, v2, _tempIntersect))
            return _tempIntersect;
        else
        {
            v1Back.Subtract(addition);
            v2Back.Subtract(addition);
            ua = ((v4.x - v3.x) * (v1Back.y - v3.y) - (v4.y - v3.y) * (v1Back.x - v3.x)) / ((v4.y - v3.y) * (v2Back.x - v1Back.x) - (v4.x - v3.x) * (v2Back.y - v1Back.y));
            ub = ((v2Back.x - v1Back.x) * (v1Back.y - v3.y) - (v2Back.y - v1Back.y) * (v1Back.x - v3.x)) / ((v4.y - v3.y) * (v2Back.x - v1Back.x) - (v4.x - v3.x) * (v2Back.y - v1Back.y));
            //Console.WriteLine(ua+"||"+ub);
            _tempIntersect = new Vec2(v1Back.x + ua * (v2Back.x - v1Back.x), v1Back.y + ua * (v2Back.y - v1Back.y));
            if (Mathf.Abs(ub) < 1 && Inside(v1Back, v2Back, _tempIntersect))
                return _tempIntersect;
            else return Vec2.zero;
        }
    }

    private bool Inside(Vec2 v1, Vec2 v2, Vec2 v3)
    {
        if (v1.x <= v3.x && v2.x >= v3.x && v1.y <= v3.y && v2.y >= v3.y)
            return true;
        if (v1.x >= v3.x && v2.x <= v3.x && v1.y >= v3.y && v2.y <= v3.y)
            return true;
        return false;
    }

    void ActualBounce(Ball ball, LineSegment line)
    {
        _ballToLineStart = _ball.position.Clone().Subtract(line.start);
        _distance = Mathf.Abs(_ballToLineStart.Dot(line.lineOnOriginNormalized.Normal().Clone()));
        _intersection = CheckIntersection(line.start.Clone(), line.end.Clone(), ball.position, ball.nextPosition, line.lineOnOriginNormalized.Normal().Scale(ball.radius));//try on border
        float _distanceToStart = line.start.DistanceTo(ball.position);
        float _distanceToEnd = line.end.DistanceTo(ball.position);


        if (_intersection.y != 0)
        {
            
            ball.position = _intersection;
            //ball.velocity = Vec2.zero;
            ball.velocity.Reflect(line.lineOnOriginNormalized, 1);
            ball.UptadeInfo();
            _ball.Step();
        }
        else if (_distanceToStart < ball.radius)
        {
            //    Console.WriteLine("start");
            //    ball.position = line.start.Clone();
            //    ball.position.Subtract(ball.velocity.Clone().Normalize().Scale(ball.radius + line.lineWidth));
            //    ball.UpdateNextPosition();
            //ball.velocity = Vec2.zero;
            ball.velocity.ReflectOnPoint(line.start, ball.position, 1);
            ball.Step();
        }
        else if (_distanceToEnd < ball.radius)
        {
            //Console.WriteLine("end");
            //ball.position = line.end.Clone();
            //ball.position.Subtract(ball.velocity.Clone().Normalize().Scale(ball.radius + line.lineWidth));
            //ball.UpdateNextPosition();
            //ball.velocity = Vec2.zero;
            ball.velocity.ReflectOnPoint(line.end, ball.position, 1);
            ball.Step();
        }
    }
}

