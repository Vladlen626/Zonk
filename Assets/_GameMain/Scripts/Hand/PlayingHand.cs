using System.Collections;
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
    
    [Header("DiceControllers")]
    [SerializeField] private SavedDiceController savedDices;
    [SerializeField] private RollDiceController rollDices;
    [SerializeField] private ChosenDiceController chosenDices;
    
    [Header("Buttons")]
    [SerializeField] private ButtonNetworkObject reRollButton;
    [SerializeField] private ButtonNetworkObject endTurnButton;
    
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
        endTurnButton.OnButtonPressed.AddListener(HandleEndTurn);
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
        StartCoroutine(nameof(RollDices));
    }

    private IEnumerator RollDices()
    {
        SaveDices();
        yield return new WaitForSeconds(0.3f);
        DisableButtons();
        
        if (rollDices.IsEmpty())
        {
            yield return new WaitForSeconds(0.3f);
            RestoreRollDices();
        }
        rollDices.Roll();
        if (rollDices.IsRollSuccessful()) yield break;
        yield return new WaitForSeconds(0.3f);
        savedDices.ResetScore();
        HandleEndTurn();
    }

    private void SetPlayerDices(Dice[] playerDices)
    {
        rollDices.FillDices(playerDices);
        chosenDices.SubscribeOnDiceChosen(playerDices);
    }

    [Command(requiresAuthority = false)]
    private void HandleEndTurn()
    {
        StartCoroutine(nameof(EndTurn));
    }

    private IEnumerator EndTurn()
    {
        SaveDices();
        savedDices.ClearDices();
        savedDices.SaveScore();
        UnChoseAllDices();
        yield return new WaitForSeconds(2);
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