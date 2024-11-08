using UnityEngine;

public class Music : Singletone<Music>
{
    public AudioSource MusicSource;

    [SerializeField] private AudioClip _mainMenu;
    [SerializeField] private AudioClip _level;

    public override void Awake()
    {
        base.Awake();

        MusicSource = GetComponent<AudioSource>();
    }

    public void SetMenu()
    {
        MusicSource.clip = _mainMenu;
        MusicSource.Play();
    }

    public void SetLevel()
    {
        MusicSource.clip = _level;
        MusicSource.Play();
    }
}
