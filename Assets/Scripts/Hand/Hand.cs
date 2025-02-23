using System.Collections.Generic;
using UnityEngine;


public class Hand : MonoBehaviour
{
    [SerializeField] private Dice[] dicesDeck;
    [SerializeField] private ReRollCup reRollCup;

    private SavedDiceController savedDices;
    private RollDiceController rollDices;
    private ChosenDiceController chosenDices;
    
    public void RollDices()
    {
        if (savedDices.SaveDices(chosenDices.GetDices()))
            rollDices.RemoveDices(chosenDices.GetDices());
        chosenDices.ClearDices();
        
        if (!rollDices.RollDices()) return;
        EndTurn();
    }

    // _____________ Private _____________

    private void Start()
    {
        savedDices = GetComponent<SavedDiceController>();
        rollDices = GetComponent<RollDiceController>();
        chosenDices = GetComponent<ChosenDiceController>();
        StartTurn();
    }

    private void StartTurn()
    {
        rollDices.FillDices(dicesDeck);
        reRollCup.OnReRoll.AddListener(RollDices);
        chosenDices.SubscribeOnDiceChosen(dicesDeck);
    }

    private void EndTurn()
    {
        savedDices.ResetScore();
        savedDices.ClearDices();
        reRollCup.OnReRoll.RemoveAllListeners();
        chosenDices.ClearDices();
    }
    
   
}