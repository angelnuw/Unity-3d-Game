using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ObjectiveBanner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private Text legacyText;
    [SerializeField] private CanvasGroup group;

    [Header("Timing")]
    [SerializeField] private float fadeIn = 0.35f;
    [SerializeField] private float hold = 2.0f;
    [SerializeField] private float fadeOut = 0.75f;

    Coroutine showCo;

    void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();
        if (!tmp) tmp = GetComponent<TextMeshProUGUI>();
        if (!legacyText) legacyText = GetComponent<Text>();
        if (group) group.alpha = 0f;
    }

    public void Show(string message, float? customHold = null)
    {
        if (showCo != null) StopCoroutine(showCo);
        showCo = StartCoroutine(ShowRoutine(message, customHold ?? hold));
    }

    IEnumerator ShowRoutine(string message, float holdTime)
    {
        if (tmp) tmp.text = message;
        if (legacyText) legacyText.text = message;

        if (group)
        {
            // Fade in
            for (float t = 0; t < fadeIn; t += Time.unscaledDeltaTime)
            {
                group.alpha = Mathf.Lerp(0f, 1f, t / fadeIn);
                yield return null;
            }
            group.alpha = 1f;
        }

        yield return new WaitForSecondsRealtime(holdTime);

        if (group)
        {
            // Fade out
            for (float t = 0; t < fadeOut; t += Time.unscaledDeltaTime)
            {
                group.alpha = Mathf.Lerp(1f, 0f, t / fadeOut);
                yield return null;
            }
            group.alpha = 0f;
        }
        showCo = null;
    }
}