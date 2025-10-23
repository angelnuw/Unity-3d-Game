using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_2023_1_OR_NEWER
using UObj = UnityEngine.Object;
#endif

public class GameManager : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;
    [Tooltip("Order MUST match your menu character indexes.")]
    [SerializeField] private GameObject[] playerPrefabs;

    [Header("Grounding")]
    [SerializeField] private LayerMask groundMask = ~0;
    [SerializeField] private float castHeight = 5f;
    [SerializeField] private float extraUp = 0.05f;

    private GameObject player;

    void Start()
    {
        SpawnAndBind();
    }

    private void SpawnAndBind()
    {
        // --- choose index (0 if you don’t have SaveState yet) ---
        int index = 0;

        if (playerPrefabs == null || playerPrefabs.Length == 0)
        {
            Debug.LogWarning("[GM] No player prefabs assigned!");
            return;
        }
        index = Mathf.Clamp(index, 0, playerPrefabs.Length - 1);

        // Reuse existing or spawn new
        PlayerController existing;
#if UNITY_2023_1_OR_NEWER
        existing = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
#else
        existing = FindObjectOfType<PlayerController>();
#endif
        if (existing)
        {
            player = existing.gameObject;
            Debug.Log("[GM] Found existing player in scene.");
        }
        else
        {
            var prefab = playerPrefabs[index];
            Vector3 pos = spawnPoint ? spawnPoint.position : Vector3.zero;

            // Drop to ground from above spawn
            Vector3 origin = pos + Vector3.up * castHeight;
            if (Physics.Raycast(origin, Vector3.down, out var hit, castHeight * 2f, groundMask, QueryTriggerInteraction.Ignore))
                pos = hit.point + Vector3.up * extraUp;
            else
                Debug.LogWarning("[GM] No ground hit under SpawnPoint – using spawn Y.");

            player = Instantiate(prefab, pos, spawnPoint ? spawnPoint.rotation : Quaternion.identity);
            player.tag = "Player";
            Debug.Log($"[GM] Spawned '{player.name}' at {pos} (index {index}).");
        }

        // Optional: snap once
        var ag = player.GetComponent<AutoGrounder>();
        if (ag) { ag.GroundMask = groundMask; ag.SnapToGround(); }

        // ---------- CAMERA HOOKUP (debug + stronger lookup) ----------
        var pc = player.GetComponent<PlayerController>();

        Camera camComp = Camera.main;
        if (!camComp)
        {
            var byName = GameObject.Find("Main Camera");
            if (byName) camComp = byName.GetComponent<Camera>();
        }
        if (!camComp)
        {
            camComp = FindObjectOfType<Camera>(true); // find inactive/disabled too
        }

        if (camComp)
        {
            var mainCam = camComp.transform;

            Debug.Log($"[GM] Found camera: name='{camComp.name}', tag='{camComp.tag}', enabled={camComp.enabled}, activeInHierarchy={camComp.gameObject.activeInHierarchy}");

            // Parent camera to player and position it
            mainCam.SetParent(player.transform, worldPositionStays: false);
            mainCam.localPosition = new Vector3(0f, 1.6f, -3f);
            mainCam.localRotation = Quaternion.Euler(15f, 0f, 0f);

            // Tell PlayerController about this camera
            if (pc)
            {
                pc.cameraTransform = mainCam;
                Debug.Log($"[GM] Camera parented under '{mainCam.parent.name}'. PlayerController bound.");
            }
            else
            {
                Debug.LogWarning("[GM] PlayerController missing on player when binding camera.");
            }
        }
        else
        {
            Debug.LogWarning("[GM] No Camera found. Is there an enabled Camera in the scene? Is it tagged 'MainCamera'?");
        }

        // ---------- HUD BIND ----------
        HUDController hud;
#if UNITY_2023_1_OR_NEWER
        hud = FindFirstObjectByType<HUDController>(FindObjectsInactive.Include);
#else
        hud = FindObjectOfType<HUDController>();
#endif
        var health = player.GetComponent<Health>();
        if (hud && health)
        {
            hud.Bind(health);
            hud.gameObject.SetActive(true);
            Debug.Log("[GM] HUD bound to player health.");
        }
        else
        {
            Debug.LogWarning("[GM] HUD bind skipped (missing HUDController or Health).");
        }

        var banner = FindObjectOfType<ObjectiveBanner>(true);
        if (banner) banner.Show("Mission: Find the sword", 2.5f); // 2.5s hold before fading
    }
}