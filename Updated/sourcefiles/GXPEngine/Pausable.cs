using System;
using System.Collections.Generic;
using GXPEngine;

public class Pausable : AnimationSprite
{
    static List<Pausable> pauseList;

    public Pausable(string name, int pCol, int pRow) : base(name, pCol, pRow) {
        pauseList = new List<Pausable>();
        pauseList.Add(this);
    }

    public static void Pause() {
        foreach (Pausable pausable in pauseList) {
            Game.main.Remove(pausable);
        }
    }

    public static void UnPause() {
        foreach (Pausable pausable in pauseList) {
            Game.main.Add(pausable);
        }
    }
}

