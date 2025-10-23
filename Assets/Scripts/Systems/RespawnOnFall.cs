using UnityEngine;

public class RespawnOnFall : MonoBehaviour
{
    [SerializeField] public Transform respawnPoint;   // keep serialized for debugging
    [SerializeField] float killY = -10f;

    CharacterController cc;

    void Awake() => cc = GetComponent<CharacterController>();

    public void SetRespawn(Transform t) => respawnPoint = t;

    void Update()
    {
        if (respawnPoint && transform.position.y < killY)
        {
            if (cc) cc.enabled = false;
            transform.SetPositionAndRotation(respawnPoint.position + Vector3.up * 0.1f, respawnPoint.rotation);
            if (cc) cc.enabled = true;
        }
    }

    public Transform RespawnPoint;
}
