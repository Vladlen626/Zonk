using DG.Tweening;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;


public class DiceVisualController : NetworkBehaviour
{
    [SerializeField] private MeshRenderer[] meshes;
    [SerializeField] private Outline outline;

    public void SetSideMesh(int sideValue)
    {
        Hide();
        if (sideValue == 0) return;
        Show(sideValue - 1);
    }

    public void UpdateChosenVisual(bool isChosen)
    {
        outline.OutlineColor = isChosen ? Color.red : Color.black;
    }

    private void Hide()
    {
        foreach (var meshRenderer in meshes)
        {
            meshRenderer.enabled = false;
        }
    }

    private void Show(int sideIdx)
    {
        meshes[sideIdx].enabled = true;
    }
    
    public void PlayRollAnimation()
    {
        DOTween.Sequence()
            .Append(transform.DOMove(transform.position + Vector3.up * 0.2f, 0.125f).SetEase(Ease.InOutQuad))
            .Join(transform.DORotate(Vector3.one * 180, 0.125f).SetEase(Ease.InOutQuad))
            .Append(transform.DORotate(Vector3.one * Random.Range(360, 720), 0.150f).SetEase(Ease.InOutQuad))
            .Append(transform.DOMove(transform.position, 0.100f).SetEase(Ease.InOutQuad))
            .Join(transform.DORotate(new Vector3(0f, Random.Range(0f, 360f), 0f), 0.100f).SetEase(Ease.InOutQuad));
    }

    public void MoveToSavePosition(Vector3 savePosition)
    {
        DOTween.Sequence()
            .Append(transform.DOJump(savePosition, 0.2f, 2, 0.5f).SetEase(Ease.InOutQuad))
            .Join(transform.DORotate(new Vector3(0f, Random.Range(0f, 360f), 0f), 0.5f).SetEase(Ease.InOutQuad));

    }
}