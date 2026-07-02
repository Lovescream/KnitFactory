using System.Collections.Generic;
using UnityEngine;

public class BoxData {
    public ColorType Color { get; set; }
    public List<BoxGimmickData> Gimmicks { get; set; } = new();
}