using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Dice : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Dice> onDiceChosen;

    [SerializeField] private Side[] sides;

    private DiceVisualController diceVisualController;
    private Side currentSide;
    private bool isChosen;

    public Dice(bool isChosen)
    {
        this.isChosen = isChosen;
    }

    public void Roll()
    {
        SetSide(Random.Range(0, sides.Length));
        isChosen = false;
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

    private void Start()
    {
        diceVisualController = GetComponentInChildren<DiceVisualController>();
        Roll();
    }

    private void SetSide(int sideNum)
    {
        currentSide = sides[sideNum];
        diceVisualController.SetSideMesh(currentSide.GetValue());
    }

    private void OnMouseUp()
    {
        onDiceChosen?.Invoke(this);
    }
}