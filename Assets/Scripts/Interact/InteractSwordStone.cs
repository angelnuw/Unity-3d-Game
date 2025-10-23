using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class InteractSwordStone : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] private GameObject rockObject;            // drag SM_checkpoint_fire
    [SerializeField] private MeshRenderer combinedRenderer;    // drag SM_checkpoint_fire MeshRenderer
    [SerializeField] private int swordSubmeshIndex = 1;        // 1 = Element 1 (M_swords)

    [Header("Pickup")]
    [SerializeField] private GameObject swordPrefab;           // the usable sword to equip
    [SerializeField] private bool requireAllEnemiesDead = false;

    bool taken;

    // ---- Public: call this from PlayerController when E is pressed while looking at it
    public void PullSword(Transform playerHint = null)
    {
        if (taken) return;

        // if "require enemies dead" is on, stop until none remain
        if (requireAllEnemiesDead && FindObjectsOfType<EnemyAI>().Length > 0)
        {
            Debug.Log("[SwordStone] Enemies remain — cannot pull yet.");
            return;
        }

        // Hide the embedded sword (submesh)
        if (combinedRenderer)
            HideSubmesh(combinedRenderer, swordSubmeshIndex);

        // Equip a usable sword on player's right hand
        var player = playerHint ? playerHint : FindObjectOfType<PlayerController>()?.transform;
        if (player && swordPrefab) AttachToRightHand(player, swordPrefab);

        taken = true;
        // disable further hits if this has any colliders
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;

        Debug.Log("[SwordStone] Sword taken!");

        FindObjectOfType<TextMeshProUGUI>().text = "Sword Retrieved!";
    }

    static void HideSubmesh(MeshRenderer mr, int index)
    {
        var mats = mr.materials; // instanced copy
        if (index < 0 || index >= mats.Length) return;

        var m = new Material(mats[index]); // unique material instance
        if (m.HasProperty("_BaseColor"))   // URP Lit
        {
            var c = m.GetColor("_BaseColor"); c.a = 0f;
            m.SetColor("_BaseColor", c);
            m.SetFloat("_Surface", 1f); // Transparent
            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
        else
        {
            var c = m.color; c.a = 0f; m.color = c;
            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
        mats[index] = m;
        mr.materials = mats;
    }

    static void AttachToRightHand(Transform player, GameObject prefab)
    {
        var anim = player.GetComponentInChildren<Animator>();
        if (!anim) { Debug.LogWarning("[SwordStone] No Animator on player."); return; }

        var hand = anim.GetBoneTransform(HumanBodyBones.RightHand)
                   ?? anim.transform.Find("RightHand")
                   ?? anim.transform.Find("hand_r");
        if (!hand) { Debug.LogWarning("[SwordStone] Right-hand bone not found."); return; }

        var weapon = Object.Instantiate(prefab, hand);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localScale = Vector3.one;

        if (weapon.TryGetComponent<Rigidbody>(out var rb)) rb.isKinematic = true;
        foreach (var col in weapon.GetComponentsInChildren<Collider>()) col.enabled = false;
    }
}
