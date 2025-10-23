using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TextMeshProUGUI text;

    [Header("Motion")]
    [SerializeField] private float lifetime = 0.9f;
    [SerializeField] private float floatSpeed = 1.2f;
    [SerializeField] private Vector3 spawnJitter = new Vector3(0.15f, 0.05f, 0.15f);

    [Header("Fade")]
    [SerializeField]
    private AnimationCurve alphaOverLife =
        AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

    private float _t;
    private Color _baseColor;

    public static void Spawn(Vector3 worldPos, int amount)
    {
        var prefab = Resources.Load<DamageNumber>("UI/DamageNumber");
        if (!prefab) { Debug.LogWarning("DamageNumber prefab missing at Resources/UI/DamageNumber"); return; }

        var inst = Instantiate(prefab, worldPos, Quaternion.identity);
        inst.SetValue(amount);
    }

    public void SetValue(int amount)
    {
        if (text)
        {
            text.SetText(amount.ToString());
            _baseColor = text.color;
        }

        // slight random offset so multiple numbers don’t stack perfectly
        transform.position += new Vector3(
            Random.Range(-spawnJitter.x, spawnJitter.x),
            Random.Range(0f, spawnJitter.y),
            Random.Range(-spawnJitter.z, spawnJitter.z)
        );
    }

    private void Update()
    {
        _t += Time.deltaTime;

        // float upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // face the camera (billboard)
        if (Camera.main)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0f, 180f, 0f); // flip so text reads correctly
        }

        // fade
        if (text)
        {
            float a = alphaOverLife.Evaluate(_t / lifetime);
            var c = _baseColor; c.a = Mathf.Clamp01(a);
            text.color = c;
        }

        if (_t >= lifetime)
            Destroy(gameObject);
    }
}
