using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {
    
    public enum Direction
    {
        up,
        down,
        left,
        right,
        upleft,
        upright,
        downleft,
        downright
    }

    public enum GameMode
    {
        menu,
        match,
        suddendeath
    }
}
