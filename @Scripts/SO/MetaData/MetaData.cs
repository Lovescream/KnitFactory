using System.Text;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "MetaData", menuName = "ActionFit/MetaData")]
public class MetaData : BaseSO
{
    public SerializedDictionary<int, RewordMetaType> dictRewordMetas = new();
}

public enum RewordMetaType
{
    None = -1,
    NewColor_Lime,
    KnitHidden,
    NewColor_Yellow,
    NewColor_Orange,
    KnitGroup,
    BoxHidden,
    BoxConnector,
}