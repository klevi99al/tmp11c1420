using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Card Setup")]
    [SerializeField] private GameObject cardPrefab;   // Prefab for the card
    [SerializeField] private Transform cardParent;    // Parent for cards (Grid or empty GameObject)
    [SerializeField] private Sprite[] spriteRenderers;  // Array of unique front images for the cards
    [SerializeField] private Sprite cardBackImage; // Single back image for all cards
    [SerializeField] private GameObject newGame;
    private List<Card> allCards = new();
    private Card firstRevealedCard = null;
    private Card secondRevealedCard = null;

    [Header("Game Data")]
    private int matches = 0;
    private int turns = 0;
    [SerializeField] private TMP_Text matchesText;
    [SerializeField] private TMP_Text turnsText;

    [Header("Audio References")]
    [SerializeField] private AudioSource effectsSource;
    [SerializeField] private AudioClip cardFlip;
    [SerializeField] private AudioClip correct;
    [SerializeField] private AudioClip wrong;

    private readonly string matchesStr = "Matches: ";
    private readonly string turnsStr = "Turns: ";
    private bool gameLoaded = false;

    public void StartTheGame()
    {
        LoadGame();
        if (gameLoaded)
        {
            gameLoaded = false;
            return;
        }
    }

    public void NewGame()
    {
        ClearData();
        ResetCards();

        Vector2Int layout = Settings.Instance.GetCurrentLayout();
        int totalCards = layout.x * layout.y;

        List<Sprite> cardSprites = CreateCardPairs(totalCards);
        ShuffleSprites(cardSprites);
        SpawnCards(cardSprites, layout);
        ShowAllCards();
        Invoke(nameof(HideAllCards), 0.5f);
    }

    private List<Sprite> CreateCardPairs(int totalCards)
    {
        List<Sprite> cardSprites = new();

        int numPairs = totalCards / 2;
        for (int i = 0; i < numPairs; i++)
        {
            Sprite sprite = spriteRenderers[i % spriteRenderers.Length];
            cardSprites.Add(sprite);
            cardSprites.Add(sprite);
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

    private void SpawnCards(List<Sprite> cardSprites, Vector2Int layout)
    {
        // Adjust grid layout
        GridLayoutGroup gridLayout = cardParent.GetComponent<GridLayoutGroup>();
        if (gridLayout != null)
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = layout.y;
        }

        // Use a dictionary to track IDs for each unique sprite
        Dictionary<Sprite, int> spriteToID = new();

        for (int i = 0; i < cardSprites.Count; i++)
        {
            Sprite currentSprite = cardSprites[i];
            int id;

            // If the sprite is not in the dictionary, assign a new ID
            if (!spriteToID.TryGetValue(currentSprite, out id))
            {
                id = spriteToID.Count; // Use the current count as the ID
                spriteToID[currentSprite] = id; // Store the ID for this sprite
            }

            GameObject card = Instantiate(cardPrefab, cardParent);
            Card cardComponent = card.GetComponent<Card>();
            cardComponent.InitializeCard(currentSprite, cardBackImage, id, this);

            allCards.Add(cardComponent);
        }
    }

    private void ShowAllCards()
    {
        foreach (var card in allCards)
        {
            card.ShowFront();
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
        effectsSource.PlayOneShot(cardFlip);

        if (firstRevealedCard == null)
        {
            firstRevealedCard = clickedCard;
            firstRevealedCard.ShowFront();
        }
        else if (secondRevealedCard == null && clickedCard != firstRevealedCard)
        {
            turns++;
            turnsText.text = turnsStr + turns.ToString();

            secondRevealedCard = clickedCard;
            secondRevealedCard.ShowFront();

            if (firstRevealedCard.id == secondRevealedCard.id)
            {
                matches++;
                matchesText.text = matchesStr + matches.ToString();
                firstRevealedCard.DeactivateCard(0.5f);
                secondRevealedCard.DeactivateCard(0.5f);

                effectsSource.PlayOneShot(correct);

                if (matches == cardParent.transform.childCount / 2)
                {
                    Invoke(nameof(GameResetScreen), 1);
                }
            }
            else
            {
                //effectsSource.PlayOneShot(wrong);

                firstRevealedCard.ShowBack(0.5f);
                secondRevealedCard.ShowBack(0.5f);
            }
            firstRevealedCard = null;
            secondRevealedCard = null;
        }
    }

    private void GameResetScreen()
    {
        ClearData();
        newGame.SetActive(true);
    }

    public void ResetCards()
    {
        foreach (Transform tr in cardParent.transform)
        {
            Destroy(tr.gameObject);
        }
        matches = 0;
        turns = 0;
        matchesText.text = matchesStr;
        turnsText.text = turnsStr;
        allCards.Clear();
        firstRevealedCard = null;
        secondRevealedCard = null;
        matches = 0;
    }


    public void SaveGame()
    {
        PlayerPrefs.SetInt("Matches", matches);
        PlayerPrefs.SetInt("Turns", turns);

        // Save the current grid layout (rows and columns)
        Vector2Int currentLayout = Settings.Instance.GetCurrentLayout();
        PlayerPrefs.SetInt("GridRows", currentLayout.x);
        PlayerPrefs.SetInt("GridColumns", currentLayout.y);

        // Save card states
        for (int i = 0; i < allCards.Count; i++)
        {
            Card card = allCards[i];
            PlayerPrefs.SetInt($"Card_{i}_ID", card.id);
            PlayerPrefs.SetInt($"Card_{i}_IsActive", card.isActive ? 1 : 0);

            // Save the index of the card front sprite
            int spriteIndex = Array.IndexOf(spriteRenderers, card.cardFront);
            PlayerPrefs.SetInt($"Card_{i}_FrontSpriteIndex", spriteIndex);

            // Save the position of found (deactivated) cards
            if (!card.isActive) // Only store the index for matched cards
            {
                PlayerPrefs.SetInt($"Card_{i}_FoundIndex", i); // Save the card's index
            }
        }

        PlayerPrefs.SetInt("CardCount", allCards.Count);
        PlayerPrefs.Save();
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("Matches"))
        {
            gameLoaded = true;
            matches = PlayerPrefs.GetInt("Matches");
            turns = PlayerPrefs.GetInt("Turns");

            matchesText.text = matchesStr + matches.ToString();
            turnsText.text = turnsStr + turns.ToString();

            int cardCount = PlayerPrefs.GetInt("CardCount");

            // Load the saved grid layout dimensions
            int rows = PlayerPrefs.GetInt("GridRows", 4);  // Default to 4 rows if not found
            int columns = PlayerPrefs.GetInt("GridColumns", 3);  // Default to 3 columns if not found

            // Set the grid layout to match the saved game
            GridLayoutGroup gridLayout = cardParent.GetComponent<GridLayoutGroup>();
            if (gridLayout != null)
            {
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayout.constraintCount = columns;  // Set the column count from saved data
            }

            allCards.Clear();

            for (int i = 0; i < cardCount; i++)
            {
                int id = PlayerPrefs.GetInt($"Card_{i}_ID");
                bool isActive = PlayerPrefs.GetInt($"Card_{i}_IsActive") == 1;

                // Load the index of the front sprite and retrieve the corresponding sprite
                int spriteIndex = PlayerPrefs.GetInt($"Card_{i}_FrontSpriteIndex");
                Sprite frontSprite = spriteRenderers[spriteIndex];

                // Create a new card for each saved state
                GameObject card = Instantiate(cardPrefab, cardParent);
                Card cardComponent = card.GetComponent<Card>();
                cardComponent.InitializeCard(frontSprite, cardBackImage, id, this);

                Image cardImage = cardComponent.gameObject.GetComponent<Image>();
                if (!isActive) // If the card was matched, make it invisible
                {
                    cardComponent.ShowBack();
                    Color newColor = cardImage.color;
                    newColor.a = 255;  // Make the matched card invisible (alpha = 0)
                    cardImage.color = newColor;
                }
                else
                {
                    cardComponent.ShowFront(); // Unmatched card should show its back
                    Color newColor = cardImage.color;
                    newColor.a = 0;  // Ensure unmatched cards are fully visible (alpha = 1)
                    cardImage.color = newColor;
                }

                allCards.Add(cardComponent);
            }
        }
    }



    public void ClearData()
    {
        // Remove all saved data related to the game
        PlayerPrefs.DeleteKey("Matches");
        PlayerPrefs.DeleteKey("Turns");

        int cardCount = PlayerPrefs.GetInt("CardCount", 0);
        for (int i = 0; i < cardCount; i++)
        {
            PlayerPrefs.DeleteKey($"Card_{i}_ID");
            PlayerPrefs.DeleteKey($"Card_{i}_IsActive");
        }

        PlayerPrefs.DeleteKey("CardCount");

        // Ensure that changes are saved
        PlayerPrefs.Save();

        Debug.Log("Game data cleared.");

        // Optionally reset the game state after clearing data
        ResetCards();  // Reset the cards in the scene
    }
}
