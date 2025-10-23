using UnityEngine;

public class FootstepReceiver : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip[] clips;

    // Event signature without params
    public void OnFootstep() => Play();

    // Some assets send AnimationEvent with data; this covers that too
    public void OnFootstep(AnimationEvent evt) => Play();

    void Play()
    {
        if (!source) source = GetComponent<AudioSource>();
        if (!source) source = gameObject.AddComponent<AudioSource>();

        if (clips != null && clips.Length > 0)
        {
            var clip = clips[Random.Range(0, clips.Length)];
            source.PlayOneShot(clip);
        }
        // If you don’t want sound yet, leave clips empty; the method existing
        // stops the console errors either way.
    }
}
