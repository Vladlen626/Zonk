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
    private ScoreController scoreController;

    public void SaveDices(Dice[] chosenDices)
    {
        var newSavedDices = chosenDices.ToList();

        foreach (var newSavedDice in newSavedDices)
        {
            SaveDice(newSavedDice);
        }

        scoreController.SavedScore += Combinator.Instance.GetScore(newSavedDices.ToArray());
    }

    public void ResetScore()
    {
        scoreController.SavedScore = 0;
    }

    public void SaveScore()
    {
        scoreController.GeneralScore += scoreController.SavedScore;
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

    public void SetScoreController(ScoreController inScoreController)
    {
        scoreController = inScoreController;
    }

    private void SaveDice(Dice dice)
    {
        savedDices.Add(dice);
        dice.UnChose();
        dice.Save(dicePoses[savedDices.Count - 1].position);
    }
}