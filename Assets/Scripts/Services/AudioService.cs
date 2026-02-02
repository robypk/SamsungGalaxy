using UnityEngine;

public class AudioService : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip correct;
    [SerializeField] private AudioClip wrong;
    [SerializeField] private AudioClip levelComplete;

    private void Awake()
    {
        Services.Register<AudioService>(this);
    }

    public void ButtonClick()
    {
        PlaySound(buttonClick);
    }

    public void PlayCorrect()
    {
        PlaySound(correct);
    }

    public void PlayWrong()
    {
        PlaySound(wrong);
    }

    public void PlayLevelComplete()
    {
        PlaySound(levelComplete);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource == null || clip == null)
            return;

        audioSource.PlayOneShot(clip);
    }
}
