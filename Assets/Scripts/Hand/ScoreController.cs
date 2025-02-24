using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ScoreController : MonoBehaviour
{ 
    [SerializeField] private TextMeshPro chosenScoreTMP;
    [SerializeField] private TextMeshPro savedScoreTMP;
    [SerializeField] private TextMeshPro generalScoreTMP;
    
    private int chosenScore;
    private int savedScore;
    private int generalScore;

    public int ChosenScore
    {
        get => chosenScore;
        set
        {
            chosenScore = value;
            chosenScoreTMP.text = chosenScore.ToString(); 
        }
    }
    public int SavedScore
    {
        get => savedScore;
        set
        {
            savedScore = value;
            savedScoreTMP.text = savedScore.ToString();
        }
    }
    public int GeneralScore
    {
        get => generalScore;
        set
        {
            generalScore = value;
            generalScoreTMP.text = generalScore.ToString();
        }
    }
    
}
