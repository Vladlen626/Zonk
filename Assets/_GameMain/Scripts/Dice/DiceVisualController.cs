using DG.Tweening;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;


public class DiceVisualController : NetworkBehaviour
{
    [SerializeField] private MeshRenderer[] meshes;
    [SerializeField] private Outline outline;
    [SerializeField] private float animSpeed = 0.15f;
    [SerializeField] private float yOffset = 0.02f;
    [SerializeField] private Transform model;

    private readonly float MOVE_TIME = 0.25f;
    private bool isRolling;
    public void SetSideMesh(int sideValue)
    {
        Hide();
        if (sideValue == 0) return;
        Show(sideValue - 1);
    }

    public void UpdateChosenVisual(bool isChosen)
    {
        if (isChosen)
        {
            RpcLevitate();
        }
        else
        {
            RpcGrounded();
        }
        outline.OutlineColor = isChosen ? Color.green : Color.black;
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
        AudioManager.inst.PlaySound(SoundNames.DiceRoll);
        isRolling = true;
        DOTween.Sequence()
            .Append(transform.DOMove(transform.position + Vector3.up * 0.2f, MOVE_TIME * 2).SetEase(Ease.Linear))
            .Join(transform.DORotate(Vector3.one * 180, MOVE_TIME * 2).SetEase(Ease.Linear))
            .Append(transform.DORotate(Vector3.one * Random.Range(360, 720) * 5, MOVE_TIME * 8, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad))
            .Append(transform.DOMove(transform.position, MOVE_TIME/2).SetEase(Ease.Linear))
            .Join(transform.DORotate(new Vector3(0f, Random.Range(0f, 360f), 0f), MOVE_TIME/2).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                AudioManager.inst.PlaySound(SoundNames.DiceRoll);
                isRolling = false;
            });
    }

    public void MoveToSavePosition(Vector3 savePosition)
    {
        AudioManager.inst.PlaySound(SoundNames.MoveDice);
        DOTween.Sequence()
            .Append(transform.DOJump(savePosition, 0.2f, 2, MOVE_TIME).SetEase(Ease.InOutQuad))
            .Join(transform.DORotate(new Vector3(0f, Random.Range(0f, 360f), 0f), MOVE_TIME).SetEase(Ease.InOutQuad));

    }
    
    [Command]
    public void OnHover()
    {
        //RpcLevitate();
    }
    
    [Command]
    public void OnUnHover()
    {
        //RpcGrounded();
    }

    [Command]
    public void Pressed()
    {
        RpcPressed();
    }

    [Command]
    public void Released()
    {
        RpcReleased();
    }
    
    private void RpcLevitate()
    {
        if (isRolling) return;
        model.DOLocalMove(Vector3.up * yOffset, animSpeed);
    }
    
    private void RpcGrounded()
    {
        if (isRolling) return;
        model.DOLocalMove(Vector3.zero , animSpeed);
    }

    [ClientRpc]
    private void RpcPressed()
    {
        transform.DOScale(0.9f, animSpeed);
    }

    [ClientRpc]
    private void RpcReleased()
    {
        transform.DOScale(1f, animSpeed);
    }
}