using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class SavedDiceController : MonoBehaviour
{
    [SerializeField] private Transform[] dicePoses;

    private List<Dice> savedDices = new List<Dice>();
    private HandScoreController _handScoreController;

    public void SaveDices(Dice[] chosenDices)
    {
        var newSavedDices = chosenDices.ToList();

        foreach (var newSavedDice in newSavedDices)
        {
            SaveDice(newSavedDice);
        }

        _handScoreController.SavedScore += Combinator.Instance.GetScore(newSavedDices.ToArray());
    }

    public void ResetScore()
    {
        _handScoreController.SavedScore = 0;
    }

    public void SaveScore()
    {
        _handScoreController.GeneralScore += _handScoreController.SavedScore;
        ResetScore();
    }

    public Dice[] GetSavedDices()
    {
        return savedDices.ToArray();
    }

    public void ClearDices()
    {
        foreach (var savedDice in savedDices)
        {
            savedDice.UnSave();
        }

        savedDices.Clear();
    }

    // _____________ Private _____________

    public void SetScoreController(HandScoreController inHandScoreController)
    {
        _handScoreController = inHandScoreController;
    }

    private void SaveDice(Dice dice)
    {
        savedDices.Add(dice);
        dice.UnChose();
        dice.Save(dicePoses[savedDices.Count - 1].position);
    }
}