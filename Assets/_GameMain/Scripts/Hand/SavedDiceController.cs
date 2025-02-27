using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public void ClearDices()
    {
        savedDices.Clear();
    }

    // _____________ Private _____________

    private void Start()
    {
        scoreController = GetComponent<ScoreController>();
    }

    private void SaveDice(Dice dice)
    {
        savedDices.Add(dice);
        dice.transform.position = dicePoses[savedDices.Count - 1].position;
    }
}