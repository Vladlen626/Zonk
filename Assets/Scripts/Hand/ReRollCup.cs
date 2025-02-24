using UnityEngine;
using UnityEngine.Events;

public class ReRollCup : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent OnReRoll = new UnityEvent();
    private void OnMouseDown()
    {
        transform.localScale = Vector3.one * 0.9f;
    }

    private void OnMouseUp()
    {
        OnReRoll.Invoke();
        transform.localScale = Vector3.one * 1f;
    }
}
