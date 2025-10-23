using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject pauseRoot;       // drag your PausePanel here

    [Header("Scenes")]
    [SerializeField] string mainMenuSceneName = "MainMenu"; // set to your actual main menu scene name

    bool _paused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_paused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        if (_paused) return;
        _paused = true;

        if (pauseRoot) pauseRoot.SetActive(true);
        Time.timeScale = 0f;

        // show cursor for UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        if (!_paused) return;
        _paused = false;

        if (pauseRoot) pauseRoot.SetActive(false);
        Time.timeScale = 1f;

        // optional: re-lock cursor for your controller
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // ensure time unfreezes
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

