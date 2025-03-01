using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


public class PlayingHand : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnOwnerChanged))]
    private uint ownerNetId; 
    public bool IsOwner => NetworkClient.connection != null && NetworkClient.connection.identity.netId == ownerNetId;

    [HideInInspector]
    public UnityEvent OnTurnEnd;
    [SerializeField] private ReRollCup reRollCup;
    [SerializeField] private EndTurnButton endTurnButton;
    
    private SavedDiceController savedDices;
    private RollDiceController rollDices;
    private ChosenDiceController chosenDices;
    private ScoreController scoreController;

    [Command(requiresAuthority = false)]
    public void CmdSetOwner(Player player)
    {
        ownerNetId = player.netId;
        scoreController = player.GetScoreController();
        savedDices.SetScoreController(scoreController);
        chosenDices.SetScoreController(scoreController);
    }

    private void OnOwnerChanged(uint oldOwnerNetId, uint newOwnerNetId)
    {
        SetPlayerDices(DiceManager.Instance.GetPlayerDices(newOwnerNetId));
        reRollCup.CmdSetOwner(newOwnerNetId);
        endTurnButton.CmdSetOwner(newOwnerNetId);
    }
    
    private void Start()
    {
        Init();
        chosenDices.onScoreChanged.AddListener(HandleScoreChanged);
        reRollCup.OnReRoll.AddListener(HandleRollDices);
        endTurnButton.OnTurnEnd.AddListener(EndTurn);
    }

    private void Init()
    {
        savedDices = GetComponent<SavedDiceController>();
        rollDices = GetComponent<RollDiceController>();
        chosenDices = GetComponent<ChosenDiceController>();
    }

    private void SaveDices()
    {
        if (scoreController.ChosenScore <= 0) return;
        savedDices.SaveDices(chosenDices.GetDices());
        rollDices.RemoveDices(chosenDices.GetDices());
    }
    
    [Command(requiresAuthority = false)]
    private void HandleRollDices()
    {
        SaveDices();
        chosenDices.ClearDices();
        DisableButtons();
        RollDices();
    }
    
    private void RollDices()
    {
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
    
    private void EndTurn()
    {
        SaveDices();
        ClearAllDices();
        DisableButtons();
        savedDices.SaveScore();
        OnTurnEnd.Invoke();
    }

    private void HandleScoreChanged()
    {
       if (scoreController.ChosenScore <= 0)
           DisableButtons();
       else
           EnableButtons();
    }

    private void DisableButtons()
    {
        reRollCup.Disable();
        endTurnButton.Disable();
    }

    private void EnableButtons()
    {
        reRollCup.Enable();
        endTurnButton.Enable();
    }

    private void ClearAllDices()
    {
        savedDices.ClearDices();
        chosenDices.ClearDices();
    }
    
   
}