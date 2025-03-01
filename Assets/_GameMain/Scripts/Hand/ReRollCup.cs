using System;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class ReRollCup : NetworkBehaviour
{
    [SyncVar] private uint ownerNetId;
    private bool IsOwner => NetworkClient.connection != null && NetworkClient.connection.identity.netId == ownerNetId;
    
    [HideInInspector] public UnityEvent OnReRoll = new UnityEvent();
    
    [Command(requiresAuthority = false)]
    public void CmdSetOwner(uint newOwnerNetId)
    {
        ownerNetId = newOwnerNetId;
    }
    
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
        if(!IsOwner) return;
        transform.localScale = Vector3.one * 0.9f;
    }

    private void OnMouseUp()
    {
        if(!IsOwner) return;
        OnReRoll.Invoke();
        transform.localScale = Vector3.one * 1f;
    }
}