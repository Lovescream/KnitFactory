using System;
using System.Collections.Generic;

public class StageDataKey : DataKey {
    public int Index { get; set; }
    public Difficulty Difficulty { get; set; }
    public int MaxKnitsOnBelt { get; set; }
    public int MaxKnitsOnDock { get; set; }

    public List<BeltData> Belts { get; set; }
    public List<BundleData> Bundles { get; set; }
    public List<BoxQueueData> BoxQueues { get; set; }
    public List<MoveMachineData> MoveMachines { get; set; }
    public List<KnitKeeperData> KnitKeepers { get; set; }
    public List<KnitSsagaeData> KnitSsagaes { get; set; }
}

public enum Difficulty {
    Normal,
    Hard,
    SuperHard,
}

[Flags]
public enum ColorType {
    None = 0,
    White = 1 << 0,
    Red = 1 << 1,
    Yellow = 1 << 2,
    Blue = 1 << 3,
    Orange = 1 << 4,
    Purple = 1 << 5,
    Pink = 1 << 6,
    Mint = 1 << 7,
    Sky = 1 << 8,
    Lime = 1 << 9,
}

public enum Direction {
    None = -1,
    Top,
    Right,
    Bottom,
    Left
}

public enum Orientation {
    None = -1,
    Vertical,
    Horizontal,
}

public enum PortType
{
    None = -1,
    Input,
    Output,
}

public enum BoxGimmickType
{
    None = -1,
    Hidden = 0,
    Connector = 1,
}

public enum KnitGimmickType
{
    None = -1,
    Hidden = 0,
    Rope = 1,
}