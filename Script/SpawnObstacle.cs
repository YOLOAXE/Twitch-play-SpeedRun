using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObstacle : MonoBehaviour
{
    [SerializeField] private GameObject[] allObstacle1 = null;// les obstacles qui font 1 de taille
    [SerializeField] private GameObject[] allObstacle2 = null;// obstacle qui font deux
    [SerializeField] private GameObject[] allDecore = null;// decor ce qui n'est pas interactable par le joueur
    [SerializeField] private Vector3 offsetPos = new Vector3(5,3,5);// la position qui ajouter l'ors ce que l'on place les elements
    [SerializeField] private GameObject[] AllTile1 = null;//les tile sur le bords
    [SerializeField] private GameObject[] AllTile2 = null;//les tile au milieux
    [SerializeField] private GameObject[] dalleSpeciale = null;// les dalle special malus bonus

    [Range(0,100)] [SerializeField] private float probabilite = 5f;//Probabilite du spawn d'un obstacle sur une case
    [Range(0, 100)] [SerializeField] private float probSD = 5f;//Probabilite du spawn d'un decor sur une case
    [Range(0, 100)] [SerializeField] private float probSDS = 1f;//Probabilite du spawn dune dalle special sur une case
    private float[] allRotation = { 0, 90, 180, -90 };// les rotations que peuvent effctuer les Object

    void Start()
    {
        for (byte i = 0; i < AllTile1.Length; i++)
        {
            if(Random.Range(0f, 100f) < probSDS)
            {
                Instantiate(dalleSpeciale[Random.Range(0, dalleSpeciale.Length)], AllTile1[i].transform.position, Quaternion.Euler(new Vector3(0,0,0))).transform.parent = transform;
                Destroy(AllTile1[i]);
            }
            if (Random.Range(0f, 100f) < probabilite && AllTile1[i] != null)
            {
                Instantiate(allObstacle1[Random.Range(0, allObstacle1.Length)], AllTile1[i].transform.position + offsetPos, Quaternion.Euler(new Vector3(0, allRotation[Random.Range(0, allRotation.Length)], 0))).transform.parent = transform;
            }
            else if(Random.Range(0f, 100f) < probSD && AllTile1[i] != null)
            {
                Instantiate(allDecore[Random.Range(0, allDecore.Length)], AllTile1[i].transform.position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0))).transform.parent = transform;
            }
        }
        for (byte i = 0; i < AllTile2.Length; i++)
        {
            if (Random.Range(0f, 100f) < probSDS)
            {
                Instantiate(dalleSpeciale[Random.Range(0, dalleSpeciale.Length)], AllTile2[i].transform.position, Quaternion.Euler(new Vector3(0, 0, 0))).transform.parent = transform;
                Destroy(AllTile2[i]);
            }
            if (Random.Range(0f, 100f) < probabilite && AllTile2[i] != null)
            {
                if (Random.Range(0, 2) == 1)
                {
                    Instantiate(allObstacle1[Random.Range(0, allObstacle1.Length)], AllTile2[i].transform.position + offsetPos, Quaternion.Euler(new Vector3(0, allRotation[Random.Range(0, allRotation.Length)], 0))).transform.parent = transform;
                }
                else
                {
                    Instantiate(allObstacle2[Random.Range(0, allObstacle2.Length)], AllTile2[i].transform.position + offsetPos, Quaternion.Euler(new Vector3(0, allRotation[Random.Range(0, allRotation.Length)], 0))).transform.parent = transform;
                }
            }
            else if (Random.Range(0f, 100f) < probSD && AllTile2[i] != null)
            {
                Instantiate(allDecore[Random.Range(0, allDecore.Length)], AllTile2[i].transform.position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0))).transform.parent = transform;
            }
        }
    }

}
