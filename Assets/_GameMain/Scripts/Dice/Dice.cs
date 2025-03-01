using System;
using System.Runtime.InteropServices;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Dice : NetworkBehaviour
{
    [SyncVar] private uint ownerNetId;
    
    public bool IsOwner => NetworkClient.connection != null && NetworkClient.connection.identity.netId == ownerNetId;
    
    
    [SyncVar(hook = nameof(OnDiceSideUpdated))] private int currentSideValue;
    [SyncVar(hook = nameof(OnChosenUpdated))] private bool isChosen;

    [HideInInspector]
    public UnityEvent<Dice> onDiceChosen = new UnityEvent<Dice>();
    [SerializeField] private Side[] sides;
    [SerializeField] private DiceVisualController diceVisualController;

    public DiceType type;

    
    [Command(requiresAuthority = false)]
    public void CmdSetOwner(uint newOwnerNetId)
    {
        ownerNetId = newOwnerNetId;
        currentSideValue = 1;
        isChosen = false;
    }
    
    public void Roll()
    {
        SetSideValue(sides[Random.Range(0, sides.Length)].GetValue());
        diceVisualController.RandomizeRotation();
    }

    public void Hide()
    {
        diceVisualController.Hide();
        GetComponent<Collider>().enabled = false;
    }

    public int GetCurrentSideValue()
    {
        return currentSideValue;
    }

    // _____________ Private _____________

    private void OnMouseUp()
    {
        if (!IsOwner) return;
        CmdToggleChoseDice();
    }

    private void OnMouseDown()
    {
        if (!IsOwner) return;
        CmdTouchDice();
    }
    
    private void SetSideValue(int sideValue)
    {
        currentSideValue = sideValue;
        diceVisualController.SetSideMesh(currentSideValue);
    }
    
    [Command]
    private void CmdToggleChoseDice()
    {
        onDiceChosen.Invoke(this);
        isChosen = !isChosen;
        diceVisualController.UpdateChosenVisual(isChosen);
        diceVisualController.RpcThrow();
    }

    [Command]
    private void CmdTouchDice()
    {
        diceVisualController.RpcHandle();
    }

    private void OnDiceSideUpdated(int oldSideValue, int newSideValue)
    {
        diceVisualController.SetSideMesh(newSideValue);
    }

    private void OnChosenUpdated(bool oldChosenValue, bool newChosenValue)
    {
        diceVisualController.UpdateChosenVisual(newChosenValue);
    }
}