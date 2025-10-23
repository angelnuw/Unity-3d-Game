using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] GameObject pausePanel;

    bool paused;

    void Start()
    {
        // Show name from the menu (or fallback)
        string name = PlayerPrefs.GetString("player_name", "Adventurer");
        if (playerNameText) playerNameText.text = name;

        // Game starts unpaused, cursor locked
        ResumeGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        paused = true;
        if (pausePanel) pausePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // You can also disable your PlayerController here if needed
    }

    public void ResumeGame()
    {
        paused = false;
        if (pausePanel) pausePanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Hook to Quit button
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
