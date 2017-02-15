using GXPEngine;

public class AnimationButton : AnimationSprite {
    public AnimationButton(string pSpriteName, int cols, int rows, int frames = -1) : base(pSpriteName, cols, rows, frames) {
        currentFrame = 0;
    }
}

