using System;
using DG.Tweening;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Dice : NetworkBehaviour
{
    [SyncVar] private uint ownerNetId;

    public bool IsOwner => NetworkClient.connection != null && NetworkClient.connection.identity.netId == ownerNetId;

    [SyncVar(hook = nameof(OnDiceSideUpdated))]
    private int currentSideValue;

    [SyncVar(hook = nameof(OnChosenUpdated))]
    private bool isChosen;

    [SyncVar] private bool isSaved;

    [HideInInspector] public UnityEvent<Dice> onDiceChosen = new UnityEvent<Dice>();
    [HideInInspector] public UnityEvent<Dice> onDiceUnChosen = new UnityEvent<Dice>();
    [SerializeField] private Side[] sides;
    [SerializeField] private DiceVisualController diceVisualController;
    public DiceType type;

    [Command(requiresAuthority = false)]
    public void CmdSetOwner(uint newOwnerNetId)
    {
        ownerNetId = newOwnerNetId;
        Hide();
    }

    public void Roll()
    {
        SetSideValue(sides[Random.Range(0, sides.Length)].GetValue());
        diceVisualController.PlayRollAnimation();
    }

    public void Hide()
    {
        isChosen = false;
        isSaved = false;
        currentSideValue = 0;
        diceVisualController.UpdateChosenVisual(isChosen);
        diceVisualController.SetSideMesh(currentSideValue);
    }

    public int GetCurrentSideValue()
    {
        return currentSideValue;
    }

    public void Chose()
    {
        isChosen = true;
        AudioManager.inst.PlaySound(SoundNames.MoveDice);
        diceVisualController.UpdateChosenVisual(isChosen);
        onDiceChosen.Invoke(this);
    }

    public void UnChose()
    {
        isChosen = false;
        AudioManager.inst.PlaySound(SoundNames.MoveDice);
        diceVisualController.UpdateChosenVisual(isChosen);
        onDiceUnChosen.Invoke(this);
    }

    public void Save(Vector3 savePosition)
    {
        isSaved = true;
        diceVisualController.MoveToSavePosition(savePosition);
    }

    public void UnSave()
    {
        isSaved = false;
    }

// _____________ Private _____________

    [Command]
    private void CmdToggleChoseDice()
    {
        if (isChosen)
        {
            UnChose();
        }
        else
        {
            Chose();
        }
    }

    private void OnMouseUp()
    {
        if (!IsOwner || isSaved) return;
        CmdToggleChoseDice();
        diceVisualController.Released();
    }

    private void OnMouseDown()
    {
        if (!IsOwner || isSaved) return;
        diceVisualController.Pressed();
    }
    
    private void OnMouseEnter()
    {
        if (!IsOwner || isSaved) return;
        diceVisualController.OnHover();
    }

    private void OnMouseExit()
    {
        if (!IsOwner || isSaved) return;
        diceVisualController.OnUnHover();
    }
    
    private void SetSideValue(int sideValue)
    {
        currentSideValue = sideValue;
        diceVisualController.SetSideMesh(currentSideValue);
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