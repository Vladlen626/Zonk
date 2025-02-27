using System;
using System.Runtime.InteropServices;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Dice : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Dice> onDiceChosen;
    [SerializeField] private Side[] sides;

    public DiceType type;
    private DiceVisualController diceVisualController;
    private BoxCollider boxCollider;
    private Side currentSide;
    private bool isChosen;
    
    public Dice(bool isChosen)
    {
        this.isChosen = isChosen;
    }

    public void Init()
    {
        diceVisualController = GetComponent<DiceVisualController>();
        boxCollider = GetComponent<BoxCollider>();
        Hide();
    }

    public void Roll()
    {
        boxCollider.enabled = true;
        SetSide(Random.Range(0, sides.Length));
        diceVisualController.Show();
    }

    public void Hide()
    {
        diceVisualController.Hide();
        GetComponent<Collider>().enabled = false;
    }

    public Side GetCurrentSide()
    {
        return currentSide;
    }

    public void Chosen()
    {
        isChosen = true;
        diceVisualController.ChosenColor();
    }

    public void UnChosen()
    {
        isChosen = false;
        diceVisualController.DefaultColor();
    }

    // _____________ Private _____________


    private void SetSide(int sideNum)
    {
        currentSide = sides[sideNum];
        diceVisualController.SetSideMesh(currentSide.GetValue());
    }
    
   
    private void OnMouseUp()
    {
        ChoseDice();
    }
    private void OnMouseDown()
    {
        TouchDice();
    }
    
    private void ChoseDice()
    {
        onDiceChosen?.Invoke(this);
        diceVisualController.Throw();
    }
    private void TouchDice()
    {
        diceVisualController.Handle();
    }
    
    
}