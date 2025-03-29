using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CursorManager : MonoBehaviour
{
    public static CursorManager inst;
    
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D hoverCursor;

    private Vector2 cursorHotspot;

    private void Awake()
    {
        if (!inst)
        {
            inst = this;    
        }
    }

    public void SetDefaultCursor()
    {
        //Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.ForceSoftware);
    }

    public void SetHoverCursor()
    {
        //Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.ForceSoftware);
    }
}
