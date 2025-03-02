using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct Combo
{
    [SerializeField] [Range(1, 6)] private int[] neededValues;
    [SerializeField] private int score;

    public int GetScore()
    {
        return score;
    }

    public int[] GetNeededValues()
    {
        return neededValues;
    }
}