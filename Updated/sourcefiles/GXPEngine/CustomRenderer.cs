using System;
using System.Collections.Generic;
using GXPEngine.Core;
using GXPEngine;

public class CustomRenderer : GameObject {
    public const int MAXLISTS = 15000;
    public const int COLUMNWIDTH = 50;

    private List<GameObject>[] _children = new List<GameObject>[MAXLISTS];

    private int _startX = 0;
    private int _endX = 0;
    
    public CustomRenderer () {
        for (int i = 0; i < MAXLISTS; i++) {
            _children[i] = new List<GameObject>();
        }
    }
    
    public void AddScenery(GameObject child) {
        int x = (int) (child.x / COLUMNWIDTH);
        if (x < 0) { return; }
        if (x >= _children.Length) { return; } //safety
        _children[x].Add(child);
    }
    
    private void Update() {
        // empty
    }

    public override void Render(GLContext glContext) {
        if (visible) {
            glContext.PushMatrix(matrix);

            RenderSelf(glContext);

            GXPEngine.Core.Vector2 position = this.TransformPoint(0, 0);//get position in global space (screen space)

            //determine what columns need to be rendered:
            _startX = -((int)Math.Floor(position.x / COLUMNWIDTH)) - 2; //left column
            _endX = _startX + 3 + (int)Mathf.Floor(game.width / COLUMNWIDTH); //right column (1 screen = 8 columns)

            for (int i = _startX; i < _endX; i++) {
                if (i >= 0 && i < _children.Length) {
                    foreach (GameObject child in _children[i]) {
                        child.Render(glContext);
                    }
                }
            }

            glContext.PopMatrix();
        }
    }

    public List<GameObject>[] GetChildObjects() {
        return _children;
    }
    
}
