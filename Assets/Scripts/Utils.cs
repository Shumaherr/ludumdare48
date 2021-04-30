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
    
    public const int sortingOrderDefault = 5000;
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
    
    public static void CreateWorldTextPopup(string text, Vector3 localPosition, float popupTime = 1f) {
        CreateWorldTextPopup(null, text, localPosition, 20, Color.yellow, localPosition + new Vector3(0, 20), popupTime);
    }
        
    // Create a Text Popup in the World
    public static void CreateWorldTextPopup(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, Vector3 finalPopupPosition, float popupTime) {
        TextMesh textMesh = CreateWorldText(parent, text, localPosition, fontSize, color, TextAnchor.LowerLeft, TextAlignment.Left, sortingOrderDefault);
        Transform transform = textMesh.transform;
        Vector3 moveAmount = (finalPopupPosition - localPosition) / popupTime;
        CodeMonkey.Utils.FunctionUpdater.Create(delegate () {
            transform.position += moveAmount * Time.unscaledDeltaTime;
            popupTime -= Time.unscaledDeltaTime;
            if (popupTime <= 0f) {
                UnityEngine.Object.Destroy(transform.gameObject);
                return true;
            } else {
                return false;
            }
        }, "WorldTextPopup");
    }
    
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder) {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }
}