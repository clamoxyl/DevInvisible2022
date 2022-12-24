using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingController : MonoBehaviour
{
    //[SerializeField]
    //private Material snowSkybox;

    // Start is called before the first frame update

    void Start()
    {
        //RenderSettings.skybox = snowSkybox;
        GameManager.Instance.PlayMusic(3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
