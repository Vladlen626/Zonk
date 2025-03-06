using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public DiceType[] diceTypes;
    
    private ScoreController scoreController;

    [SerializeField] private TextMeshPro nameTmp;
    
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

    public void SetName(int playerIdx)
    {
        nameTmp.text = "Player: " + playerIdx;
    }
    
}
