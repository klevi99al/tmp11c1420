using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int id;
    public Sprite cardFront;

    private Image cardImage;
    private Sprite cardBackImage;

    public void Initialize(Sprite cardBack)
    {
        cardImage = GetComponent<Image>();
        cardBackImage = cardBack;

        cardImage.sprite = cardBackImage;
    }

    public void ShowFront()
    {
        cardImage.sprite = cardFront;
    }

    public void ShowBack()
    {
        cardImage.sprite = cardBackImage;
    }
}
