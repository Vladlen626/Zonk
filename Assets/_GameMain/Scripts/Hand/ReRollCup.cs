using UnityEngine;
using UnityEngine.Events;

public class ReRollCup : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnReRoll = new UnityEvent();
    
    public void Enable()
    {
        transform.localScale = Vector3.one;
    }

    public void Disable()
    {
        transform.localScale = Vector3.zero;
    }

    // _____________ Private _____________
    
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