using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] float respawnDelay = 1.0f;
    [SerializeField] Transform explicitSpawnPoint;   // optional override

    CharacterController cc;
    PlayerController pc;
    Health health;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        pc = GetComponent<PlayerController>();
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        if (health) health.OnDied += HandleDied;
    }

    void OnDisable()
    {
        if (health) health.OnDied -= HandleDied;
    }

    void HandleDied()
    {
        // Disable input movement while “dead”
        if (pc) pc.enabled = false;

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        // Pick a spawn point
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        Transform sp = explicitSpawnPoint;
        if (!sp)
        {
            // Try find a "SpawnPoint" object or ask GameManager
            var go = GameObject.Find("SpawnPoint");
            if (go) sp = go.transform;
        }

        if (sp)
        {
            spawnPos = sp.position;
            spawnRot = sp.rotation;
        }

        // Teleport safely (disable controller to avoid bumps)
        if (cc) cc.enabled = false;
        transform.SetPositionAndRotation(spawnPos, spawnRot);
        if (cc) cc.enabled = true;

        // Restore full health
        if (health) health.Refill();

        // Re-enable player control
        if (pc) pc.enabled = true;
    }
}
