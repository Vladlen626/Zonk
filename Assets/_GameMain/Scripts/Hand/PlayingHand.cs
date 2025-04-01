using System;
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
    
    public UnityAction OnTurnEnd;
    public UnityAction OnRollFinish;

    [Header("Params")] 
    [SerializeField] private float rollTime;
    
    [Header("Controllers")]
    [SerializeField] private SavedDiceController savedDices;
    [SerializeField] private RollDiceController rollDices;
    [SerializeField] private ChosenDiceController chosenDices;
    [SerializeField] private HandScoreController handScoreController;
    
    [Header("Objects")]
    [SerializeField] private ParticleSystem rollEffect;
    [SerializeField] private ButtonNetworkObject reRollButton;
    [SerializeField] private ButtonNetworkObject endTurnButton;
    
    [Command(requiresAuthority = false)]
    public void CmdSetOwner(Player player, int playerGeneralScore, Dice[] playerDices)
    {
        ownerNetId = player.netId;
        SetPlayerDices(playerDices);
        reRollButton.CmdSetOwner(ownerNetId);
        endTurnButton.CmdSetOwner(ownerNetId);
        handScoreController.SetPlayerName(player.playerName);
        handScoreController.ResetScore();
        handScoreController.GeneralScore = playerGeneralScore;
        savedDices.SetScoreController(handScoreController);
        chosenDices.SetScoreController(handScoreController);
        EnableButtons();
    }
    
    // _____________ Private _____________

    private void Start()
    {
        chosenDices.onScoreChanged.AddListener(HandleScoreChanged);
        reRollButton.OnButtonPressed.AddListener(HandleRollDices);
        endTurnButton.OnButtonPressed.AddListener(HandleEndTurn);
    }

    private void SaveDices()
    {
        if (handScoreController.ChosenScore <= 0) return;
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
        DisableButtons();
        yield return new WaitForSeconds(1f);
        if (rollDices.IsEmpty())
        {
            yield return new WaitForSeconds(0.3f);
            RestoreRollDices();
        }
        rollDices.Roll(rollTime);
        rollEffect.Play();
        AudioManager.inst.PlaySound(SoundNames.Lyghting);
        yield return new WaitForSeconds(rollTime * 0.95f);
        OnRollFinish?.Invoke();
        AudioManager.inst.PlaySound(SoundNames.MoveDice);
        rollEffect.Stop();
        if (rollDices.IsRollSuccessful()) yield break;
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
        DisableButtons();
        StartCoroutine(nameof(EndTurn));
    }

    private IEnumerator EndTurn()
    {
        SaveDices();
        savedDices.ClearDices();
        savedDices.SaveScore();
        GameManager.I.SavePlayerScore(netId, handScoreController.GeneralScore);
        UnChoseAllDices();
        yield return new WaitForSeconds(1f);
        OnTurnEnd?.Invoke();
    }

    [Command(requiresAuthority = false)]
    private void HandleScoreChanged()
    {
        if (handScoreController.ChosenScore <= 0)
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