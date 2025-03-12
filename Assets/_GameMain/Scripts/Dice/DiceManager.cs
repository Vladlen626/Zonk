using System;
using System.Collections.Generic;
using Mirror;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public enum DiceType
{
    Regular
}

public class DiceManager : NetworkBehaviour
{

    [SerializeField] private GameObject[] dicePrefabs;
    private Dictionary<DiceType, GameObject> dicePrefabsDictionary = new Dictionary<DiceType, GameObject>();
    private Dictionary<uint, List<Dice>> dicesCollection = new Dictionary<uint, List<Dice>>();

    private void Start()
    {
        foreach (var dicePrefab in dicePrefabs)
        {
            dicePrefabsDictionary.Add(dicePrefab.GetComponent<Dice>().type, dicePrefab);
        }
    }
    
    [Server]
    public void CreatePlayerDices(DiceType[] types, Player player)
    {
        var playerDices = new List<Dice>();
        foreach (var diceType in types)
        {
            var dice = Instantiate(dicePrefabsDictionary[diceType], Vector3.one * 1000, Quaternion.identity);
            var diceComponent = dice.GetComponent<Dice>();
            NetworkServer.Spawn(dice);
            diceComponent.CmdSetOwner(player.netId);
            dice.GetComponent<NetworkIdentity>().AssignClientAuthority(player.connectionToClient);
            playerDices.Add(diceComponent);
        }
        
        dicesCollection.Add(player.netId, playerDices);
    }

    [Server]
    public void RemovePlayerDices(uint playerNetId)
    {
        dicesCollection.Remove(playerNetId);
    }

    [Server]
    public Dice[] GetPlayerDices(uint playerNetId)
    {
        return dicesCollection[playerNetId].ToArray();
    }
}