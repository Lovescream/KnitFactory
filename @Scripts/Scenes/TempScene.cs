using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TempScene : SceneBase {

    #region Properties

    public InputController InputController { get; private set; }
    
    #endregion
    
    #region MonoBehaviours

    protected override void Update() {
        base.Update();
        Main.Time.OnUpdate(Time.deltaTime);
    }
    
    void OnDisable() {
        InputController?.OnDestroy();
    }

    #endregion

    protected override bool Initialize() {
        if (!base.Initialize()) return false;
        //
        // StageData stageData = new StageData {
        //     Index = 1,
        //     Difficulty = Difficulty.Normal,
        //     TimeLimit = 100,
        //     Belts = new List<BeltData> {
        //         new BeltData {
        //             Index = new(0, 1),
        //             StartDirection = Direction.Top,
        //             EndDirection = Direction.Bottom
        //         },
        //         new BeltData {
        //             Index = new(0, 0),
        //             StartDirection = Direction.Top,
        //             EndDirection = Direction.Right
        //         },
        //         new BeltData {
        //             Index = new(1, 0),
        //             StartDirection = Direction.Left,
        //             EndDirection = Direction.Right
        //         },
        //         new BeltData {
        //             Index = new(2, 0),
        //             StartDirection = Direction.Left,
        //             EndDirection = Direction.Right
        //         },
        //         new BeltData {
        //             Index = new(3, 0),
        //             StartDirection = Direction.Left,
        //             EndDirection = Direction.Right
        //         },
        //         new BeltData {
        //             Index = new(4, 0),
        //             StartDirection = Direction.Left,
        //             EndDirection = Direction.Right
        //         },
        //         new BeltData {
        //             Index = new(5, 0),
        //             StartDirection = Direction.Left,
        //             EndDirection = Direction.Top
        //         },
        //         new BeltData {
        //             Index = new(5, 1),
        //             StartDirection = Direction.Bottom,
        //             EndDirection = Direction.Top
        //         },
        //         new BeltData {
        //             Index = new(5, 2),
        //             StartDirection = Direction.Bottom,
        //             EndDirection = Direction.Top
        //         },
        //         new BeltData {
        //             Index = new(5, 3),
        //             StartDirection = Direction.Bottom,
        //             EndDirection = Direction.Top
        //         },
        //         new BeltData {
        //             Index = new(5, 4),
        //             StartDirection = Direction.Bottom,
        //             EndDirection = Direction.Top
        //         },
        //         new BeltData {
        //             Index = new(5, 5),
        //             StartDirection = Direction.Bottom,
        //             EndDirection = Direction.Left
        //         },
        //         new BeltData {
        //             Index = new(4, 5),
        //             StartDirection = Direction.Right,
        //             EndDirection = Direction.Left
        //         },
        //         new BeltData {
        //             Index = new(3, 5),
        //             StartDirection = Direction.Right,
        //             EndDirection = Direction.Left
        //         },
        //         new BeltData {
        //             Index = new(2, 5),
        //             StartDirection = Direction.Right,
        //             EndDirection = Direction.Left
        //         },
        //         new BeltData {
        //             Index = new(1, 5),
        //             StartDirection = Direction.Right,
        //             EndDirection = Direction.Left
        //         },
        //         new BeltData {
        //             Index = new(0, 5),
        //             StartDirection = Direction.Right,
        //             EndDirection = Direction.Bottom
        //         },
        //         new BeltData {
        //             Index = new(0, 4),
        //             StartDirection = Direction.Top,
        //             EndDirection = Direction.Bottom
        //         },
        //     },
        //     Bundles = new List<BundleData> {
        //         new BundleData {
        //             Knits = new List<KnitData> {
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Lime },
        //             },
        //         },
        //         new BundleData {
        //             Knits = new List<KnitData> {
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //             },
        //         },
        //         new BundleData {
        //             Knits = new List<KnitData> {
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Sky },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Pink },
        //                 new KnitData { Color = ColorType.Lime },
        //                 new KnitData { Color = ColorType.Lime },
        //             },
        //         },
        //     },
        //     BoxQueues = new List<BoxQueueData> {
        //         new BoxQueueData {
        //             Index = new(1, 1),
        //             Direction = Direction.Bottom,
        //             Boxes = new List<BoxData> {
        //                 new BoxData { Color = ColorType.Sky },
        //                 new BoxData { Color = ColorType.Pink },
        //                 new BoxData { Color = ColorType.Pink },
        //             }
        //         },
        //         new BoxQueueData {
        //             Index = new(4, 1),
        //             Direction = Direction.Right,
        //             Boxes = new List<BoxData> {
        //                 new BoxData { Color = ColorType.Lime },
        //                 new BoxData { Color = ColorType.Sky },
        //                 new BoxData { Color = ColorType.Lime },
        //             }
        //         },
        //         new BoxQueueData {
        //             Index = new(4, 4),
        //             Direction = Direction.Top,
        //             Boxes = new List<BoxData> {
        //                 new BoxData { Color = ColorType.Pink },
        //                 new BoxData { Color = ColorType.Pink },
        //                 new BoxData { Color = ColorType.Pink },
        //             }
        //         },
        //         new BoxQueueData {
        //             Index = new(1, 4),
        //             Direction = Direction.Left,
        //             Boxes = new List<BoxData> {
        //                 new BoxData { Color = ColorType.Sky },
        //                 new BoxData { Color = ColorType.Lime },
        //                 new BoxData { Color = ColorType.Lime },
        //             }
        //         },
        //     }
        // };
        //
        // Main.Board.GenerateBoard(stageData);
        // Main.Board.GenerateBoardObject();
        //
        // InputController = new();
        //
        // Main.Screen.SetTempCamera();
        
        return true;
    }
    
}