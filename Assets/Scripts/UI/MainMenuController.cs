using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // new Input System
#endif

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";

    [Header("Panels")]
    [SerializeField] GameObject welcomePanel;
    [SerializeField] GameObject namePanel;
    [SerializeField] GameObject characterPanel;
    [SerializeField] GameObject instructionsPanel;

    [Header("Name Step")]
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Button nameContinueButton;   // NamePanel/Continue

    [Header("Character Step")]
    [SerializeField] Button characterContinueButton;
    [SerializeField] CharacterOption[] characterOptions;


    int _selectedCharacter = -1;

    void Awake()
    {
        // Start at the welcome screen
        ShowWelcome();

        if (instructionsPanel) instructionsPanel.SetActive(false);   // <— add this

        // Validate name as the user types
        if (nameInput)
            nameInput.onValueChanged.AddListener(OnNameChanged);

        if (nameContinueButton) nameContinueButton.interactable = false;
        if (characterContinueButton) characterContinueButton.interactable = false;
    }

    // ---------- Screen switches ----------
    public void ShowWelcome()
    {
        SetPanels(welcome: true, name: false, character: false);
    }

    public void ShowName()
    {
        SetPanels(welcome: false, name: true, character: false);

        // focus the input next frame so it’s ready to type
        StartCoroutine(FocusNameFieldNextFrame());
        ValidateName();
    }

    public void ShowCharacterSelect()
    {
        SetPanels(welcome: false, name: false, character: true);

        _selectedCharacter = -1;
        if (characterContinueButton) characterContinueButton.interactable = false;

        // clear any selection highlights
        if (characterOptions != null)
            foreach (var opt in characterOptions)
                if (opt) opt.SetSelected(false);
    }

    void SetPanels(bool welcome, bool name, bool character)
    {
        if (welcomePanel) welcomePanel.SetActive(welcome);
        if (namePanel) namePanel.SetActive(name);
        if (characterPanel) characterPanel.SetActive(character);
    }

    IEnumerator FocusNameFieldNextFrame()
    {
        yield return null;
        if (!nameInput) yield break;
        EventSystem.current?.SetSelectedGameObject(nameInput.gameObject);
        nameInput.ActivateInputField();
    }

    // ---------- Name step ----------
    void OnNameChanged(string _)
    {
        ValidateName();
    }

    void ValidateName()
    {
        if (!nameContinueButton || !nameInput) return;
        var ok = !string.IsNullOrWhiteSpace(nameInput.text);
        nameContinueButton.interactable = ok;
    }

    public void OnNameContinue()
    {
        if (!nameInput || string.IsNullOrWhiteSpace(nameInput.text)) return;

        PlayerPrefs.SetString("player_name", nameInput.text.Trim());
        PlayerPrefs.Save();

        ShowCharacterSelect();
    }

    // ---------- Character step ----------
    public void SelectCharacter(int idx)
    {
        _selectedCharacter = idx;

        if (characterOptions != null)
            for (int i = 0; i < characterOptions.Length; i++)
                if (characterOptions[i]) characterOptions[i].SetSelected(i == idx);

        if (characterContinueButton) characterContinueButton.interactable = true;

        Debug.Log($"[CharacterSelect] Picked index: {_selectedCharacter}");
    }

    public void ShowInstructions()
    {
        // Hide character UI, show instructions
        if (characterPanel) characterPanel.SetActive(false);
        if (instructionsPanel) instructionsPanel.SetActive(true);
    }

    public void OnCharacterContinue()
    {
        if (_selectedCharacter < 0) return;

        PlayerPrefs.SetInt("character_index", _selectedCharacter);
        PlayerPrefs.Save();

        ShowInstructions(); // <-- go to instructions instead of loading the scene yet
    }

    public void OnStartGame()
    {
        // Load your gameplay scene (create it in Step 3 and name it "Game")
        SceneManager.LoadScene("Game");
    }

    // ---------- Keyboard shortcuts ----------
    void Update()
    {
        // Enter on name step
        if (namePanel && namePanel.activeSelf &&
            nameContinueButton && nameContinueButton.interactable &&
            Input.GetKeyDown(KeyCode.Return))
        {
            OnNameContinue();
        }

        // Optional: Esc returns to Welcome from Name
        if (namePanel && namePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ShowWelcome();
        }
    }

    public void LoadGameScene()
    {
        // (optional) persist anything you’ve collected so far
        // PlayerPrefs.SetString("player_name", nameInput.text);
        // PlayerPrefs.SetInt("selected_char", selectedIndex);
        // PlayerPrefs.Save();

        SceneManager.LoadScene(gameSceneName);
    }
}
