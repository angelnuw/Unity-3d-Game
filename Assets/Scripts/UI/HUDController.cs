using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("UI (use one)")]
    [SerializeField] private Slider healthSlider; // normalized 0..1
    [SerializeField] private Image healthFill;    // optional filled Image

    [Header("Optional")]
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private CanvasGroup damageFlash;
    [SerializeField, Range(0, 1)] private float flashAlpha = 0.5f;
    [SerializeField] private float flashFadeSeconds = 0.25f;

    private Health _bound;

    // ---------------------------
    // PUBLIC: Bind to a Health
    // ---------------------------
    public void Bind(Health h)
    {
        if (_bound != null) _bound.OnChanged -= UpdateUI; // unsubscribe old
        _bound = h;
        if (_bound != null)
        {
            _bound.OnChanged += UpdateUI;

            // Force normalized slider behavior and ensure no integer rounding.
            if (healthSlider)
            {
                healthSlider.wholeNumbers = false; // IMPORTANT
                healthSlider.maxValue = 1f;
            }

            UpdateUI(_bound.Current, _bound.Max);
        }
    }

    private void OnDestroy()
    {
        if (_bound != null) _bound.OnChanged -= UpdateUI;
    }

    // ---------------------------
    // UPDATE HEALTH DISPLAY
    // ---------------------------
    private void UpdateUI(int current, int max)
    {
        if (max <= 0) max = 1;
        float t = current / (float)max; // normalized 0..1

        if (healthSlider)
        {
            // keep normalized mode and no rounding
            if (healthSlider.wholeNumbers) healthSlider.wholeNumbers = false;
            if (healthSlider.maxValue != 1f) healthSlider.maxValue = 1f;

            healthSlider.value = t;
        }

        if (healthFill) healthFill.fillAmount = t;

        // optional simple flash kick
        if (damageFlash && current < max)
        {
            StopAllCoroutines();
            StartCoroutine(DamageFlashRoutine());
        }
    }

    // Overload so legacy OnChanged(Action) can call it
    private void UpdateUI()
    {
        if (_bound == null) return;
        UpdateUI(_bound.Current, _bound.Max);
    }

    // ---------------------------
    // OPTIONAL: fade the damage flash overlay
    // ---------------------------
    public void FlashDamage()
    {
        if (damageFlash)
        {
            StopAllCoroutines();
            StartCoroutine(DamageFlashRoutine());
        }
    }

    private IEnumerator DamageFlashRoutine()
    {
        if (!damageFlash) yield break;
        damageFlash.alpha = flashAlpha;
        float t = 0f;
        while (t < flashFadeSeconds)
        {
            t += Time.deltaTime;
            damageFlash.alpha = Mathf.Lerp(flashAlpha, 0f, t / flashFadeSeconds);
            yield return null;
        }
        damageFlash.alpha = 0f;
    }
}
