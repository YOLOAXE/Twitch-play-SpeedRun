using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grille : MonoBehaviour
{
    [SerializeField] private Vector2 maxSpawn = new Vector2(100f,100f);
    [SerializeField] private GameObject spawnObject = null;
    [SerializeField] private float espace = 2f;

    void Awake()
    {
        for(int i = 0; i < maxSpawn.y; i++)
        {
            for (int j = 0; j < maxSpawn.x; j++)
            {
                Instantiate(spawnObject, transform.position + new Vector3(i*espace,0,j*espace), Quaternion.identity);
            }
        }
    }
}
