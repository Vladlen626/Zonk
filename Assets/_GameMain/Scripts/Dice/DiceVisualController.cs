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

    public void RandomizeRotation()
    {
        var randomAngle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, randomAngle, 0f);
    }
}