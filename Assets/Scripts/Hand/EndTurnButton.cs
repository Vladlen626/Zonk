using UnityEngine;
using UnityEngine.Events;

public class EndTurnButton : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent OnTurnEnd = new UnityEvent();
    private void OnMouseDown()
    {
        transform.localScale = Vector3.one * 0.9f;
    }

    private void OnMouseUp()
    {
        OnTurnEnd.Invoke();
        transform.localScale = Vector3.one * 1f;
    }
}
