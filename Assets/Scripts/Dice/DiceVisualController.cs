using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DiceVisualController : MonoBehaviour
{
    [SerializeField] private Sprite[] sideSprites;

    private Image spriteImage;
    private RectTransform rectTransform;
    
    public void SetSideSprite(int sideSpriteNum)
    {
        spriteImage.sprite = sideSprites[sideSpriteNum - 1];
        RandomizeRotation();
    }
    
    public void ChosenPlaceHolder()
    {
        spriteImage.color = Color.cyan;
    }

    public void DefaultColor()
    {
        spriteImage.color = Color.white;
    }

    // _____________ Private _____________
    
    private void Start()
    {
        spriteImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }
    
    private void RandomizeRotation()
    {
        var randomAngle = Random.Range(0f, 360f);
        rectTransform.rotation = Quaternion.Euler(0f, 0f, randomAngle);
    }
    
}