using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Singleton Pattern
    public static SoundManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
    #endregion

    public static AudioClip game_song, reveal_cell, flag_cell, victory, bomb, mouse_click;

    private static AudioSource audioSource;


    void Start()
    {
        reveal_cell = Resources.Load<AudioClip>("reveal_cell");
        flag_cell = Resources.Load<AudioClip>("flag_cell");
        victory = Resources.Load<AudioClip>("victory");
        bomb = Resources.Load<AudioClip>("bomb");
        mouse_click = Resources.Load<AudioClip>("mouse_click");
        game_song = Resources.Load<AudioClip>("game_song");

        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "reveal_cell":
                audioSource.PlayOneShot(reveal_cell, 1f);
                break;
            case "flag_cell":
                audioSource.PlayOneShot(flag_cell, .4f);
                break;
            case "victory":
                audioSource.PlayOneShot(victory, 3f);
                break;
            case "bomb":
                audioSource.PlayOneShot(bomb, 1f);
                break;
            case "mouse_click":
                audioSource.PlayOneShot(mouse_click, 1.5f);
                break;
            default:
                break;
        }
    }

    public static void ChangeMusic(string music)
    {
        switch (music)
        {
            case "game_song":
                audioSource.clip = game_song;
                audioSource.Play();
                break;
            default:
                break;
        }
    }
}