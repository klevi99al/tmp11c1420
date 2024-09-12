using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static Settings Instance { get; private set; }
    private int gameDifficluty;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
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
}
