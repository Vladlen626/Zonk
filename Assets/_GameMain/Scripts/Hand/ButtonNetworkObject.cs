using DG.Tweening;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class ButtonNetworkObject : NetworkBehaviour
{
    [SyncVar] private uint ownerNetId;
    [SyncVar] private bool isEnable;

    private bool IsOwner => NetworkClient.connection != null && NetworkClient.connection.identity.netId == ownerNetId;

    [SerializeField] private Animator animator;
    [HideInInspector] public UnityEvent OnButtonPressed = new UnityEvent();

    public void CmdSetOwner(uint newOwnerNetId)
    {
        ownerNetId = newOwnerNetId;
    }
    
    public void Enable()
    {
        isEnable = true;
    }

    public void Disable()
    {
        isEnable = false;
    }

    // _____________ Private _____________


    protected virtual void CallButtonPressed()
    {
        if (!isEnable) return;
        AudioManager.inst.PlaySound(SoundNames.ButtonPressed);
        OnButtonPressed.Invoke();
    }
    
    private void OnMouseDown()
    {
        if (!IsOwner) return;
        Pressed();
    }
    

    [Command(requiresAuthority = false)]
    private void Pressed()
    {
        RpcPressed();
        CallButtonPressed();
    }
    
    [ClientRpc]
    private void RpcPressed()
    {
        animator.Play("ButtonPressed", -1, 0f);
    }
    
}