using UnityEngine;

/// <summary>
/// Toca efeitos e músicas. Não conhece grid nem peças: quem decide
/// o momento de tocar é o GameManager.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("Efeitos")]
    [SerializeField] private AudioClip pieceLanded;   // bloco_caindo1
    [SerializeField] private AudioClip lineClear;     // coluna_completa1
    [SerializeField] private AudioClip buttonClick;   // botoes

    [Header("Músicas (loop)")]
    [SerializeField] private AudioClip gameplayMusic; // musica
    [SerializeField] private AudioClip menuMusic;     // menu

    [Header("Volumes")]
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float musicVolume = 0.5f;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    private void Awake()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
    }

    public void PlayPieceLanded() { PlaySfx(pieceLanded); }
    public void PlayLineClear() { PlaySfx(lineClear); }
    public void PlayButtonClick() { PlaySfx(buttonClick); }

    public void PlayGameplayMusic() { PlayMusic(gameplayMusic); }
    public void PlayMenuMusic() { PlayMusic(menuMusic); }

    public void StopMusic() { musicSource.Stop(); }

    private void PlaySfx(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip, sfxVolume);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }
}