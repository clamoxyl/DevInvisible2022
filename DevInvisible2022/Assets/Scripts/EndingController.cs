using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingController : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSrc;
    [SerializeField]
    private AudioClip greetingsClip;

    // Start is called before the first frame update

    void Start()
    {
        //RenderSettings.skybox = snowSkybox;
        GameManager.Instance.PlayMusic(3);
        StartCoroutine(PlayGreetings());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator PlayGreetings()
    {
        yield return new WaitForSeconds(3);
        audioSrc.PlayOneShot(greetingsClip);
    }
}
