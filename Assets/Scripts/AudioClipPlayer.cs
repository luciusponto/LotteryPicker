using UnityEngine;

public class AudioClipPlayer : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip[] Clips;

    void Start()
    {
        if (AudioSource == null)
        {
            AudioSource = GetComponent<AudioSource>();
        }

        if (AudioSource == null)
        {
            Debug.LogError("Could not find AudioSource component. Self-destroying.");
            Destroy(gameObject);
        }
    }

    public void Play(int index)
    {
        if (index >= Clips.Length)
        {
            Debug.LogError($"Index {index} out of range. Only {Clips.Length} audio clips configured");
            return;
        }

        AudioSource.clip = Clips[index];
        AudioSource.Play();
    }

    public void PlayRandom()
    {
        var index = Random.Range(0, Clips.Length);
        Play(index);
    }
}
