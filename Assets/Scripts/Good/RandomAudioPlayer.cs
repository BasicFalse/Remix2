using UnityEngine;

public class RandomAudioPlayer : RemixScript
{
    public AudioClip[] audioClips; // Array of audio clips to play
    public float minInterval = 2f; // Minimum interval between audio plays
    public float maxInterval = 5f; // Maximum interval between audio plays

    private AudioSource audioSource;
    private float nextPlayTime;

    public override void StartExt()
    {
        audioSource = GetComponent<AudioSource>();

        // Schedule the first audio play
        ScheduleNextPlay();
    }

    void Update()
    {
        // Check if it's time to play another audio clip
        if (Time.time >= nextPlayTime)
        {
            PlayRandomAudio();
            ScheduleNextPlay();
        }
    }

    void PlayRandomAudio()
    {
        if (audioClips.Length > 0)
        {
            // Generate a random index within the array length
            int randomIndex = Random.Range(0, audioClips.Length);

            // Play the audio clip at the random index
            audioSource.clip = audioClips[randomIndex];
            audioSource.Play();

            // Reset the next play time
            nextPlayTime = Time.time + Random.Range(minInterval, maxInterval);
        }
    }

    void ScheduleNextPlay()
    {
        // Generate a random interval between min and max
        nextPlayTime = Time.time + Random.Range(minInterval, maxInterval);
    }
}