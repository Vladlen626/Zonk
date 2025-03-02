using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public DiceType[] diceTypes;
    
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

    public ScoreController GetScoreController()
    {
        return scoreController;
    }
    
}
