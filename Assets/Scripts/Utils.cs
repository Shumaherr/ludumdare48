using System;
using UnityEngine;

public static class DoubleExtension
{
    //default dound base is 5
    public static int RoundToValue(float d, int value = 5) =>
        Mathf.RoundToInt(d / value) * value;
}