using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtons : MonoBehaviour
{
    [SerializeField] string gameplayScene = "Game"; // <-- set to your gameplay scene name in Inspector

    public void Play()
    {
        Time.timeScale = 1f;               // safety in case we return here while paused
        SceneManager.LoadScene(gameplayScene);
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
