using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RollDiceController : NetworkBehaviour
{
    [SerializeField] private Transform[] rollingPoses;
    private List<Dice> rollDices = new List<Dice>();
    
    public void Roll()
    {
        for (var diceNum = 0; diceNum < rollDices.Count; diceNum++)
        {
            var dice = rollDices[diceNum];
            dice.transform.position = rollingPoses[diceNum].position;
            dice.Roll();
        }
    }

    public bool IsRollSuccessful()
    {
        return Combinator.Instance.GetScore(rollDices.ToArray(), true) > 0;
    }
    
    public void FillDices(Dice[] dicesDeck)
    {
        rollDices.Clear();
        foreach (var dice in dicesDeck)
        {
            rollDices.Add(dice);
        }
    }

    public void RemoveDices(Dice[] dicesToRemove)
    {
        foreach (var dice in dicesToRemove)
        {
            rollDices.Remove(dice);
        }
    }
}