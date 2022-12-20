using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FoodItem : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 5.0f;
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Collider col;
    [SerializeField]
    private Transform meshTr;
    [SerializeField]
    private int foodID = 0;    

    private Transform mouthPosition;

    public int FoodID { get => foodID; set => foodID = value; }
    public Transform MouthPosition { get => mouthPosition; set => mouthPosition = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        EnableFoodItem();
        StartCoroutine(AutoRecycleItem());
    }

    public void EatFoodItem()
    {
        rb.isKinematic = true;
        col.enabled = false;
        rb.transform.DOMove(mouthPosition.position, 0.2f).SetEase(Ease.InOutQuad).onComplete = DisableFoodItem;
        //DisableFoodItem();
    }

    private IEnumerator AutoRecycleItem()
    {
        yield return new WaitForSeconds(lifeTime);
        DisableFoodItem();
    }

    void DisableFoodItem()
    {
        rb.isKinematic = true;
        col.enabled = false;
        gameObject.SetActive(false);
    }

    void EnableFoodItem()
    {
        rb.isKinematic = false;
        col.enabled = true;
    }
}
