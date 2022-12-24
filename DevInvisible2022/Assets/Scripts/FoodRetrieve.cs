using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodRetrieve : MonoBehaviour
{
    [SerializeField]
    private Transform foodSpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        other.transform.position = foodSpawnPoint.position;
        rb.velocity = Vector3.zero;
    }
}
