using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class GameManager : NetworkBehaviour
{
    public static GameManager I;
    public DiceManager diceManager;
    [HideInInspector] public UnityEvent OnPlayerConnected = new UnityEvent();

    [SerializeField] private int scoreGoal;
    [SerializeField] private Transform[] handPositions;
    [SerializeField] private PlayingHand playingHand;
    [SerializeField] private TextMeshPro scoreGoalTmp;

    private List<Player> players = new List<Player>();
    private Dictionary<uint, int> playersScoreDict = new Dictionary<uint, int>();

    private int currentPlayerIndex = 0;

    [Server]
    public void AddPlayer(Player player)
    {
        // Kostyl poka setki net, kak sdelayem settku na Unity Realy obyazatelno Popravlu!!!!!!!

        if (players.Contains(player)) return;
        players.Add(player);
        playersScoreDict.Add(player.netId, 0);
        player.SetName(players.Count);
        diceManager.CreatePlayerDices(player.diceTypes, player);

        if (players.Count == 1)
        {
            currentPlayerIndex = 0;
            UpdatePlayingHandOwner();
        }
        else if (players.Count == 2)
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
    public void SavePlayerScore(uint playerNetId, int score)
    {
        playersScoreDict[playerNetId] = score;
    }

    [Server]
    public Player GetCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    // _____________ Private _____________

    private void Awake()
    {
        if (I == null)
        {
            I = this;
        }
        else
        {
            Destroy(gameObject);
        }

        scoreGoalTmp.text = "Goal: " + scoreGoal;
        playingHand.OnTurnEnd.AddListener(EndTurn);
    }

    [Server]
    private void EndTurn()
    {
        CleanUpPlayerDices(GetCurrentPlayer().netId);
        if (CheckFinishGameCondition()) return;
        StartCoroutine(nameof(NextTurn));
    }

    [Server]
    private IEnumerator NextTurn()
    {
        SetPlayersToDefaultView();
        yield return new WaitForSeconds(0.35f);
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        UpdatePlayingHandOwner();
        MovePlayingHand(currentPlayerIndex);
    }

    [Server]
    private void SetPlayersToDefaultView()
    {
        foreach (var player in players)
        {
            player.GetViewManager().ResetViewToDefault();
        }
    }

    [Server]
    private bool CheckFinishGameCondition()
    {
        if (playersScoreDict[GetCurrentPlayer().netId] < scoreGoal) return false;
        RpcAnnouncePlayerWin();
        return true;
    }


    [Server]
    private void UpdatePlayingHandOwner()
    {
        var player = GetCurrentPlayer();
        playingHand.CmdSetOwner(player, playersScoreDict[player.netId], diceManager.GetPlayerDices(player.netId));
    }

    [Server]
    private void CleanUpPlayerDices(uint playerNetId)
    {
        foreach (var playerDice in diceManager.GetPlayerDices(playerNetId))
        {
            playerDice.Hide();
            playerDice.onDiceChosen.RemoveAllListeners();
            playerDice.onDiceUnChosen.RemoveAllListeners();
        }
    }

    [ClientRpc]
    private void RpcAnnouncePlayerWin()
    {
        scoreGoalTmp.text = "Game finish, player " + currentPlayerIndex + " win";
        playingHand.transform.localScale = Vector3.zero;
    }

    [ClientRpc]
    private void MovePlayingHand(int positionNum)
    {
        playingHand.transform.DOMove(handPositions[positionNum].position, 0.5f);
        playingHand.transform.DORotate(handPositions[positionNum].rotation.eulerAngles, 0.5f);
    }
}