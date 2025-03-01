using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    [HideInInspector]
    public UnityEvent OnPlayerConnected = new UnityEvent();
    
    [SerializeField] private Transform[] handPositions;
    [SerializeField] private PlayingHand playingHand;
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
        playingHand.OnTurnEnd.AddListener(EndTurn);
    }
    
    [Server]
    public void AddPlayer(Player player)
    {
        if (players.Contains(player)) return;
        players.Add(player);
        DiceManager.Instance.CreatePlayerDices(player.diceTypes, player);

        if (players.Count == 1)
        {
            currentPlayerIndex = 0;
        }
        OnPlayerConnected.Invoke();
        UpdatePlayingHandOwner();
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
        players[currentPlayerIndex].TurnEnd();
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        UpdatePlayingHandOwner();
    }

    [Server]
    private void UpdatePlayingHandOwner()
    {
        playingHand.CmdSetOwner(players[currentPlayerIndex]);
        MovePlayingHand(currentPlayerIndex);
    }
    
    [ClientRpc]
    private void MovePlayingHand(int positionNum)
    {
        playingHand.transform.parent = handPositions[positionNum];
        playingHand.transform.localPosition = Vector3.zero;
        playingHand.transform.localRotation = Quaternion.identity;
    }
    
}