using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerDeath : MonoBehaviour
{
    [Header("Respawn")]
    [Tooltip("If set, we search this exact name; otherwise we try Tag 'Respawn'.")]
    public string spawnPointName = "SpawnPoint";
    [Tooltip("Seconds to wait before teleporting back to spawn.")]
    public float respawnDelay = 1.25f;

    Health _health;
    CharacterController _cc;
    PlayerController _pc;
    bool _respawning;

    void Awake()
    {
        _health = GetComponent<Health>();
        _cc = GetComponent<CharacterController>();
        _pc = GetComponent<PlayerController>();
    }

    void OnEnable()
    {
        if (_health) _health.OnDied += HandleDied;
    }

    void OnDisable()
    {
        if (_health) _health.OnDied -= HandleDied;
    }

    void HandleDied()
    {
        if (_respawning) return;
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        _respawning = true;

        // Freeze control by disabling components
        if (_pc) _pc.enabled = false;
        if (_cc) _cc.enabled = false;

        yield return new WaitForSecondsRealtime(respawnDelay);

        // --- find a spawn transform ---
        Transform spawn = null;

        if (!string.IsNullOrWhiteSpace(spawnPointName))
        {
            var byName = GameObject.Find(spawnPointName);
            if (byName) spawn = byName.transform;
        }

        if (!spawn)
        {
            var byTag = GameObject.FindGameObjectWithTag("Respawn");
            if (byTag) spawn = byTag.transform;
        }

        // --- move there (with a small up-offset) ---
        if (spawn)
        {
            var pos = spawn.position + Vector3.up * 0.1f;
            transform.SetPositionAndRotation(pos, spawn.rotation);

            // Optional: drop to ground
            if (Physics.Raycast(pos + Vector3.up, Vector3.down, out var hit, 5f, ~0, QueryTriggerInteraction.Ignore))
                transform.position = hit.point + Vector3.up * 0.05f;
        }
        else
        {
            Debug.LogWarning("[Respawn] No spawn found. Staying where we are.");
        }

        // Refill health (see Health.ReviveFull below)
        if (_health) _health.ReviveFull();

        // Re-enable control
        if (_cc) _cc.enabled = true;
        if (_pc) _pc.enabled = true;

        _respawning = false;
    }
}