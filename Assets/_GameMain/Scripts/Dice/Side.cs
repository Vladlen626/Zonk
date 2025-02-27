using System;
using UnityEngine;

[Serializable]
public struct Side
{ 
    [SerializeField][Range(1, 6)]
    private int value;

    public int GetValue()
    {
        return value;
    }
}
