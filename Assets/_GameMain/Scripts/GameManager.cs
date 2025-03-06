using System;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [HideInInspector]
    public UnityEvent OnPlayerConnected = new UnityEvent();

    [SerializeField] private int scoreGoal;
    [SerializeField] private Transform[] handPositions;
    [SerializeField] private PlayingHand playingHand;
    [SerializeField] private TextMeshPro scoreGoalTmp;
    
    private List<Player> players = new List<Player>();
    
    private int currentPlayerIndex = 0;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        scoreGoalTmp.text = "Goal: " + scoreGoal;
        playingHand.OnTurnEnd.AddListener(EndTurn);
    }
    
    [Server]
    public void AddPlayer(Player player)
    {
        if (players.Contains(player)) return;
        player.SetName(players.Count);
        players.Add(player);
        DiceManager.Instance.CreatePlayerDices(player.diceTypes, player);

        if (players.Count == 1)
        {
            currentPlayerIndex = 0;
            UpdatePlayingHandOwner();
        } else if (players.Count == 2)
        {
            MovePlayingHand(currentPlayerIndex);
        }
        OnPlayerConnected.Invoke();
    }

    [Server]
    public void RemovePlayer(Player player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
        }
    }
    
    [Server]
    private void EndTurn()
    {
        if (players.Count < 2) return;
        CleanUpPlayerDices(GetCurrentPlayer().netId);
        if (CheckFinishGameCondition()) return;
        NextTurn();
    }

    [Server]
    private void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        UpdatePlayingHandOwner();
        MovePlayingHand(currentPlayerIndex);
    }

    [Server]
    private bool CheckFinishGameCondition()
    {
        if (GetCurrentPlayer().GetScoreController().GeneralScore < scoreGoal) return false;
        RpcAnnouncePlayerWin();
        return true;
    }

    [ClientRpc]
    private void RpcAnnouncePlayerWin()
    {
        scoreGoalTmp.text = "Game finish, player " + currentPlayerIndex + " win";
        playingHand.transform.localScale = Vector3.zero;
    }

    [Server]
    private Player GetCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    [Server]
    private void UpdatePlayingHandOwner()
    {
        playingHand.CmdSetOwner(players[currentPlayerIndex]);
    }

    [Server]
    private void CleanUpPlayerDices(uint playerNetId)
    {
        foreach (var playerDice in DiceManager.Instance.GetPlayerDices(playerNetId))
        {
            playerDice.Hide();
            playerDice.onDiceChosen.RemoveAllListeners();
            playerDice.onDiceUnChosen.RemoveAllListeners();
        }
    }
    
    [ClientRpc]
    private void MovePlayingHand(int positionNum)
    {
        playingHand.transform.parent = handPositions[positionNum];
        playingHand.transform.localPosition = Vector3.zero;
        playingHand.transform.localRotation = Quaternion.identity;
    }
    
    
}