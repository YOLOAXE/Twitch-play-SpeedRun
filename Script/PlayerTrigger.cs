using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    [SerializeField] private Player pl = null;
    
    void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Obstacle")
        {
            pl.StartCoroutine(pl.Obstacle());
        }
    }
}
