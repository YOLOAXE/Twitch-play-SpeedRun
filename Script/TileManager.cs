using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Vector3 offsetNextTile = new Vector3(0f,0f,0f);
    [SerializeField] private GameObject[] triggerObject = null;
    [SerializeField] private Transform dispositionSpawn;

  
    public void Spawn()
    {
        Instantiate(dispositionSpawn,transform.parent.position + offsetNextTile, Quaternion.Euler(new Vector3(0, 0, 0)));
        triggerObject = GameObject.FindGameObjectsWithTag("Disposition");
        for(byte i = 0; i < triggerObject.Length;i++)
        {
            triggerObject[i].GetComponent<TileManager>().SetDisposition(dispositionSpawn);
        }
    }

    public void SetDisposition(Transform d)
    {
        this.dispositionSpawn = d;
    }

}
