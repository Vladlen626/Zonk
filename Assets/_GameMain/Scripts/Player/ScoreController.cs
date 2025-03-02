using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class ScoreController : NetworkBehaviour
{ 
    [SerializeField] private TextMeshPro chosenScoreTMP;
    [SerializeField] private TextMeshPro savedScoreTMP;
    [SerializeField] private TextMeshPro generalScoreTMP;
    
    [SyncVar(hook = nameof(OnChosenScoreChange))]
    private int chosenScore;
    [SyncVar(hook = nameof(OnSavedScoreChange))]
    private int savedScore;
    [SyncVar(hook = nameof(OnGeneralScoreChange))]
    private int generalScore;

    public int ChosenScore
    {
        get => chosenScore;
        set
        {
            chosenScore = value;
            UpdateUiScoreText(chosenScoreTMP, chosenScore);
        }
    }
    public int SavedScore
    {
        get => savedScore;
        set
        {
            savedScore = value;
            UpdateUiScoreText(savedScoreTMP, savedScore);
        }
    }
    public int GeneralScore
    {
        get => generalScore;
        set
        {
            generalScore = value;
            UpdateUiScoreText(generalScoreTMP, generalScore);
        }
    }

    private void UpdateUiScoreText(TextMeshPro scoreTmp, int score)
    {
        scoreTmp.text = score.ToString();
    }

    private void OnChosenScoreChange(int oldValue, int newValue)
    {
        UpdateUiScoreText(chosenScoreTMP, newValue);
    }

    private void OnSavedScoreChange(int oldValue, int newValue)
    {
        UpdateUiScoreText(savedScoreTMP, newValue);
    }

    private void OnGeneralScoreChange(int oldValue, int newValue)
    {
        UpdateUiScoreText(generalScoreTMP, newValue);
    }
    
}
