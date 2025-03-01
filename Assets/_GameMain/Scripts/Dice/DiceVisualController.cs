using System;
using DG.Tweening;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class DiceVisualController : NetworkBehaviour
{
    [SerializeField] private MeshRenderer[] meshes;
    [SerializeField] private Outline outline;
    
    public void SetSideMesh(int sideValue)
    {
        Hide();
        Show(sideValue - 1);
    }
    
    public void UpdateChosenVisual(bool isChosen)
    {
        outline.OutlineColor = isChosen ? Color.green : Color.black;
    }
    
    public void Hide()
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
    
    [ClientRpc]
    public void RpcHandle()
    {
        transform.localScale = Vector3.one * 0.9f;
    }
    
    [ClientRpc]
    public void RpcThrow()
    {
        transform.localScale = Vector3.one;
    }
    
    public void RandomizeRotation()
    {
        var randomAngle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, randomAngle, 0f);
    }
  
}