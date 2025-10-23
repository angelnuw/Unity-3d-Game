using UnityEngine;
using UnityEngine.EventSystems;

public class PausingSystem : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;

    bool _paused;

    void Start()
    {
        Unpause();               // 🔴 force game unpaused on scene start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_paused) Unpause();
            else Pause();
        }
    }

    public void Pause()
    {
        _paused = true;
        Time.timeScale = 0f;
        if (pausePanel) pausePanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Unpause()
    {
        _paused = false;
        Time.timeScale = 1f;
        if (pausePanel) pausePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
