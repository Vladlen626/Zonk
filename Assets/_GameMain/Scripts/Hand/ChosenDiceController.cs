using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChosenDiceController : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent onScoreChanged = new UnityEvent();
    
    private List<Dice> chosenDices = new List<Dice>();
    private ScoreController scoreController;

    public Dice[] GetDices()
    {
        return chosenDices.ToArray();
    }

    public void SubscribeOnDiceChosen(Dice[] diceDeck)
    {
        foreach (var dice in diceDeck)
        {
            dice.onDiceChosen.AddListener(HandleNewChosenDice);
        }
    }

    public void ClearDices()
    {
        var chosenDicesForClear = chosenDices.ToArray();
        foreach (var dice in chosenDicesForClear)
        {
            RemoveDiceFromChosen(dice);
        }

        UpdateChosenScore();
    }

    // _____________ Private _____________

    private void Start()
    {
        scoreController = GetComponent<ScoreController>();
    }

    private void HandleNewChosenDice(Dice newChosenDice)
    {
        if (chosenDices.Contains(newChosenDice))
            RemoveDiceFromChosen(newChosenDice);
        else
            AddDiceToChosen(newChosenDice);
        
        UpdateChosenScore();
    }
    
    private void AddDiceToChosen(Dice newChosenDice)
    {
        chosenDices.Add(newChosenDice);
        newChosenDice.Chosen();
    }

    private void RemoveDiceFromChosen(Dice newChosenDice)
    {
        chosenDices.Remove(newChosenDice);
        newChosenDice.UnChosen();
    }

    private void UpdateChosenScore()
    {
        scoreController.ChosenScore = chosenDices.Count > 0 ? Combinator.Instance.GetScore(chosenDices.ToArray()) : 0;
        onScoreChanged.Invoke();
    }
}