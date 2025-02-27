using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Combinator : MonoBehaviour
{
    private static Combinator instance;
    public static Combinator Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindFirstObjectByType<Combinator>();

            if (instance != null) return instance;
            GameObject singletonObject = new GameObject("Combinator");
            instance = singletonObject.AddComponent<Combinator>();
            return instance;
        }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    private void Start()
    {
        for (var i = 0; i < diceContainers.Length; i++)
        {
            diceContainers[i] = new List<Dice>();
        }
    }

    private const int FIRST_CONTAINER = 0;
    private const int FIFTH_CONTAINER = 4;
    private const int MIN_IDENTICAL_DICES = 3;
    
    [SerializeField] private Combo[] combos;
    [SerializeField] private int oneMultiplier;
    [SerializeField] private int fiveMultiplier;
    [SerializeField] private int identicalMultiplier;
    [SerializeField] private int extraMultiplier;

    private List<Dice>[] diceContainers = new List<Dice>[6];
    
    public int GetScore(Dice[] dices, bool ignoreMistakes = false)
    {
        var score = 0;
        
        FillContainers(dices);
        score += GetCombinationScore();
        score += GetIdenticalDiceScore();
        score += GetBasicScore(FIRST_CONTAINER);
        score += GetBasicScore(FIFTH_CONTAINER);

        if (ignoreMistakes) return score;
        
        return diceContainers.Any(diceContainer => diceContainer.Count > 0) ? 0 : score;
    }
    

    private int GetCombinationScore()
    {
        foreach (var combo in combos)
        {
            var isCombo = true;
            
            foreach (var neededValue in combo.GetNeededValues())
            {
                if (diceContainers[neededValue - 1].Count == 0)
                    isCombo = false;
            }
            

            if (!isCombo) continue;
            foreach (var neededValue in combo.GetNeededValues())
            {
                diceContainers[neededValue - 1].RemoveAt(0);
            }

            return combo.GetScore();
        }

        return 0;
    }

    private int GetIdenticalDiceScore()
    {
        var score = 0;
        for (var containerNum = 0; containerNum < diceContainers.Length; containerNum++)
        {
            var count = diceContainers[containerNum].Count;
            if (count < MIN_IDENTICAL_DICES) continue;
            var multiplier = containerNum == FIRST_CONTAINER ? extraMultiplier : identicalMultiplier;
            score += (containerNum + 1) * multiplier * (int)Mathf.Pow(2, count - MIN_IDENTICAL_DICES);
            diceContainers[containerNum].Clear();
        }

        return score;
    }

    private int GetBasicScore(int containerNum)
    {
        var score = 0;
        var count = diceContainers[containerNum].Count;

        if (count <= 0) return score;
        var multiplier = containerNum == FIRST_CONTAINER ? oneMultiplier : fiveMultiplier;
        score += (containerNum + 1) * multiplier * count;
        diceContainers[containerNum].Clear();

        return score;
    }

    private void FillContainers(Dice[] dices)
    {
        ClearContainers();
        foreach (var dice in dices)
        {
            diceContainers[dice.GetCurrentSide().GetValue() - 1].Add(dice);
        }
    }

    private void ClearContainers()
    {
        foreach (var diceContainer in diceContainers)
        {
            diceContainer.Clear();
        }
    }
    
}
