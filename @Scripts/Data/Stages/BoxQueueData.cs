using System.Collections.Generic;
using UnityEngine;

public class BoxQueueData {
    public Vector2Int Index { get; set; }
    public Direction Direction { get; set; }
    public List<BoxData> Boxes { get; set; }
}