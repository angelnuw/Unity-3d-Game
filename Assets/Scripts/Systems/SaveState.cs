using UnityEngine;

public class SaveState : MonoBehaviour
{
    public static SaveState Instance { get; private set; }

    public string PlayerName { get; private set; }

    private void Awake()
    {
        // Ensure only one instance exists and it persists across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetPlayerName(string name)
    {
        PlayerName = name;
        // You can Debug.Log here if you want to verify:
        // Debug.Log($"[SaveState] PlayerName set to: {PlayerName}");
    }
}
