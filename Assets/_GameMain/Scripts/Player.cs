using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public DiceType[] diceTypes;
    private List<Dice> dicesDeck = new List<Dice>();
    
    private ScoreController scoreController;
    
    // ReSharper disable Unity.PerformanceAnalysis
    public override void OnStartServer()
    {
        base.OnStartServer();
        
        scoreController = GetComponent<ScoreController>();
        GameManager.Instance.AddPlayer(this);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void OnStopServer()
    {
        base.OnStopServer();
        DiceManager.Instance.RemovePlayerDices(netId);
        GameManager.Instance.RemovePlayer(this);
    }

    public Dice[] GetDicesDeck()
    {
        return dicesDeck.ToArray();
    }

    public ScoreController GetScoreController()
    {
        return scoreController;
    }

    [Command]
    public void TurnEnd()
    {
        foreach (var dice in dicesDeck)
        {
            dice.onDiceChosen.RemoveAllListeners();
            dice.Hide();
        }
    }
    
}
