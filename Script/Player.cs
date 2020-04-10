using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;// pour utiliser le NavMeshAgent
using TMPro;//pour utiliser les TextMeshPro

public class Player : MonoBehaviour
{
    private NavMeshAgent agent = null;//Permet de controller L'IA 
    private Animator animator = null;//Permet de jouer des animation via des parametre
    [SerializeField] private string playerName = "";//le non du joueur une variable un peut inutile car c'est deja renseigner dans la base de donner mais bon.
    [SerializeField] private GameObject target = null;//L'object que suis en permanance L'IA
    [SerializeField] private char[] instructionPL = null;// les instruction char par char que va lire L'IA
    [SerializeField] private int tabPos = 0;//La position dans le tableaux des instruction pour savoir ou elle ce situe dans les instructions.
    [SerializeField] private float distanceChange = 0.5f;//Distance Avant de commencer a prevoir la prochaine instruction pour corriger le faite que isStoped sois long a demarer et donc quil ne face pas plusieur instruction a la suite
    [SerializeField] private TextMeshPro textName = null;//le texte qui est au dessus de la tete du PLAYER
    [SerializeField] private Material[] mPL = null;// un tableaux de materiaux qui correspond au different skill
    [SerializeField] private Vector3 precedentePos = new Vector3(0,0,0);//la position precedent au cas ou il ce bouffe un mur le Target peut donc revenire a la position de base.
    [SerializeField] private GameObject deadParticule = null;// Particule que l'on fait spawn a la mort du personnage
    private Renderer m_Renderer = null;// le rendu du personnage pour lui appliquer le Tableaux de materiaux PL
    private bool isDead = false;//etat de mort
    private bool onObstacle = false;//etat Obstacle si il ce boufe un mur il ne pourat momentanement rien faire

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();// On recuperer l'agent dans la scene il va le chercher sur la position du script.
        animator = transform.GetChild(0).GetComponent<Animator>();// on recupe L'animator
        m_Renderer = transform.GetChild(0).GetChild(0).GetComponent<Renderer>();//le renderer
        m_Renderer.material = mPL[Random.Range(0, mPL.Length)];//on attribut un materiaux au hasard a perso
        textName.color = new Color32((byte)Random.Range(0, 150), (byte)Random.Range(0, 150), (byte)Random.Range(0, 150),255);// on attrubut une couleur au hasard au texte du nom du perso
        precedentePos = target.transform.position;//on assigne la valeur de la position precedent pour eviter les bugs si le joueur percute un obstacle au premier deplacement
    }

    void Update()
    {
        RaycastHit hit;
        if (!Physics.Raycast(target.transform.position, target.transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))// Raycast pour detecter si Target et dans le vide 
        {
            if (instructionPL.Length > 0 && tabPos <= instructionPL.Length)
            {
                isDead = true;//meurt
                StartCoroutine(Saut());// on appele  la coroutine saut
                Destroy(transform.parent.gameObject, 1f);//on detruit le joueur au bout de une seconde  
            }
        }
        if (!isDead && !onObstacle)//si il n'est pas mort et quil n'est pas entreint de percuter un obstacle
        {
            if (distanceChange > Vector3.Distance(target.transform.position, transform.position) && !agent.isStopped)//si il a effectuer ça dernierre instruction et quil ne bouge plus
            {
                if (instructionPL.Length > 0 && tabPos < instructionPL.Length)//si il a des instruction quil na pas encore effectuer
                {
                    precedentePos = target.transform.position;//on mais a jour la position precedente avant de changer celle de Target
                    if (instructionPL[tabPos] == 'z')//Haut
                    {
                        target.transform.position = new Vector3(target.transform.position.x + 2, target.transform.position.y, target.transform.position.z);
                    }
                    if (instructionPL[tabPos] == 's')//bas
                    {
                        target.transform.position = new Vector3(target.transform.position.x - 2, target.transform.position.y, target.transform.position.z);
                    }
                    if (instructionPL[tabPos] == 'q')//gauche
                    {
                        target.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z + 2);
                    }
                    if (instructionPL[tabPos] == 'd')//droite
                    {
                        target.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z - 2);
                    }
                    tabPos++;//on incremente pour la prochaine instruction.
                }
            }
            agent.SetDestination(target.transform.position);//l'agent suis La Target
        }
        animator.SetFloat("Speed", agent.desiredVelocity.magnitude);//on asigne la vitesse de l'agent a celle de l'animator 
    }

    public void SetInstruction(char[] instruction)//on mais a jour le tableaux des instruction et tabPos pour recommencer au debut des instructions.
    {
        instructionPL = instruction;
        tabPos = 1;//pas 0 car ! n'est pas conte comme instruction
    }

    public void SetPlayerName(string name)//on mais a jour le non du joueur et l'affiche sur le texte
    {
        this.playerName = name;
        textName.text = name;
    }

    public IEnumerator Obstacle()//si il percute un obstacle cette fonction est appeler sur un script qui est sur Target
    {
        onObstacle = true;
        animator.SetBool("Wall",true);
        yield return new WaitForSeconds(0.3f);
        agent.enabled = false;
        yield return new WaitForSeconds(1f);
        agent.enabled = true;
        target.transform.position = precedentePos;
        instructionPL = new char[2];
        tabPos = 0;
        onObstacle = false;
    }

    public IEnumerator Dead()// si il meurt
    {
        isDead = true;
        yield return new WaitForSeconds(0.2f);
        agent.enabled = false;
        animator.SetBool("DeadSol",true);
        yield return new WaitForSeconds(3.5f);
        Destroy(Instantiate(deadParticule,target.transform.position,Quaternion.identity),5);
        Destroy(transform.parent.gameObject);
    }

    IEnumerator Saut()// si il saut donc il meurt
    {
        agent.enabled = false;
        animator.SetBool("Jump", true);
        float timer = 0.3f;
        while (true)
        {
            transform.position = new Vector3(Mathf.Lerp(transform.position.x,target.transform.position.x, Time.deltaTime * 0.2f), transform.position.y, Mathf.Lerp(transform.position.z, target.transform.position.z, Time.deltaTime * 0.2f));
            timer -= Time.deltaTime;
            if (timer >= 0)
            {
                transform.Translate(Vector3.up * Time.deltaTime * 0.3f, Space.World);
            }
            else
            {
                transform.Translate(Vector3.down * Time.deltaTime * 1.5f, Space.World);
            }
            yield return null;
        }
    }
}
