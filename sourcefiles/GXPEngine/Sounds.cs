using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

class Sounds
{
    private Sound _shoot;
    private Sound _charge;
    private SoundChannel _chargeChannel;
    private Sound _jump;
    private Sound _ballBounce;
    private Sound _rockBounce;
    private Sound _ballRockCollision;
    private Sound _walk;
    private Sound _cutRope;
    private Sound _bridgeFall;
    private Sound _explosion;
    private Sound _plankBlow;
    private Sound _breakPot;
    private Sound _pickUp;
    private Sound _switch;
    private Sound _music;
    private SoundChannel _musicChannel;

    public Sounds()
    {
        _switch = new Sound("assets//sounds//switch.wav", false, false);
        _shoot = new Sound("assets//sounds//shoot.wav", false, false);
        _charge = new Sound("assets//sounds//charge.wav", false, false);
        _jump = new Sound("assets//sounds//jump.wav", false, false);
        _ballBounce = new Sound("assets//sounds//ballBounce", false, false);
        _rockBounce = new Sound("assets//sounds//rockBounce", false, false);
        _ballRockCollision = new Sound("assets//sounds//ballRockCollision.wav", false, false);
        _walk = new Sound("assets//sounds//walk.wav", false, false);
        _cutRope = new Sound("assets//sounds//cutRope.wav", false, false);
        _bridgeFall = new Sound("assets//sounds//bridgeFall.wav", false, false);
        _explosion = new Sound("assets//sounds//explosion.wav", false, false);
        _plankBlow = new Sound("assets//sounds//plankBlow.wav", false, false);
        _breakPot = new Sound("assets//sounds//breakPot.wav", false, false);
        _pickUp = new Sound("assets//sounds//pickUp.wav", false, false);

        _music = new Sound("assets//sounds//music.mp3", true, true);
    }

    public void PlaySwitch()
    {
        _switch.Play();
    }

    public void PlayShoot()
    {
        _shoot.Play();
    }

    public void PlayCharge()
    {
       _chargeChannel=_charge.Play();
    }

    public void StopCharge()
    {
        _chargeChannel.Stop();
    }

    public void PlayJump()
    {
        _jump.Play();
    }
    public void PlayBallBounce()
    {
        Console.WriteLine("ballbounce");
        _ballBounce.Play();
    }
    public void PlayRockBounce()
    {
        _rockBounce.Play();
    }
    public void PlayBallRockCollision()
    {
        _ballRockCollision.Play();
    }
    public void PlayWalk()
    {
        Console.WriteLine("walk");
        _walk.Play();
    }
    public void PlayCutRope()
    {
        _cutRope.Play();
    }
    public void PlayBridgeFall()
    {
        _bridgeFall.Play();
    }
    public void PlayExplosion()
    {
        _explosion.Play();
    }
    public void PlayPlankBlow()
    {
        _plankBlow.Play();
    }
    public void PlayBreakPot()
    {
        _breakPot.Play();
    }
    public void PlayPickUp()
    {
        _pickUp.Play();
    }
    public void PlayMusic()
    {
        _musicChannel = _music.Play();
    }
}

