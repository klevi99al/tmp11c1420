using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int id;
    public bool isActive = false;

    public Sprite cardFront;
    private Sprite cardBack;
    
    public GameObject comparingCard = null;
    
    private Image cardImage;
    private GameManager gameManager;

    private void Awake()
    {
        cardImage = GetComponent<Image>();
    }

    public void DeactivateCard(float timer = 0)
    {
        Invoke(nameof(SetCardState), timer);
    }

    private void SetCardState()
    {
        isActive = true; // so it doesn't trigger OnCardClicked
        Color newColor = cardImage.color;
        newColor.a = 0;
        cardImage.color = newColor;
    }

    public void InitializeCard(Sprite frontImage, Sprite backImage, int cardId, GameManager manager)
    {
        id = cardId;
        cardFront = frontImage;
        cardBack = backImage;
        gameManager = manager;
    }

    public void ShowFront()
    {
        isActive = true;
        cardImage.sprite = cardFront;
    }

    public void ShowBack(float time = 0)
    {
        Invoke(nameof(HideCard), time);
    }

    private void HideCard()
    {
        isActive = false;
        cardImage.sprite = cardBack;
    }

    public void OnCardClick()
    {
        Debug.Log("Clicked");
        if (!isActive)
        {
            gameManager.HandleCardClick(this);
        }
    }

    public void CardReset()
    {
        id = 0;
        isActive = false;
        cardFront = null;
        comparingCard = null;
        cardImage.sprite = cardBack;

        Color newColor = cardImage.color;
        newColor.a = 0;
        cardImage.color = newColor;
    }
}
