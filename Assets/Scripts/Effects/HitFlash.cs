using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Briefly tints all renderers under this object using a MaterialPropertyBlock,
/// so we don't permanently modify materials or create leaks.
/// Works with Standard/URP Lit that expose _Color.
/// </summary>
/// 
public class HitFlash : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;     // optional manual list
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float duration = 0.12f;

    private readonly List<Renderer> _rs = new List<Renderer>();
    private readonly List<Color> _orig = new List<Color>();
    private MaterialPropertyBlock _mpb;

    // shader property ids for speed
    static readonly int ID_Color = Shader.PropertyToID("_Color");
    static readonly int ID_BaseColor = Shader.PropertyToID("_BaseColor");

    void Awake()
    {
        _mpb = new MaterialPropertyBlock();

        // build renderer list
        _rs.Clear();
        if (renderers != null && renderers.Length > 0)
        {
            _rs.AddRange(renderers);
        }
        else
        {
            GetComponentsInChildren(true, _rs); // MeshRenderer + SkinnedMeshRenderer
        }

        // cache original colors
        _orig.Clear();
        foreach (var r in _rs)
        {
            _mpb.Clear();
            r.GetPropertyBlock(_mpb);

            // Try URP first, then Standard; if neither, fall back to material color or white
            Color c = Color.white;

            if (_mpb.HasVector(ID_BaseColor)) c = _mpb.GetColor(ID_BaseColor);
            else if (_mpb.HasVector(ID_Color)) c = _mpb.GetColor(ID_Color);
            else if (r.sharedMaterial != null)
            {
                if (r.sharedMaterial.HasProperty(ID_BaseColor)) c = r.sharedMaterial.GetColor(ID_BaseColor);
                else if (r.sharedMaterial.HasProperty(ID_Color)) c = r.sharedMaterial.GetColor(ID_Color);
            }

            _orig.Add(c);
        }
    }

    public void Flash()
    {
        if (!isActiveAndEnabled || _rs.Count == 0) return;
        StopAllCoroutines();
        StartCoroutine(CoFlash());
    }

    IEnumerator CoFlash()
    {
        // set flash
        for (int i = 0; i < _rs.Count; i++)
        {
            var r = _rs[i];
            r.GetPropertyBlock(_mpb);
            // write both so whichever the shader uses will take effect
            _mpb.SetColor(ID_BaseColor, flashColor);
            _mpb.SetColor(ID_Color, flashColor);
            r.SetPropertyBlock(_mpb);
        }

        yield return new WaitForSeconds(duration);

        // restore
        for (int i = 0; i < _rs.Count; i++)
        {
            var r = _rs[i];
            r.GetPropertyBlock(_mpb);
            _mpb.SetColor(ID_BaseColor, _orig[i]);
            _mpb.SetColor(ID_Color, _orig[i]);
            r.SetPropertyBlock(_mpb);
        }
    }
}
