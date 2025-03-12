using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [HideInInspector] public string name;
    public DiceType[] diceTypes;

    [SerializeField] private ViewManager viewManager;
    
    // ReSharper disable Unity.PerformanceAnalysis
    public override void OnStartServer()
    {
        base.OnStartServer();
        GameManager.I.AddPlayer(this);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void OnStopServer()
    {
        base.OnStopServer();
        GameManager.I.diceManager.RemovePlayerDices(netId);
        GameManager.I.RemovePlayer(this);
    }

    public ViewManager GetViewManager()
    {
        return viewManager;
    }

    public void SetName(int playerIdx)
    {
        name = "Player " + playerIdx;
    }
    
}
