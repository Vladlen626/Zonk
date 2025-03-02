using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class ButtonNetworkObject : NetworkBehaviour
{
    [SyncVar] private uint ownerNetId;

    [SyncVar(hook = nameof(OnVisibilityChange))]
    private bool isVisible;

    private bool IsOwner => NetworkClient.connection != null && NetworkClient.connection.identity.netId == ownerNetId;

    [HideInInspector] public UnityEvent OnButtonPressed = new UnityEvent();

    public void CmdSetOwner(uint newOwnerNetId)
    {
        ownerNetId = newOwnerNetId;
    }

    public void Enable()
    {
        isVisible = true;
        Show();
    }

    public void Disable()
    {
        isVisible = false;
        Hide();
    }

    // _____________ Private _____________


    protected virtual void CallButtonPressed()
    {
        OnButtonPressed.Invoke();
    }

    private void OnMouseDown()
    {
        if (!IsOwner) return;
        transform.localScale = Vector3.one * 0.9f;
    }

    private void OnMouseUp()
    {
        if (!IsOwner) return;
        CallButtonPressed();
        transform.localScale = Vector3.one * 1f;
    }

    private void Hide()
    {
        transform.localScale = Vector3.zero;
    }

    private void Show()
    {
        transform.localScale = Vector3.one;
    }

    private void OnVisibilityChange(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
}