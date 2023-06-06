using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyWeightPointer
{
    public static readonly FlyWeight EntityStates = new FlyWeight()
    {
        speed = 3,
        viewAngle = 90,
        viewRadius = 13
    };
}