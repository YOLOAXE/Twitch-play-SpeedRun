using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallDead : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            StartCoroutine(other.transform.GetComponent<Player>().Dead());
        }
    }
}
