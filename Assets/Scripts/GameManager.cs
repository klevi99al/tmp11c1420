using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Card Setup")]
    public GameObject cardPrefab;   // Prefab for the card
    public Transform cardParent;    // Parent for cards (Grid or empty GameObject)
    public Sprite[] spriteRenderers;  // Array of unique front images for the cards
    public float cardSpacing = 2.0f;  // Spacing between the cards
    public bool cardsSpawned = false;

    public List<GameObject> allCards = new();

    public void StartTheGame()
    {
        if (!cardsSpawned)
        {
            List<Sprite> cardSprites = CreateCardPairs(); // Create pairs of sprites
            ShuffleSprites(cardSprites); // Shuffle the sprites
            SpawnCards(cardSprites); // Spawn cards using the shuffled sprites
            cardsSpawned = true; // Mark cards as spawned
        }
    }

    private List<Sprite> CreateCardPairs()
    {
        List<Sprite> cardSprites = new List<Sprite>();

        // Duplicate each sprite in spriteRenderers
        foreach (Sprite sprite in spriteRenderers)
        {
            cardSprites.Add(sprite); // Add the original sprite
            cardSprites.Add(sprite); // Add the duplicate for matching
        }

        return cardSprites;
    }

    private void ShuffleSprites(List<Sprite> sprites)
    {
        for (int i = sprites.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            // Swap the sprites
            Sprite temp = sprites[i];
            sprites[i] = sprites[j];
            sprites[j] = temp;
        }
    }

    private void SpawnCards(List<Sprite> cardSprites)
    {
        for (int i = 0; i < cardSprites.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, cardParent);
            card.GetComponent<Image>().sprite = cardSprites[i];
            allCards.Add(card);
        }
    }
}
