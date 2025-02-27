using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DiceVisualController : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] meshes;
    
    private RectTransform rectTransform;
    private Outline outline;
    private MeshRenderer currentMeshRenderer;
    
    public void SetSideMesh(int sideValue)
    {
        Hide();
        currentMeshRenderer = meshes[sideValue - 1];
        Show();
        
        RandomizeRotation();
    }
    
    public void ChosenColor()
    {
        outline.OutlineColor = Color.green;
    }

    public void DefaultColor()
    {
        outline.OutlineColor = Color.black;
    }

    public void Hide()
    {
        foreach (var meshRenderer in meshes)
        {
            meshRenderer.enabled = false;
        }
    }

    public void Show()
    {
        currentMeshRenderer.enabled = true;
    }

    public void Handle()
    {
        transform.localScale = Vector3.one * 0.9f;
    }

    public void Throw()
    {
        transform.localScale = Vector3.one;
    }

    // _____________ Private _____________
    
    private void Start()
    {
        outline = GetComponent<Outline>();
    }
    
    private void RandomizeRotation()
    {
        var randomAngle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, randomAngle, 0f);
    }
  
}