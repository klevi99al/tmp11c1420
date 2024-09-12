using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Card Setup")]
    public GameObject cardPrefab;   // Prefab for the card
    public Transform cardParent;    // Parent for cards (Grid or empty GameObject)
    public Sprite[] spriteRenderers;  // Array of unique front images for the cards
    public Sprite cardBackImage; // Single back image for all cards
    public bool cardsSpawned = false;

    public List<Card> allCards = new List<Card>();
    public Card firstRevealedCard = null;
    public Card secondRevealedCard = null;

    [Header("Game Data")]
    private int matches = 0;
    [SerializeField] private TMP_Text matchesText;
    public void StartTheGame()
    {
        if (!cardsSpawned)
        {
            cardsSpawned = true;

            List<Sprite> cardSprites = CreateCardPairs();
            ShuffleSprites(cardSprites);
            SpawnCards(cardSprites);
            ShowAllCards();
            Invoke(nameof(HideAllCards), 0.5f); // Show all cards briefly at start
        }
    }

    private void ShowAllCards()
    {
        foreach (var card in allCards)
        {
            card.ShowFront();
        }
    }

    private List<Sprite> CreateCardPairs()
    {
        List<Sprite> cardSprites = new();

        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            cardSprites.Add(spriteRenderers[i]); // Add the sprite
            cardSprites.Add(spriteRenderers[i]); // Add the duplicate for matching
        }

        return cardSprites;
    }

    private void ShuffleSprites(List<Sprite> sprites)
    {
        for (int i = sprites.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            // Swap the sprites
            (sprites[j], sprites[i]) = (sprites[i], sprites[j]);
        }
    }

    private void SpawnCards(List<Sprite> cardSprites)
    {
        Dictionary<Sprite, int> spriteToID = new();

        for (int i = 0; i < cardSprites.Count; i++)
        {
            Sprite currentSprite = cardSprites[i];
            int id;

            if (!spriteToID.TryGetValue(currentSprite, out id))
            {
                id = spriteToID.Count;
                spriteToID[currentSprite] = id;
            }

            GameObject card = Instantiate(cardPrefab, cardParent);
            Card cardComponent = card.GetComponent<Card>();
            cardComponent.InitializeCard(currentSprite, cardBackImage, id, this); // Pass reference to GameManager

            allCards.Add(cardComponent);
        }
    }

    private void HideAllCards()
    {
        foreach (var card in allCards)
        {
            card.ShowBack();
        }
    }

    public void HandleCardClick(Card clickedCard)
    {
        if (firstRevealedCard == null)
        {
            firstRevealedCard = clickedCard;
            firstRevealedCard.ShowFront();
        }
        else if (secondRevealedCard == null && clickedCard != firstRevealedCard)
        {
            secondRevealedCard = clickedCard;
            secondRevealedCard.ShowFront();

            if (firstRevealedCard.id == secondRevealedCard.id)
            {
                matches++;
                matchesText.text = "Matches: "+ matches.ToString();
                firstRevealedCard.DeactivateCard(0.5f);
                secondRevealedCard.DeactivateCard(0.5f);
            }
            else
            {
                firstRevealedCard.ShowBack(0.5f);
                secondRevealedCard.ShowBack(0.5f);
            }
            firstRevealedCard = null;
            secondRevealedCard = null;
        }
    }
}
