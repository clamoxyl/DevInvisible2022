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
    private AudioSource sfxSrc;
    [SerializeField]
    private AudioClip menuMusic;
    [SerializeField]
    private AudioClip gameMusic;

    private Color courtainColor;

    private bool introEnded = false;

    private bool gameEnded = false;

    private void Start()
    {
        courtainColor = transitionCourtain.color;
        LoadSceneAdditive(1);
        CourtainUp();
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
        }

        if (musicSrc.isPlaying)
        {
            musicSrc.DOFade(0, 2f).onComplete = () =>
            {
                musicSrc.Stop();
                musicSrc.clip = clip;
                musicSrc.Play();
            };
        }
        else
        {
            musicSrc.clip = clip;
            musicSrc.Play();
        }
    }
        

}