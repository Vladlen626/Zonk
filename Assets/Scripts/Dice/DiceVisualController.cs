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
    
    public void SetSideMesh(int sideValue)
    {
        foreach (var meshRenderer in meshes)
        {
            meshRenderer.enabled = false;
        }

        meshes[sideValue - 1].enabled = true;
        
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

    private void OnMouseDown()
    {
        transform.localScale = Vector3.one * 0.9f;
    }

    private void OnMouseUp()
    {
        transform.localScale = Vector3.one * 1f;
    }
}