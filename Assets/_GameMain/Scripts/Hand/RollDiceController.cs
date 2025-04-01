using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using UnityEngine;

public class RollDiceController : NetworkBehaviour
{
    [SerializeField] private Transform[] rollingPoses;
    private List<Dice> rollDices = new List<Dice>();

    private readonly Vector2 ROLL_OFFSET = new Vector2(-0.02f, 0.02f);

    public void Roll(float rollTime)
    {
        for (var diceNum = 0; diceNum < rollDices.Count; diceNum++)
        {
            var dice = rollDices[diceNum];
            var randomOffset = new Vector3(Random.Range(ROLL_OFFSET.x, ROLL_OFFSET.y), 0,
                Random.Range(ROLL_OFFSET.x, ROLL_OFFSET.y));
            var moveDuration = 0.1f;
            dice.transform.DOMove(rollingPoses[diceNum].position + randomOffset, moveDuration)
                .OnComplete(() =>
                {
                    dice.Roll();
                    PlayRollAnimation(dice, rollTime - moveDuration);
                });
        }
    }
    
    public void PlayRollAnimation(Dice dice, float rollTime)
    {
        dice.SerRollState(true);
        var rollTimePoint = rollTime / 10;
        var rollingTime = rollTimePoint * 8;
        var diceTransform = dice.transform;
        DOTween.Sequence()
            .Append(diceTransform.DOMove(diceTransform.position + Vector3.up * 0.2f, rollTimePoint).SetEase(Ease.Linear))
            .Join(diceTransform.DORotate(Vector3.one * 180, rollTimePoint).SetEase(Ease.Linear))
            .Append(diceTransform.DORotate(Vector3.one * Random.Range(360, 720) * 5, rollingTime, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad))
            .Append(diceTransform.DOMove(diceTransform.position, rollTimePoint).SetEase(Ease.Linear))
            .Join(diceTransform.DORotate(new Vector3(0f, Random.Range(0f, 360f), 0f), rollTimePoint).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                dice.SerRollState(false);
            });
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