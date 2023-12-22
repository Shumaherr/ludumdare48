using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CellData : ScriptableObject {
    public List<Sprite> Sprites;
    public GenericDictionary<int, float> LineToProbability;
    public int giveStone;
}