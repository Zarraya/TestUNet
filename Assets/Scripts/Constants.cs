using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {

    public const float STAGGER_FULL = 1.0f;
    public const float STAGGER_HALF = 0.5f;
    public const float STAGGER_THIRD = 0.33f;

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
    
    public enum StaggerState
    {
        full,
        half,
        third,
        none
    }
}
