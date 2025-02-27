using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum DiceType
{
    Regular
}


public class DiceManager : MonoBehaviour
{
    private static DiceManager instance;
    public static DiceManager Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindFirstObjectByType<DiceManager>();

            if (instance != null) return instance;
            GameObject singletonObject = new GameObject("DiceManager");
            instance = singletonObject.AddComponent<DiceManager>();
            return instance;
        }
    }
    
    [SerializeField] private GameObject[] dicePrefabs;
    private Dictionary<DiceType, GameObject> dicePrefabsDictionary = new Dictionary<DiceType, GameObject>();
    
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
        foreach (var dicePrefab in dicePrefabs)
        {
            dicePrefabsDictionary.Add(dicePrefab.GetComponent<Dice>().type, dicePrefab);
        }
    }

    public Dice CreateDice(DiceType type)
    {
        var dice = Instantiate(dicePrefabsDictionary[type], transform, true);
        var diceComponent = dice.GetComponent<Dice>();
        diceComponent.Init();

        return diceComponent;
    }
}
