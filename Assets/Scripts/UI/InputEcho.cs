using UnityEngine;

public class InputEcho : MonoBehaviour
{
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f)
            Debug.Log($"HV = {h:F2}, {v:F2}");
    }
}
