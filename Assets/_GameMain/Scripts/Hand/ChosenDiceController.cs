using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class ChosenDiceController : NetworkBehaviour
{
    [HideInInspector]
    public UnityEvent onScoreChanged = new UnityEvent();
    
    private List<Dice> chosenDices = new List<Dice>();
    private HandScoreController _handScoreController;

    public Dice[] GetDices()
    {
        return chosenDices.ToArray();
    }

    public void SubscribeOnDiceChosen(Dice[] diceDeck)
    {
        foreach (var dice in diceDeck)
        {
            dice.onDiceChosen += AddDiceToChosen;
            dice.onDiceUnChosen += RemoveDiceFromChosen;
        }
    }
    
    // _____________ Private _____________

    public void SetScoreController(HandScoreController inHandScoreController)
    {
        _handScoreController = inHandScoreController;
    }
    
    private void AddDiceToChosen(Dice newChosenDice)
    {
        chosenDices.Add(newChosenDice);
        UpdateChosenScore();
    }

    private void RemoveDiceFromChosen(Dice newChosenDice)
    {
        chosenDices.Remove(newChosenDice);
        UpdateChosenScore();
    }

    private void UpdateChosenScore()
    {
        _handScoreController.ChosenScore = chosenDices.Count > 0 ? Combinator.Instance.GetScore(chosenDices.ToArray()) : 0;
        onScoreChanged.Invoke();
    }
}