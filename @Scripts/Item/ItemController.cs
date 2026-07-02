using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemController
{
    public Board Board { get; private set; }

    public ItemController(Board board)
    {
        Board = board;
    }
}
