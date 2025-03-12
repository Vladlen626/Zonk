using DG.Tweening;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class ButtonNetworkObject : NetworkBehaviour
{
    [SyncVar] private uint ownerNetId;

    [SyncVar(hook = nameof(OnVisibilityChange))]
    private bool isVisible;

    private bool IsOwner => NetworkClient.connection != null && NetworkClient.connection.identity.netId == ownerNetId;

    [SerializeField] private Animator animator;
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
        if (!IsOwner || !isVisible) return;
        Pressed();
    }
    
    private void OnMouseUp()
    {
        if (!IsOwner || !isVisible) return;
        Released();
    }


    [Command(requiresAuthority = false)]
    private void Pressed()
    {
        CallButtonPressed();
        RpcPressed();
    }

    [Command(requiresAuthority = false)]
    private void Released()
    {
        RpcReleased();
    }
    
    
    [ClientRpc]
    private void RpcPressed()
    {
        animator.Play("ButtonPressed", -1, 0f);
    }

    [ClientRpc]
    private void RpcReleased()
    {
        
    }
    

    private void Hide()
    {
        //transform.DOScale(Vector3.zero, 0.25f);
    }

    private void Show()
    {
        //transform.DOScale(Vector3.one, 0.25f);
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