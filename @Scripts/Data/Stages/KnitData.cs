using System.Collections.Generic;
using UnityEngine;

public class KnitData {
    public ColorType Color { get; set; }
    public List<KnitGimmickData> Gimmicks { get; set; } = new();
}