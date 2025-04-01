using DG.Tweening;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;


public class DiceVisualController : NetworkBehaviour
{
    [SerializeField] private ParticleSystem bloodEffect;
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
        outline.OutlineColor = isChosen ? Color.red : Color.black;
    }
    
    
    [Command]
    public void Pressed()
    {
        RpcPressed();
    }

    [Command]
    public void Released()
    {
        RpcPlayEffect();
        RpcReleased();
    }
    [ClientRpc]
    public void RpcPlayEffect()
    {
        bloodEffect.Play();
    }

    // _____________ Private _____________

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
    
    private void RpcLevitate()
    {
        model.DOLocalMove(Vector3.up * yOffset, animSpeed);
    }
    
    private void RpcGrounded()
    {
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