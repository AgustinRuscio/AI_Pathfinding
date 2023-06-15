//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyWeightPointer
{
    public static readonly FlyWeight EnemiesAtributs = new FlyWeight()
    {
        speed = 3,
        viewAngle = 90,
        viewRadius = 13,
        waypointRadius = 1,
        nodeDistance = 2,
        playerDistance = 3,
    };
}