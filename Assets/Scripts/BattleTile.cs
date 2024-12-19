// BattleTile.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTile
{
    public int x;
    public int y;
    public bool isOccupied;

    public BattleTile(int x, int y)
    {
        this.x = x;
        this.y = y;
        isOccupied = false;
    }
}
