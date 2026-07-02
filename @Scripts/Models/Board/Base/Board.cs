using System;
using Project.Scripts.Board;
using UnityEngine;

public class Board {
    
    #region Properties

    public BeltBoard BeltBoard { get; }
    public SpriteDim SpriteDim { get; }
    public ItemController ItemController { get; }
    public Vector2 Size { get; }
    public Vector2 Min { get; }
    public Vector2 Max { get; }
    public Vector2 Center { get; }
    public UI_GameScene UI_GameScene { get; }
    public BoardObject Object { get; private set; }

    #endregion

    #region Constructor

    public Board(StageDataKey stageDataKey, UI_GameScene uiScene) {
        BeltBoard = new(this, stageDataKey);
        SpriteDim = new SpriteDim(this);
        ItemController = new ItemController(this);
        UI_GameScene = uiScene;

        Min = new(Mathf.Min(BeltBoard.Min.x - BeltBoard.BeltSize.x * 0.5f), 0);
        Max = new(Mathf.Max(BeltBoard.Max.x + BeltBoard.BeltSize.x * 0.5f), BeltBoard.Size.y + BeltBoard.BeltSize.y * 0.5f);
        Size = new(BeltBoard.Size.x, BeltBoard.Size.y + 4);
        Center = new (BeltBoard.Center.x, BeltBoard.Center.y / 3 - 0.5f);
    }

    public void GenerateObject() {
        Object = Main.Object.Instantiate<BoardObject>();
        Object.Set(this);
        BeltBoard.GenerateObject();
        SpriteDim.GenerateObject();
    }

    #endregion
    
}