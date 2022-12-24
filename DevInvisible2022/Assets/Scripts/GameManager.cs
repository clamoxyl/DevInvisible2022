using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);    
    }

    [SerializeField]
    private Image transitionCourtain;
    [SerializeField]
    private AudioSource musicSrc;
    [SerializeField]
    private AudioClip menuMusic;
    [SerializeField]
    private AudioClip gameMusic;
    [SerializeField]
    private AudioClip endMusic;
    [SerializeField]
    private float musicMaxVolume = 0.3f;

    private Color courtainColor;

    private bool introEnded = false;

    private bool gameEnded = false;

    private void Start()
    {
        courtainColor = transitionCourtain.color;
        LoadSceneAdditive(1);
        CourtainUp();
        musicSrc.volume = musicMaxVolume;
    }

    public void LoadSceneAdditive (int id)
    {
        if (!SceneManager.GetSceneByBuildIndex(id).isLoaded)
        {
            transitionCourtain.DOColor(courtainColor, 2.0f).SetEase(Ease.InOutQuad).onComplete = () => 
            {
                SceneManager.LoadScene(id, LoadSceneMode.Additive);
                CourtainUp();
            };
        }
    }

    public void UnloadScene (int id)
    {
        SceneManager.UnloadSceneAsync(id);
    }

    public void ReloadScene (int id)
    {
        transitionCourtain.DOColor(courtainColor, 2.0f).SetEase(Ease.InOutQuad).onComplete = () =>
        {
            SceneManager.UnloadSceneAsync(id);
            SceneManager.LoadScene(id, LoadSceneMode.Additive);
            CourtainUp();
        };
    }

    public void CourtainUp()
    {
        Color col = courtainColor;
        col.a = 0;
        transitionCourtain.DOColor(col, 2.0f).SetEase(Ease.InOutQuad);
    }

    public void PlayMusic (int id)
    {
        AudioClip clip = menuMusic;

        switch (id)
        {
            case 1:
                clip = menuMusic;
                break;
            case 2:
                clip = gameMusic;
                break;
            case 3:
                clip = endMusic;
                break;
        }

        if (musicSrc.isPlaying)
        {
            musicSrc.DOFade(0, .7f).onComplete = () =>
            {
                musicSrc.Stop();
                musicSrc.clip = clip;
                musicSrc.Play();
                musicSrc.DOFade(musicMaxVolume, .7f);
            };
        }
        else
        {
            musicSrc.clip = clip;
            musicSrc.Play();
        }
    }
        

}