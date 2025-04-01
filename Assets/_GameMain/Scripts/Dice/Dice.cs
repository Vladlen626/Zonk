using Mirror;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Dice : NetworkBehaviour
{
    [SyncVar] private uint ownerNetId;
    public UnityAction<Dice> onDiceChosen;
    public UnityAction<Dice> onDiceUnChosen;
    public bool IsOwner => NetworkClient.connection != null && NetworkClient.connection.identity.netId == ownerNetId;
    public DiceType type;
    
    [SyncVar(hook = nameof(OnDiceSideUpdated))]
    private int currentSideValue;

    [SyncVar(hook = nameof(OnChosenUpdated))]
    private bool isChosen;

    [SyncVar] private bool isSaved;
    [SyncVar] private bool isRolling;
    
    [SerializeField] private Side[] sides;
    [SerializeField] private DiceVisualController diceVisualController;

    [Command(requiresAuthority = false)]
    public void CmdSetOwner(uint newOwnerNetId)
    {
        ownerNetId = newOwnerNetId;
        Hide();
    }

    public void Roll()
    {
        SetSideValue(sides[Random.Range(0, sides.Length)].GetValue());
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
        onDiceChosen?.Invoke(this);
    }

    public void UnChose()
    {
        isChosen = false;
        AudioManager.inst.PlaySound(SoundNames.MoveDice);
        diceVisualController.UpdateChosenVisual(isChosen);
        onDiceUnChosen?.Invoke(this);
    }

    public void Save()
    {
        isSaved = true;
    }

    public void UnSave()
    {
        isSaved = false;
    }

    public void SerRollState(bool state)
    {
        isRolling = state;
    }

    public DiceVisualController GetVisualController()
    {
        return diceVisualController;
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
        if (!IsOwner || isSaved || isRolling) return;
        CmdToggleChoseDice();
        diceVisualController.Released();
    }

    private void OnMouseDown()
    {
        if (!IsOwner || isSaved || isRolling) return;
        diceVisualController.Pressed();
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