using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Settings : MonoBehaviour
{
    [Header("References")]
    public TMP_Text layoutText;

    public static Settings Instance { get; private set; }
    private int gameDifficluty;

    public List<Vector2Int> gridSizes = new()
    {
        new Vector2Int(2, 2),  // 4 cards
        new Vector2Int(2, 3),  // 6 cards
        new Vector2Int(2, 4),  // 8 cards
        new Vector2Int(3, 4),  // 12 cards
        new Vector2Int(4, 4),  // 16 cards
        new Vector2Int(4, 5),  // 20 cards
    };

    [SerializeField] private int selectedLayoutIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        // Load the saved layout index
        if (PlayerPrefs.HasKey("SelectedLayoutIndex"))
        {
            selectedLayoutIndex = PlayerPrefs.GetInt("SelectedLayoutIndex");
        }

        // Update the layout text
        Vector2Int selectedLayout = gridSizes[selectedLayoutIndex];
        layoutText.text = $"{selectedLayout.x} x {selectedLayout.y}";
    }

    public void ChangeLayout(bool isNextButton)
    {
        if (isNextButton)
        {
            selectedLayoutIndex++;
            if (selectedLayoutIndex >= gridSizes.Count)
            {
                selectedLayoutIndex = 0;
            }
        }
        else
        {
            selectedLayoutIndex--;
            if (selectedLayoutIndex < 0)
            {
                selectedLayoutIndex = gridSizes.Count - 1;
            }
        }

        Vector2Int selectedLayout = gridSizes[selectedLayoutIndex];
        layoutText.text = $"{selectedLayout.x} x {selectedLayout.y}";

        PlayerPrefs.SetInt("SelectedLayoutIndex", selectedLayoutIndex);
        PlayerPrefs.Save();
    }

    public void SetGameDifficulty(int difficulty)
    {
        gameDifficluty = difficulty;
        Debug.Log("Game difficulty is: "+gameDifficluty);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public Vector2Int GetCurrentLayout()
    {
        return gridSizes[selectedLayoutIndex];
    }
}
