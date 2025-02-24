using System.Collections.Generic;
using UnityEngine;


public class Hand : MonoBehaviour
{
    [SerializeField] private Dice[] dicesDeck;
    [SerializeField] private ReRollCup reRollCup;
    [SerializeField] private EndTurnButton endTurnButton;

    private SavedDiceController savedDices;
    private RollDiceController rollDices;
    private ChosenDiceController chosenDices;
    private ScoreController scoreController;

    private void Start()
    {
        Init();
        StartTurn();
    }

    private void Init()
    {
        savedDices = GetComponent<SavedDiceController>();
        rollDices = GetComponent<RollDiceController>();
        chosenDices = GetComponent<ChosenDiceController>();
        scoreController = GetComponent<ScoreController>();
    }

    private void SaveDices()
    {
        if (savedDices.SaveDices(chosenDices.GetDices()))
            rollDices.RemoveDices(chosenDices.GetDices());
    }
    
    private void HandleRollDices()
    {
        SaveDices();
        chosenDices.ClearDices();
        RollDices();
    }

    private void RollDices()
    {
        if (!rollDices.Roll()) return;
        savedDices.ResetScore();
        EndTurn();
    }

    private void StartTurn()
    {
        rollDices.FillDices(dicesDeck);
        chosenDices.SubscribeOnDiceChosen(dicesDeck);
        reRollCup.OnReRoll.AddListener(HandleRollDices);
        endTurnButton.OnTurnEnd.AddListener(EndTurn);
        RollDices();
    }
    
    private void EndTurn()
    {
        SaveDices();
        ClearDices();
        savedDices.SaveScore();
        reRollCup.OnReRoll.RemoveAllListeners();
        endTurnButton.OnTurnEnd.RemoveAllListeners();
        StartTurn();
    }

    private void ClearDices()
    {
        savedDices.ClearDices();
        chosenDices.ClearDices();   
        foreach (var dice in dicesDeck)
        {
            dice.onDiceChosen.RemoveAllListeners();
        }
    }
    
   
}