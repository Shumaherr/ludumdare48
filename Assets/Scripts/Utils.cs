using System;
using UnityEngine;

public static class DoubleExtension
{
    //default dound base is 5
    public static int RoundToValue(float d, int value = 5) =>
        Mathf.RoundToInt(d / value) * value;
}

public static class Utils
{
    public static GridCell[,] TrimArray(int rowToRemove, GridCell[,] originalArray)
    {
        GridCell[,] result = new GridCell[originalArray.GetLength(0), originalArray.GetLength(1) - 1];

        for (int i = 0, j = 0; i < originalArray.GetLength(0); i++)
        {

            for (int k = 0, u = 0; k < originalArray.GetLength(1); k++)
            {
                if (k == rowToRemove)
                    continue;

                result[j, u] = originalArray[i, k];
                u++;
            }
            j++;
        }

        return result;
    }
}