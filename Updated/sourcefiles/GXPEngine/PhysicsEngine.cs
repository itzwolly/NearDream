using System;
using System.Collections.Generic;
using GXPEngine;
using System.Drawing;

public class PhysicsEngine {
    public const int GRAVITY = 15;
    private const float EPSILON = 0.1f;

    private Vec2 _gravity = new Vec2(0, 1);
    private Vec2 _ballToLineStart, _intersection;
    private Level _level;
    private CollidedOption _collision;
    private Random _rnd = new Random();
    private int _explosionWait;

    private float _distance, _playerXOffset;
    private bool _goingUp;


    // Constructor
    public PhysicsEngine(Level pLevel) {
        _level = pLevel;
    }

    private void CheckAllLines(Ball ball) {
        ball.UpdateNextPosition();
        for (int i = 0; i < _level.GetLines().Count; i++) {
            ActualBounce(ball, _level.GetLines()[i], ball.IsExploding);
        }

        bool noOverlap = false;
        int iterations = 0;
        int maxIterations = 20;

        do {
            noOverlap = true;
            for (int i = 0; i < _level.GetLines().Count; i++) {
                noOverlap = noOverlap & !CorrectOverlap(ball, _level.GetLines()[i]); // ActualBounce(ball, _lines[i], ball.IsExploding);
            }
            iterations++;
        } while (!noOverlap && iterations < maxIterations);
    }

    bool CorrectOverlap(Ball ball, LineSegment line) {
        Vec2 differenceVec = ball.Position.Clone().Subtract(line.start);
        Vec2 normalizedLineVec = line.end.Clone().Subtract(line.start).Normalize();
        Vec2 lineNormal = normalizedLineVec.Normal();

        float distanceToLine = differenceVec.Dot(lineNormal);
        float distanceOnLine = differenceVec.Dot(normalizedLineVec);
        if (distanceOnLine <= line.lineLenght && distanceOnLine >= 0 && distanceToLine >= -ball.radius && distanceToLine <= ball.radius) {
            if (distanceToLine > 0)
                ball.Position.Add(lineNormal.Clone().Scale(ball.radius - distanceToLine + EPSILON));
            else
                ball.Position.Add(lineNormal.Clone().Scale(-ball.radius - distanceToLine - EPSILON));
            return true;
        }
        return false;
    }

    public void HandlePlayer() {
        if (Input.GetKey(Key.D)) {
            if (_collision.dir == CollidedOption.Direction.UP)
                _level.GetPlayer().horizontalDirection = Player.Direction.RIGHT;
            _level.GetPlayer().IsMoving = true;
            _level.GetPlayer().Position.x += Player.SPEED / 2;
            _level.SetPlayerDirection(Player.Direction.RIGHT);
        } else if (Input.GetKey(Key.A)) {
            if (_collision.dir == CollidedOption.Direction.UP)
                _level.GetPlayer().horizontalDirection = Player.Direction.LEFT;
            _level.GetPlayer().IsMoving = true;
            _level.GetPlayer().Position.x -= Player.SPEED / 2;
            _level.SetPlayerDirection(Player.Direction.LEFT);
        } else {
            _level.GetPlayer().IsMoving = false;
        }
        if(_level.GetPlayer().Position.x - _level.GetPlayer().x==0)
        {
            _level.GetPlayer().horizontalDirection = Player.Direction.NONE;
        }
        if (_level.GetPlayer().Velocity.y < 0)
        {
            _level.GetPlayer().verticalDirection = Player.Direction.UP;
        }
        else if (_level.GetPlayer().Velocity.y > 0)
        {
            _level.GetPlayer().verticalDirection = Player.Direction.DOWN;
        }
        else
        {
            _level.GetPlayer().verticalDirection = Player.Direction.NONE;
        }
        //Console.WriteLine(_level.GetPlayer().horizontalDirection+" || "+ _level.GetPlayer().verticalDirection);
        if (Input.GetKeyDown(Key.SPACE)) {
            //_sounds.PlayJump();
            if (!_level.GetPlayer().Jumped) {
                //_sounds.PlayJump();
                _level.GetPlayer().Position.y--;
                _level.GetPlayer().Velocity.y = -GRAVITY;
                _level.GetPlayer().Jumped = true;
            }
        }

        if (Input.GetKeyDown(Key.R)) {
            ResetBall();
        }

        CheckPlayerCollision(_level.GetPlayer(), ref _collision);

        if (_collision.dir != CollidedOption.Direction.NONE) {
            if (_collision.dir == CollidedOption.Direction.UP) {
                //_sounds.PlayWalk();
                _level.GetPlayer().Position.y = _collision.obj.y - _collision.obj.height / 2 - _level.GetPlayer().height / 2;
                _level.GetPlayer().Velocity = Vec2.zero;
                _level.GetPlayer().Jumped = false;
            }
            if (_collision.dir == CollidedOption.Direction.DOWN) {
                _level.GetPlayer().Position.y = _collision.obj.y + _collision.obj.height / 2 + _level.GetPlayer().height / 2;
                _level.GetPlayer().Velocity = Vec2.zero;
            }
            if (_collision.dir == CollidedOption.Direction.RIGHT) {
                _level.GetPlayer().Position.x = _collision.obj.x + _collision.obj.width / 2 + _level.GetPlayer().width / 2;
                _level.GetPlayer().Velocity = Vec2.zero;
            }
            if (_collision.dir == CollidedOption.Direction.LEFT) {
                _level.GetPlayer().Position.x = _collision.obj.x - _collision.obj.width / 2 - _level.GetPlayer().width / 2;
                _level.GetPlayer().Velocity = Vec2.zero;
            }
        } else if (_collision.obj == null) {
            _level.GetPlayer().Velocity.y += _gravity.y;
        }

        if (_level.GetPlayer().IsMoving) {
            _playerXOffset = _level.GetPlayer().Position.x - _level.GetPlayer().x;
        } else {
            _playerXOffset = 0;
        }

        _level.GetPlayer().Step();
    }

    public void CheckPlayerCollision(Player pPlayer, ref CollidedOption co) {
        co.dir = CollidedOption.Direction.NONE;
        co.obj = null;

        float _distanceX, _distanceY;

        if (_level.GetPlayer().x < +32) {
            _level.GetPlayer().Position.x = 32;
        }
        if (_level.GetPlayer().x > _level.GetMap().GetLevelWidth() - 32) {
            _level.GetPlayer().Position.x = _level.GetMap().GetLevelWidth() - 32;
        }

        for (int obj = 0; obj < _level.GetBridgeColliders().Count; obj++) {
            Sprite wall = _level.GetBridgeColliders()[obj];
            _distanceX = wall.width / 2 + pPlayer.width / 2;
            _distanceY = wall.height / 2 + pPlayer.height / 2;
            if (pPlayer.Position.x + _distanceX >= wall.x &&
                pPlayer.Position.x - _distanceX <= wall.x &&
                pPlayer.Position.y + _distanceY >= wall.y &&
                pPlayer.Position.y - _distanceY <= wall.y) {
                    if (pPlayer.Position.x < wall.x - wall.width / 2 - 20) //sees if who is on the left of the wall
                    {
                    co.obj = wall;
                    co.dir = CollidedOption.Direction.LEFT;
                    //Console.WriteLine("left");
                    return;
                    }
                    if (pPlayer.Position.x > wall.x + wall.width / 2 + 20)// sees if who is on the right of enemy5
                    {
                    co.obj = wall;
                    co.dir = CollidedOption.Direction.RIGHT;
                    //Console.WriteLine("right");
                    return;
                    }
            }
        }
        for (int obj = 0; obj < _level.GetBridgeColliders().Count; obj++) {
            Sprite wall = _level.GetBridgeColliders()[obj];
            _distanceX = wall.width / 2 + pPlayer.width / 2;
            _distanceY = wall.height / 2 + pPlayer.height / 2;
            if (pPlayer.Position.x + _distanceX >= wall.x &&
                pPlayer.Position.x - _distanceX <= wall.x &&
                pPlayer.Position.y + _distanceY >= wall.y &&
                pPlayer.Position.y - _distanceY <= wall.y) {
                if (pPlayer.Position.y < wall.y - wall.height / 2) {
                    co.obj = wall;
                    co.dir = CollidedOption.Direction.UP;
                    //Console.WriteLine("above");
                    return;
                }
                if (pPlayer.Position.y > wall.y + wall.height / 2) {
                    co.obj = wall;
                    co.dir = CollidedOption.Direction.DOWN;
                    //Console.WriteLine("below");
                    return;
                }
            }
        }

        for (int obj = 0; obj < _level.GetCollidables().Count; obj++)//goes through all the walls in the list
        {
            Sprite wall = _level.GetCollidables()[obj];//selects one of the walls
            _distanceX = wall.width / 2 + pPlayer.width / 2;//sets the horizontal distance between who and wall
            _distanceY = wall.height / 2 + pPlayer.height / 2;//sets the vertical distance between who and wall
            if (pPlayer.Position.x + _distanceX >= wall.x &&
                pPlayer.Position.x - _distanceX <= wall.x &&
                pPlayer.Position.y + _distanceY - 20 >= wall.y &&
                pPlayer.Position.y - _distanceY + 20 <= wall.y)//selects if who is inside the boundaries of the wall
            {
                if (pPlayer.Position.x < wall.x - wall.width + 20)//sees if who is on the left of the wall
                {
                    co.obj = wall;
                    co.dir = CollidedOption.Direction.LEFT;
                    //Console.WriteLine("left");
                    return;
                }

                if (pPlayer.Position.x > wall.x + wall.width - 20)// sees if who is on the right of enemy5
                {
                    co.obj = wall;
                    co.dir = CollidedOption.Direction.RIGHT;
                    //Console.WriteLine("right");
                    return;
                }
            }
        }
        for (int obj = 0; obj < _level.GetCollidables().Count; obj++) {
            Sprite wall = _level.GetCollidables()[obj];
            _distanceX = wall.width / 2 + pPlayer.width / 2;
            _distanceY = wall.height / 2 + pPlayer.height / 2;
            if (pPlayer.Position.x + _distanceX >= wall.x &&
                pPlayer.Position.x - _distanceX <= wall.x &&
                pPlayer.Position.y + _distanceY >= wall.y &&
                pPlayer.Position.y - _distanceY <= wall.y) {
                if (pPlayer.Position.y < wall.y - wall.height / 2) {
                    co.obj = wall;
                    co.dir = CollidedOption.Direction.UP;
                    //Console.WriteLine("above");
                    return;
                }

                if (pPlayer.Position.y > wall.y + wall.height / 2) {
                    co.obj = wall;
                    co.dir = CollidedOption.Direction.DOWN;
                    //Console.WriteLine("below");
                    return;
                }
            }
        }

        for (int obj = 0; obj < _level.GetDestroyables().Count; obj++)//goes through all the walls in the list
        {
            Sprite wall = _level.GetDestroyables()[obj];//selects one of the walls
            _distanceX = wall.width / 2 + pPlayer.width / 2;//sets the horizontal distance between who and wall
            _distanceY = wall.height / 2 + pPlayer.height / 2;//sets the vertical distance between who and wall
            if (pPlayer.Position.x + _distanceX >= wall.x &&
                pPlayer.Position.x - _distanceX <= wall.x &&
                pPlayer.Position.y + _distanceY - 20 >= wall.y &&
                pPlayer.Position.y - _distanceY + 20 <= wall.y)//selects if who is inside the boundaries of the wall
            {
                if (pPlayer.Position.x < wall.x - wall.width + 20)//sees if who is on the left of the wall
                {
                    co.obj = wall;
                    co.dir = CollidedOption.Direction.LEFT;
                    //Console.WriteLine("left");
                    return;
                }

                if (pPlayer.Position.x > wall.x + wall.width - 20)// sees if who is on the right of enemy5
                {
                    co.obj = wall;
                    co.dir = CollidedOption.Direction.RIGHT;
                    //Console.WriteLine("right");
                    return;
                }
            }
        }
        for (int obj = 0; obj < _level.GetDestroyables().Count; obj++) {
            Sprite wall = _level.GetDestroyables()[obj];
            _distanceX = wall.width / 2 + pPlayer.width / 2;
            _distanceY = wall.height / 2 + pPlayer.height / 2;
            if (pPlayer.Position.x + _distanceX >= wall.x &&
                pPlayer.Position.x - _distanceX <= wall.x &&
                pPlayer.Position.y + _distanceY >= wall.y &&
                pPlayer.Position.y - _distanceY <= wall.y) {
                if (pPlayer.Position.y < wall.y - wall.height / 2) {
                    co.obj = wall;
                    co.dir = CollidedOption.Direction.UP;
                    //Console.WriteLine("above");
                    return;
                }

                if (pPlayer.Position.y > wall.y + wall.height / 2) {
                    co.obj = wall;
                    co.dir = CollidedOption.Direction.DOWN;
                    //Console.WriteLine("below");
                    return;
                }
            }
        }
        return;
    }

    public void HandleBall() {
        if (Input.GetKeyDown(Key.E)) {
            //_sounds.PlaySwitch();
            _level.GetBall().IsExploding = !_level.GetBall().IsExploding;
            _level.GetHUD().ReDrawCurrentBall(_level.GetBall().IsExploding);
        }
        

        if(_level.GetBall().OnPlayer && !_level.GetBall().StartedTimer)
        {
            ResetBall();
        }

        if (Input.GetMouseButton(0) && _level.GetBall().OnPlayer) {

            _level.GetBall().chargeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee = true;

            if (_level.GetPlayer().GetIndicator() == null) {
                _level.CreateIndicator();
            }
                

            if (_level.GetBall().StartingBallVelocity > Player.MAXPOWER || _level.GetBall().StartingBallVelocity < 0) {
                _goingUp = !_goingUp;
            }

            if (_goingUp) {
                _level.GetBall().StartingBallVelocity += 0.3f;
            } else {
                _level.GetBall().StartingBallVelocity -= 0.3f;
            }

            HandleIndicator((int) _level.GetBall().StartingBallVelocity / 4);
        } else if (Input.GetMouseButtonUp(0) && _level.GetBall().OnPlayer) {
            _level.GetBall().Position.x = _level.GetPlayer().x;
            _level.GetBall().Position.y = _level.GetPlayer().y;
            _level.GetBall().Velocity.x = (Input.mouseX - _level.GetPlayer().x + _level.GetXOffset());
            _level.GetBall().Velocity.y = (Input.mouseY - _level.GetPlayer().y + _level.GetYOffSet());
            _level.GetBall().Velocity.Normalize().Scale(_level.GetBall().StartingBallVelocity);
            _level.GetBall().OnPlayer = false;
            _level.GetBall().StartingBallVelocity = Ball.SPEED;
            RemoveIndicator();
            _level.GetBall().chargeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee = false;
            //_sounds.StopCharge();
            //_sounds.PlayShoot();
        } else if (!_level.GetBall().OnPlayer) {
            _level.GetBall().Velocity.Add(_gravity);
            for (int i = 0; i <= Ball.REPETITIONS; i++) {
                _level.GetBall().Step();
                CheckAllLines(_level.GetBall());
            }
        }
    }

    public void CheckStones() {
        for (int i = 0; i < _level.GetStones().Count; i++) {
            /// <summary>
            /// Fix stone distance it is now half of the distance it should be radius minus distance/2
            /// Add a check to see if after reset ball is still between lines
            /// </summary>

            if (_level.GetStones()[i].Position.DistanceTo(_level.GetBall().Position) < _level.GetStones()[i].radius + _level.GetBall().radius && !_level.GetStones()[i].hitPlayer) {
                //_sounds.PlayBallRockCollision();
                float _tempdistance = _level.GetStones()[i].Position.DistanceTo(_level.GetBall().Position);
                ;
                Vec2 _stoneToStone = _level.GetStones()[i].Position.Clone().Subtract(_level.GetBall().Position).Normalize();
                //_stones[i].position.Add(_stoneToStone.Scale(0.5f));
                _level.GetBall().Position.Subtract(_stoneToStone.Scale(_level.GetBall().radius - _tempdistance / 2));
                _level.GetStones()[i].Velocity = _level.GetBall().Velocity.Clone();//new Vec2(1, 0).Scale(_ball.velocity.Length());
                CheckAllLines(_level.GetStones()[i]);
                _level.GetStones()[i].Step();
                _level.GetBall().Velocity = Vec2.zero;
                //_ball.position.Clone().Subtract(_stones[i].position).Normalize()
                //_ball.velocity.ReflectOnPoint(_ball.position.Clone().Subtract(_stones[i].position).Normalize(), 1);
                CheckAllLines(_level.GetBall());
                _level.GetBall().Step();
                //CollisionFix2Balls(stone, _ball);.Scale
                _level.GetStones()[i].active = true;
                //stone.hitPlayer = true;
            }
            if (_level.GetStones()[i].active) {
                CheckInGravityChangers(_level.GetStones()[i]);
                _level.GetStones()[i].Velocity.Add(_gravity);
                for (int j = 0; j < Ball.REPETITIONS; j++) {
                    CheckPressurePlatesCollision(_level.GetStones()[i]);
                    CheckAllLines(_level.GetStones()[i]);
                    _level.GetStones()[i].Step();
                    //_sounds.PlayRockBounce();
                }
            }
            for (int j = 0; j < _level.GetStones().Count; j++) {
                float _tempDistance = _level.GetStones()[i].Position.DistanceTo(_level.GetStones()[j].Position);
                if (j != i && _tempDistance < _level.GetStones()[i].radius + _level.GetStones()[j].radius) {
                    //stone.position.x - ();
                    //stone.position.y - ();
                    Vec2 _stoneToStone = _level.GetStones()[i].Position.Clone().Subtract(_level.GetStones()[j].Position).Normalize();
                    _level.GetStones()[i].Position.Add(_stoneToStone.Scale(_level.GetStones()[i].radius - _tempDistance / 2));
                    //_stones[j].position.Subtract(_stoneToStone.Scale(0.5f));
                    _level.GetStones()[j].active = true;
                    //if (!stone2.started)
                    {
                        //_sounds.PlayRockBounce();
                        _level.GetStones()[j].Velocity = _level.GetStones()[i].Velocity.Clone();//new Vec2(1, 0).Scale(stone.velocity.Length());
                        // stone2.started = true;
                    }
                    _level.GetStones()[i].hitPlayer = false;
                    _level.GetStones()[i].Velocity.Scale(0.0f);
                    //CheckAllLines(_stones[i]);
                    //CheckAllLines(_stones[j]);
                    _level.GetStones()[i].Step();
                    _level.GetStones()[j].Step();
                }
            }
        }
    }

    private bool Inside(Vec2 v1, Vec2 v2, Vec2 v3) {
        if (v1.x <= v3.x && v2.x >= v3.x && v1.y <= v3.y && v2.y >= v3.y)
            return true;
        if (v1.x >= v3.x && v2.x <= v3.x && v1.y >= v3.y && v2.y <= v3.y)
            return true;
        return false;
    }

    private void HandleIndicator(int pPower) {
        _level.GetPlayer().GetIndicator().x = _level.GetPlayer().x;
        _level.GetPlayer().GetIndicator().y = _level.GetPlayer().y;
        _level.GetPlayer().GetIndicator().SetPower(pPower);
    }

    private void RemoveIndicator() {
        _goingUp = true;
        if(_level.GetPlayer().GetIndicator()!=null)
        _level.GetPlayer().GetIndicator().Destroy();
        _level.GetPlayer().SetIndicator(null);
    }

    public Vec2 CheckIntersection(Vec2 lineStart, Vec2 lineEnd, Vec2 ballPosition, Vec2 ballNextPosition, Vec2 difference) {
        // check which side of the line we collide with:
        Vec2 velocity = ballNextPosition.Clone().Subtract(ballPosition);
        float direction = velocity.Dot(difference); // if positive, we collide with the back of the line

        if (direction < 0) {
            lineStart.Add(difference);
            lineEnd.Add(difference);
            // ua is the percentage of the line where the intersection is:
            float ua = ((ballNextPosition.x - ballPosition.x) * (lineStart.y - ballPosition.y) - (ballNextPosition.y - ballPosition.y) * (lineStart.x - ballPosition.x)) / ((ballNextPosition.y - ballPosition.y) * (lineEnd.x - lineStart.x) - (ballNextPosition.x - ballPosition.x) * (lineEnd.y - lineStart.y));
            // ub is the percentage of the "velocity" until point of impact:
            float ub = ((lineEnd.x - lineStart.x) * (lineStart.y - ballPosition.y) - (lineEnd.y - lineStart.y) * (lineStart.x - ballPosition.x)) / ((ballNextPosition.y - ballPosition.y) * (lineEnd.x - lineStart.x) - (ballNextPosition.x - ballPosition.x) * (lineEnd.y - lineStart.y));
            //Console.WriteLine(ua+"||"+ub);
            Vec2 _tempIntersect = new Vec2(lineStart.x + ua * (lineEnd.x - lineStart.x), lineStart.y + ua * (lineEnd.y - lineStart.y));


            if (ub <= 1 && ub >= -EPSILON && ua < 1 && ua >= 0) {
                //if (ub < 0)
                //    Console.WriteLine("WARNING: negative time of impact! : " + ub);
                return _tempIntersect;//.Add(addition.Normalize());
            }
        } else {
            Vec2 lineStartUnderneath = lineStart.Clone();
            Vec2 lineEndUnderneath = lineEnd.Clone();
            lineStartUnderneath.Subtract(difference);
            lineEndUnderneath.Subtract(difference);

            float ua = ((ballNextPosition.x - ballPosition.x) * (lineStartUnderneath.y - ballPosition.y) - (ballNextPosition.y - ballPosition.y) * (lineStartUnderneath.x - ballPosition.x)) / ((ballNextPosition.y - ballPosition.y) * (lineEndUnderneath.x - lineStartUnderneath.x) - (ballNextPosition.x - ballPosition.x) * (lineEndUnderneath.y - lineStartUnderneath.y));
            float ub = ((lineEndUnderneath.x - lineStartUnderneath.x) * (lineStartUnderneath.y - ballPosition.y) - (lineEndUnderneath.y - lineStartUnderneath.y) * (lineStartUnderneath.x - ballPosition.x)) / ((ballNextPosition.y - ballPosition.y) * (lineEndUnderneath.x - lineStartUnderneath.x) - (ballNextPosition.x - ballPosition.x) * (lineEndUnderneath.y - lineStartUnderneath.y));
            //Console.WriteLine(ua+"||"+ub);
            Vec2 _tempIntersect = new Vec2(lineStartUnderneath.x + ua * (lineEndUnderneath.x - lineStartUnderneath.x), lineStartUnderneath.y + ua * (lineEndUnderneath.y - lineStartUnderneath.y));
            if (ub <= 1 && ub >= -EPSILON && ua < 1 && ua >= 0)
                return _tempIntersect;//.Subtract(addition.Normalize());
            //else return Vec2.zero;
        }
        return Vec2.zero;
    }

    private void ActualBounce(Ball ball, LineSegment line, bool stick) {
        //_ballToLineStart = _ball.position.Clone().Subtract(line.start);
        //_distance = Mathf.Abs(_ballToLineStart.Dot(line.lineOnOriginNormalized.Normal().Clone()));
        _intersection = CheckIntersection(line.start.Clone(), line.end.Clone(), ball.Position, ball.NextPosition, line.lineOnOriginNormalized.Normal().Scale(ball.radius - 2));//try on border
        float _distanceToStart = line.start.DistanceTo(ball.NextPosition);
        float _distanceToEnd = line.end.DistanceTo(ball.NextPosition);
        //Console.WriteLine(_intersection);
        if (_intersection.y != 0) {
            if (stick) {
                ball.Velocity = Vec2.zero;
                ball.StartedTimer = true;
                ball.OnPlayer = true;
            } else {
                // _sounds.PlayBallBounce();
                ball.Position = _intersection;
                ball.UpdateNextPosition();
                //ball.velocity = Vec2.zero;
                ball.Velocity.Reflect(line.lineOnOriginNormalized, Ball.ELASTICITY);
                ball.Velocity.Scale(Ball.COLLISION_FRICTION);
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
            foreach (Vec2 cap in caps) {
                _distanceToStart = cap.DistanceTo(ball.NextPosition);
                if (_distanceToStart < ball.radius) {
                    if (stick) {
                        ball.Velocity = Vec2.zero;
                        ball.StartedTimer = true;
                        ball.OnPlayer = true;
                    } else {
                        float tempDistance = cap.DistanceTo(ball.NextPosition);
                        ;
                        Vec2 collisionNormal = ball.NextPosition.Clone().Subtract(cap).Normalize();
                        //_stones[i].position.Add(_stoneToStone.Scale(0.5f));
                        ball.Position = ball.NextPosition.Clone().Add(collisionNormal.Clone().Scale(ball.radius - tempDistance)); //.Subtract(collisionNormal.Scale(_ball.radius - tempDistance / 2));
                        //_sounds.PlayBallBounce();
                        //ball.position.Subtract(ball.velocity.Clone().Normalize().Scale(ball.radius));
                        ball.UpdateNextPosition();
                        ball.Velocity.ReflectOnPoint(collisionNormal, Ball.ELASTICITY);
                        //Console.WriteLine(ball.velocity.Length()+"||"+collisionNormal.Length());
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

    public void CheckTrophyCollision() {
        if (!_level.GetBall().OnPlayer) {
            foreach (Trophy trophy in _level.GetTrophies()) {
                if (_level.GetBall().HitTest(trophy)) {
                    if (!trophy.IsDestroyed()) {
                        _level.GetTrophyArray().SetValue(1, trophy.Id - 1);
                        _level.GetHUD().ReDrawTrophy(trophy.Id - 1);
                        _level.GetPlayer().AmountOfTrophies++;
                    }
                    trophy.Destroy();
                    //_sounds.PlayPickUp();
                }
            }
        }
    }

    public void CheckPotCollision() {
        if (!_level.GetBall().OnPlayer) {
            foreach (Pot pot in _level.GetPots()) {
                if (_level.GetBall().HitTest(pot)) {
                    if (!pot.IsDestroyed()) {
                        int score = _rnd.Next(100, 275);
                        _level.GetPlayer().Score += score;
                        pot.Canvas.graphics.DrawString("+" + score, new Font(FontFamily.GenericSansSerif, 18, FontStyle.Italic), Brushes.Green, 0, 0);
                        new Timer(1500, pot.Canvas.Destroy);
                    }
                    //_sounds.PlayBreakPot();
                    pot.Destroy();
                }
            }
        }
    }

    public void CheckRopeCollision() {
        foreach (Rope rope in _level.GetRopes()) {
            if (_level.GetBall().HitTest(rope)) {
                if (!rope.IsDestroyed()) {
                    foreach (Bridge bridge in _level.GetBridges()) {
                        if (bridge.BridgeName == rope.BridgeToDrop) {
                            bridge.GetBridgePlank().StartAnimation = true;
                            bridge.Down = true;
                            //_sounds.PlayBridgeFall();
                        }
                    }
                }
                //_sounds.PlayCutRope();
                rope.Destroy();
            }
        }
    }

    private void CheckInGravityChangers(Ball ball) {
        foreach (GravityChanger gravchangers in _level.GetGravityChangers()) {
            if (ball.Position.x < gravchangers.x + gravchangers.width / 2 &&
                ball.Position.x > gravchangers.x - gravchangers.width / 2 &&
                ball.Position.y > gravchangers.y - gravchangers.height / 2 &&
                ball.Position.y < gravchangers.y + gravchangers.height / 2) {
                ball.Velocity.Add(gravchangers.changedGravity);
            }
        }
    }

    private void CheckPressurePlatesCollision(Stone ball) {
        foreach (PressurePlate presspl in _level.GetPressurePlates()) {
            if (ball.Position.x < presspl.x + presspl.width / 2 &&
                ball.Position.x > presspl.x - presspl.width / 2 &&
                ball.Position.y < presspl.y && ball.Position.y > presspl.y - ball.height / 2) {
                    foreach (Sprite sprite in _level.GetPressurePlateObjects()) {
                        presspl.OpenCorresponding(sprite);
                    }
                if (presspl.cover) {
                    ball.active = false;
                    ball.y = presspl.y - ball.height / 2;
                    _level.GetLines().Add(presspl.coverLine);
                    _level.AddChild(presspl.coverLine);
                }
            }
        }
    }

    private void ResetBall() {
        _level.GetBall().Position.x = _level.GetPlayer().x - _level.GetBall().width;
        _level.GetBall().Position.y = _level.GetPlayer().y - _level.GetBall().height-20;
        _level.GetBall().Velocity = Vec2.zero;
        _level.GetBall().OnPlayer = true;
        _level.GetBall().Step();
    }

    public void HandleStickyBall() {
        if (_level.GetBall().StartedTimer) {
            if (_explosionWait == Ball.WAITFORBOOM) {
                //_sounds.PlayExplosion();
                for (int i = 0; i < _level.GetDestroyables().Count; i++) {
                    Plank plank = _level.GetDestroyables()[i];
                    if (_level.GetBall().Position.DistanceTo(plank.position) < Ball.BLASTSIZE) {
                        //_sounds.PlayPlankBlow();
                        _level.GetDestroyables().Remove(plank);
                        plank.Destroy();
                        i--;
                    }
                }
                ResetBall();
                _explosionWait = 0;
                _level.GetBall().StartedTimer = false;
            }
            _explosionWait++;
        }
    }

    public void HandleDestructablePlanks() {
        if (_level.GetBall().IsExploding) {
            foreach (Plank plank in _level.GetPlanks()) {
                if (_level.GetBall().HitTest(plank)) {
                    _level.GetBall().Velocity = new Vec2();
                    _level.GetBall().StartedTimer = true;
                    _level.GetBall().OnPlayer = true;
                }
            }
        }
    }
}
