using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


public class PlayingHand : NetworkBehaviour
{
    [SyncVar] private uint ownerNetId;
    public bool IsOwner => NetworkClient.connection != null && NetworkClient.connection.identity.netId == ownerNetId;

    [HideInInspector] public UnityEvent OnTurnEnd;
    [SerializeField] private ButtonNetworkObject reRollButton;
    [SerializeField] private ButtonNetworkObject endTurnButton;
    [SerializeField] private SavedDiceController savedDices;
    [SerializeField] private RollDiceController rollDices;
    [SerializeField] private ChosenDiceController chosenDices;

    private ScoreController scoreController;

    [Command(requiresAuthority = false)]
    public void CmdSetOwner(Player player)
    {
        ownerNetId = player.netId;
        SetPlayerDices(DiceManager.Instance.GetPlayerDices(ownerNetId));
        reRollButton.CmdSetOwner(ownerNetId);
        endTurnButton.CmdSetOwner(ownerNetId);
        scoreController = player.GetScoreController();
        savedDices.SetScoreController(scoreController);
        chosenDices.SetScoreController(scoreController);
    }

    private void Start()
    {
        chosenDices.onScoreChanged.AddListener(HandleScoreChanged);
        reRollButton.OnButtonPressed.AddListener(HandleRollDices);
        endTurnButton.OnButtonPressed.AddListener(EndTurn);
    }
    
    private void SaveDices()
    {
        if (scoreController.ChosenScore <= 0) return;
        rollDices.RemoveDices(chosenDices.GetDices());
        savedDices.SaveDices(chosenDices.GetDices());
    }

    [Command(requiresAuthority = false)]
    private void HandleRollDices()
    {
        SaveDices();
        UnChoseAllDices();
        DisableButtons();
        RollDices();
    }

    private void RollDices()
    {
        if (rollDices.IsEmpty())
        {
            RestoreRollDices();
        }
        rollDices.Roll();
        if (rollDices.IsRollSuccessful()) return;
        savedDices.ResetScore();
        EndTurn();
    }

    private void SetPlayerDices(Dice[] playerDices)
    {
        rollDices.FillDices(playerDices);
        chosenDices.SubscribeOnDiceChosen(playerDices);
        EnableButtons();
    }

    [Command(requiresAuthority = false)]
    private void EndTurn()
    {
        SaveDices();
        savedDices.ClearDices();
        UnChoseAllDices();
        DisableButtons();
        savedDices.SaveScore();
        OnTurnEnd.Invoke();
    }

    [Command(requiresAuthority = false)]
    private void HandleScoreChanged()
    {
        if (scoreController.ChosenScore <= 0)
            DisableButtons();
        else
            EnableButtons();
    }

    private void DisableButtons()
    {
        reRollButton.Disable();
        endTurnButton.Disable();
    }

    private void EnableButtons()
    {
        reRollButton.Enable();
        endTurnButton.Enable();
    }

    private void UnChoseAllDices()
    {
        foreach (var dice in chosenDices.GetDices())
        {
            dice.UnChose();
        }
    }

    private void RestoreRollDices()
    {
        rollDices.FillDices(savedDices.GetSavedDices());
        savedDices.ClearDices();
    }
}