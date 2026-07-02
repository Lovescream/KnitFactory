using System.Collections.Generic;
using UnityEngine;

public class MoveMachine
{
    public MoveMachineObject Object { get; private set; }
    public BeltBoard Board { get; private set; }
    public MoveMachineData Data { get; private set; }
    public Vector2 Position { get; }
    public MoveMachine ConnectMoveMachine { get; private set; }
    public Belt ConnectBelt { get; private set; }
    private BeltLane _nextLane;

    public MoveMachine(BeltBoard board, MoveMachineData data)
    {
        Board = board;
        Data = data;
        Position = data.Index;
    }

    public void GenerateObject()
    {
        Object = Main.Object.Instantiate<MoveMachineObject>();
        Object.Set(this);
        if (Data.PortType == PortType.Input)
        {
            ConnectBelt = Board.GetNearEndBelt(Data.Index);
            ConnectMoveMachine = GetOutputMoveMachine();
        }
        else
        {
            ConnectBelt = Board.GetNearStartBelt(Data.Index);
            ConnectMoveMachine = GetInputMoveMachine();
        }
        ConnectBelt.MoveMachine = this;
    }

    private MoveMachine GetOutputMoveMachine()
    {
        if (!Board.OutputMoveMachines.TryGetValue(Data.ConnectIndex, out MoveMachine outputMoveMachine))
        {
            Debug.LogError($"output move machine count {Data.ConnectIndex} does not exist");
        }
        return outputMoveMachine;
    }
    
    private MoveMachine GetInputMoveMachine()
    {
        if (!Board.InputMoveMachines.TryGetValue(Data.ConnectIndex, out MoveMachine inputMoveMachine))
        {
            Debug.LogError($"input move machine count {Data.ConnectIndex} does not exist");
        }
        return inputMoveMachine;
    }

    public void Poop(Knit knit) {
        if (knit == null) return;
        knit.MoveToBelt(_nextLane, ConnectBelt);
        _nextLane = _nextLane == BeltLane.Inner ? BeltLane.Outer : BeltLane.Inner;
    }

    public Vector3 GetStartPos()
    {
        return (ConnectBelt.Object.transform.position + Object.transform.position) * 0.5f;
    }
}
