using DG.Tweening;
using Mirror;
using TMPro;
using UnityEngine;


public class HandScoreController : NetworkBehaviour
{ 
    [SerializeField] private TextMeshPro chosenScoreTMP;
    [SerializeField] private TextMeshPro savedScoreTMP;
    [SerializeField] private TextMeshPro generalScoreTMP;
    [SerializeField] private TextMeshPro playerNameTmp;
    
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
            UpdateUiScoreText(chosenScoreTMP, chosenScore, value);
            chosenScore = value;
        }
    }
    public int SavedScore
    {
        get => savedScore;
        set
        {
            UpdateUiScoreText(savedScoreTMP, savedScore, value);
            savedScore = value;
        }
    }
    public int GeneralScore
    {
        get => generalScore;
        set
        {
            UpdateUiScoreText(generalScoreTMP, generalScore, value);
            generalScore = value;
        }
    }

    public void ResetScore()
    {
        ChosenScore = 0;
        SavedScore = 0;
        GeneralScore = 0;
    }

    [ClientRpc]
    public void SetPlayerName(string name)
    {
        playerNameTmp.text = name;
    }

    private void UpdateUiScoreText(TextMeshPro scoreTmp, int oldScore, int newScore)
    {
        DOTween.To(() => oldScore, x => oldScore = x, newScore, 0.25f)
            .OnUpdate(() =>  scoreTmp.text = oldScore.ToString())
            .SetEase(Ease.Linear);
       ;
    }

    private void OnChosenScoreChange(int oldValue, int newValue)
    {
        UpdateUiScoreText(chosenScoreTMP, oldValue,newValue);
    }

    private void OnSavedScoreChange(int oldValue, int newValue)
    {
        UpdateUiScoreText(savedScoreTMP, oldValue,newValue);
    }

    private void OnGeneralScoreChange(int oldValue, int newValue)
    {
        UpdateUiScoreText(generalScoreTMP, oldValue,newValue);
    }
    
}
