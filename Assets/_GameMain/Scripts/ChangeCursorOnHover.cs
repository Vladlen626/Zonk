using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCursorOnHover : MonoBehaviour
{
    private void OnMouseEnter()
    {
        CursorManager.inst.SetHoverCursor();
    }
    
    private void OnMouseExit()
    {
        CursorManager.inst.SetDefaultCursor();
    }
}

