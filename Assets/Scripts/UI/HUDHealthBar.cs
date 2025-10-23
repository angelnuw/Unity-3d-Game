using UnityEngine;
using UnityEngine.UI;

public class HUDHealthBar : MonoBehaviour
{
    [Header("UI (use one)")]
    [SerializeField] Slider healthSlider;
    [SerializeField] Image healthFill;

    Health _bound;

    void Awake()
    {
        // Try to auto-wire the slider/image if left empty
        if (!healthSlider) healthSlider = GetComponentInChildren<Slider>();
        if (!healthFill && healthSlider)
        {
            // Try to find a child Image named "Fill" under the slider
            var fills = healthSlider.GetComponentsInChildren<Image>(true);
            foreach (var img in fills)
            {
                if (img.gameObject.name.ToLower().Contains("fill"))
                {
                    healthFill = img;
                    break;
                }
            }
        }
    }

    void Start()
    {
        // Optional auto-bind to the player's Health if not bound in code
        if (_bound == null)
        {
            var player = FindFirstObjectByType<PlayerController>();
            if (player) Bind(player.GetComponent<Health>());
            else
            {
                var anyHealth = FindFirstObjectByType<Health>();
                if (anyHealth) Bind(anyHealth);
            }
        }
    }

    void OnDestroy()
    {
        if (_bound != null) _bound.OnChanged -= OnChanged;
    }

    public void Bind(Health h)
    {
        if (_bound != null) _bound.OnChanged -= OnChanged;
        _bound = h;
        if (_bound != null)
        {
            _bound.OnChanged += OnChanged;
            OnChanged(_bound.Current, _bound.Max);
        }
    }

    void OnChanged(int current, int max)
    {
        if (max <= 0) max = 1;
        float t = current / (float)max;

        if (healthSlider) healthSlider.value = t;
        if (healthFill) healthFill.fillAmount = t;
    }
}
