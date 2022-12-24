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
    public Rigidbody Rb { get => rb; set => rb = value; }
    public Collider Col { get => col; set => col = value; }
    
    public void EatFoodItem()
    {
        Rb.transform.DOMove(mouthPosition.position, 0.2f).SetEase(Ease.InOutQuad).onComplete = DisableFoodItem;        
    }

    private IEnumerator AutoRecycleItem()
    {
        yield return new WaitForSeconds(lifeTime);
        DisableFoodItem();
    }

    public void DisableFoodItem()
    {
        Rb.isKinematic = true;
        Col.enabled = false;
        Rb.transform.position = transform.position;
        gameObject.SetActive(false);
    }

    public void EnableFoodItem(float delay)
    {
        gameObject.SetActive(true);        
        StartCoroutine(DelayedCollider(delay));        
        transform.rotation = Random.rotation;
    }

    IEnumerator DelayedCollider(float delay)
    {
        yield return new WaitForSeconds(delay);
        Col.enabled = true;
        Rb.isKinematic = false;
        Rb.velocity = Vector3.zero;
    }
}
