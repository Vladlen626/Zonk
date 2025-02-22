using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Hand : MonoBehaviour
{
    [SerializeField] private Dice[] dicesDeck;
    [SerializeField] private TextMeshProUGUI pointsTMP;
    private List<Dice> handDices = new List<Dice>();
    private List<Dice> savedDices = new List<Dice>();
    private List<Dice> chosenDices = new List<Dice>();
    
    public void RollDices()
    {
        foreach (var dice in handDices)
        {
            dice.Roll();
        }

        if (Combinator.Instance.GetScore(handDices.ToArray(), true) <= 0)
        {
            Debug.Log("NoCombination");
        }
    }
    
    public void SaveDices()
    {
        savedDices.Clear();
        foreach (var chosenDice in chosenDices)
        {
            handDices.Remove(chosenDice);
            savedDices.Add(chosenDice);
        }
    }

    // _____________ Private _____________
    
    private void Start()
    {
        StartTurn();
    }

    private void StartTurn()
    {
        handDices.Clear();
        foreach (var dice in dicesDeck)
        {
            handDices.Add(dice);
            dice.onDiceChosen.AddListener(HandleNewChosenDice);
        }
    }
    
    private void HandleNewChosenDice(Dice newChosenDice)
    {
        if (chosenDices.Contains(newChosenDice))
        {
            chosenDices.Remove(newChosenDice);
            newChosenDice.UnChosen();
        } else
        {
            chosenDices.Add(newChosenDice);
            newChosenDice.Chosen();
        }
        
        if (chosenDices.Count > 0)
        {
            UpdateScore(Combinator.Instance.GetScore(chosenDices.ToArray()));
        }
    }

    private void UpdateScore(int score)
    {
        pointsTMP.text = score.ToString();
    }
    
}
