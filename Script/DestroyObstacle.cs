using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObstacle : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if(other.transform.tag == "Obstacle" && Random.Range(0,10) > 5)
        {
            Destroy(gameObject);
        }
    }
}
