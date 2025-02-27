using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class Hand : MonoBehaviour
{
    [SerializeField] private DiceType[] diceTypes;
    [SerializeField] private ReRollCup reRollCup;
    [SerializeField] private EndTurnButton endTurnButton;
    
    private List<Dice> dicesDeck = new List<Dice>();
    private SavedDiceController savedDices;
    private RollDiceController rollDices;
    private ChosenDiceController chosenDices;
    private ScoreController scoreController;

    private void Start()
    {
        Init();
        chosenDices.onScoreChanged.AddListener(HandleScoreChanged);
        reRollCup.OnReRoll.AddListener(HandleRollDices);
        endTurnButton.OnTurnEnd.AddListener(EndTurn);
        StartTurn();
    }

    private void Init()
    {
        savedDices = GetComponent<SavedDiceController>();
        rollDices = GetComponent<RollDiceController>();
        chosenDices = GetComponent<ChosenDiceController>();
        scoreController = GetComponent<ScoreController>();
        
        foreach (var diceType in diceTypes)
        {
            dicesDeck.Add(DiceManager.Instance.CreateDice(diceType));
        }
    }

    private void SaveDices()
    {
        if (scoreController.ChosenScore <= 0) return;
        savedDices.SaveDices(chosenDices.GetDices());
        rollDices.RemoveDices(chosenDices.GetDices());
    }
    
    private void HandleRollDices()
    {
        SaveDices();
        chosenDices.ClearDices();
        DisableButtons();
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
        rollDices.FillDices(dicesDeck.ToArray());
        chosenDices.SubscribeOnDiceChosen(dicesDeck.ToArray());
        EnableButtons();
    }
    
    private void EndTurn()
    {
        SaveDices();
        ClearAllDices();
        DisableButtons();
        savedDices.SaveScore();
        StartTurn();
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
        foreach (var dice in dicesDeck)
        {
            dice.onDiceChosen.RemoveAllListeners();
            dice.Hide();
        }
    }
    
   
}