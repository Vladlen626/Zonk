using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using UnityEngine;

public class RollDiceController : NetworkBehaviour
{
    [SerializeField] private Transform[] rollingPoses;
    private List<Dice> rollDices = new List<Dice>();

    private readonly Vector2 ROLL_OFFSET = new Vector2(-0.02f, 0.02f);

    public void Roll()
    {
        for (var diceNum = 0; diceNum < rollDices.Count; diceNum++)
        {
            var dice = rollDices[diceNum];
            var randomOffset = new Vector3(Random.Range(ROLL_OFFSET.x, ROLL_OFFSET.y), 0,
                Random.Range(ROLL_OFFSET.x, ROLL_OFFSET.y));
            dice.transform.position = rollingPoses[diceNum].position + randomOffset;
            dice.Roll();
        }
    }

    public bool IsEmpty()
    {
        return rollDices.Count == 0;
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