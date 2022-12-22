using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class IntroController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text loreText;
    [SerializeField]
    private Image loreBG;
    [SerializeField]
    private float scrollDuration = 65f;
    [SerializeField]
    private float scrollFinalPosY = 500f;

    private bool allowInput = false;

    // Start is called before the first frame update
    void Start()
    {
        loreText.transform.DOMoveY(scrollFinalPosY, scrollDuration).SetEase(Ease.Linear).onComplete = LoreEnd;
        GameManager.Instance.PlayMusic(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (allowInput && Input.anyKeyDown)
        {
            GameManager.Instance.LoadSceneAdditive(2);
            GameManager.Instance.UnloadScene(1);
        }
    }

    private void LoreEnd()
    {
        loreBG.DOColor(Color.clear, 2f).SetEase(Ease.InOutQuad).onComplete = () => { allowInput = true; };
    }
}
