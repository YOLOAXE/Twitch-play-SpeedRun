using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

[System.Serializable]
public class TwitchPlayer// class Twitch Player assosie le nom a Object 
{
    private string namePL = "";//le nom de la persone sur twitch
    private GameObject PlayerObject = null;// l'object

    public TwitchPlayer(string name,GameObject plObject)
    {
        this.namePL = name;
        this.PlayerObject = plObject;
    }

    public string GetName()
    {
        return this.namePL;
    }

    public GameObject GetPlayerObject()
    {
        return this.PlayerObject;
    }
}

public class TwitchChat : MonoBehaviour
{

    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;

    [SerializeField] private string username = "", password = "", channelName = "";
    [SerializeField] private GameObject spawnPlayer = null;//l'object Player qui est spawn pour apres l'assigner sur un joueur
    [SerializeField] private List<TwitchPlayer> joueurTwitch = new List<TwitchPlayer>();// la liste des joueurs
    [SerializeField] private GameObject[] spawnPoint = null;// les points de spawn
    [SerializeField] private GameObject particuleSpawn = null;//la particule de spawn
    [SerializeField] private TextMeshProUGUI textConteur = null;// le text qui conte
    [SerializeField] private NavMeshSurface navSurface = null; // la navSurface qui permet au joueur de ce deplacer.
    [SerializeField] private float tADP = 30f;//Le temps avant de debut d'une partie
    [SerializeField] private float speedMult = 1f;// la vitesse de base de la camera elle augmente au fur et a mesur que la cam avence
    private string lastPlayerName = "";// le dernier joueur encore en vie sur la carte pour pouvoir afficher qui a ganger
    private float temps = 30f;// le temps qui est decrementer et qui prend la valeur de tADP.
    private bool TimerStart = false;// le timer de la partie decremente .
    private bool endPartie = false;// la fin de partie.
    private bool startPartie = false;// le dubut de la partie.
    private bool partieUnJoueur = false;// si il y a que un seul joueur.

    void Start()
    {
        Connect();//connection a twitch
        spawnPoint = GameObject.FindGameObjectsWithTag("SpawnPoint");// on recup tout les point de spawn dans un tableaux.
        temps = tADP;// on mais a jour le temps qui va etre decrementer au debut de la partie.
        StartCoroutine(StartPartieC());// on demare la coroutine de debut de partie on att les joueurs.
    }

    void Update()
    {
        if (!twitchClient.Connected)// si le joueur n'est pas co
        {
            Connect();
        }
        ReadChat();// on li le tchat
        if (TimerStart)// si le timer a deparer on decremente temps
        {
            temps -= Time.deltaTime;
            textConteur.text = Mathf.Round(temps).ToString();
            TimerStart = temps > 0;
            if(temps <= 0)
            {
                textConteur.text = "";
            }
        }
        if(temps <= 0)// si c'est la fin du timer alors la camera ce deplacer
        {
            speedMult += Time.deltaTime * 0.005f;//acceleration
            transform.Translate(Vector3.left * Time.deltaTime * 1.5f * speedMult);//deplacement
        }
    }

    private void Connect()
    {
        twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        reader = new StreamReader(twitchClient.GetStream());
        writer = new StreamWriter(twitchClient.GetStream());

        writer.WriteLine("PASS " + password);
        writer.WriteLine("NICK " + username);
        writer.WriteLine("USER " + username + " 8 * :" + username);
        writer.WriteLine("JOIN #" + channelName);
        writer.Flush();
    }

    private void ReadChat()
    {
        if (twitchClient.Available > 0)
        {
            var message = reader.ReadLine(); //Read in the current message
            if (message.Contains("PRIVMSG"))
            {
                var splitPoint = message.IndexOf("!", 1);
                var chatName = message.Substring(0, splitPoint);// on recup le non de la personne qui a ecrit le message
                chatName = chatName.Substring(1);
                splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1);//on recup le message
                GameCommande(chatName, message);// on traite le message
                Debug.Log(chatName +" :" + message);
            }
        }
    }
    private void removePlayerLeft()// on suprime les joueur de la lest TwitchPlayer qui non plus de GameObject car il a ete detruit
    {
        for (byte i = 0; i < joueurTwitch.Count; i++)
        {
            if (joueurTwitch[i].GetPlayerObject() == null)
            {
                joueurTwitch.Remove(joueurTwitch[i]);
                i = 0;
            }
        }
    }
    private void GameCommande(string name,string message)//on traite les message par utilisateur
    {
        char[] SplitMessage = message.ToLower().ToCharArray();// on cast le message en tableaux de character
        bool result = false;// 

        removePlayerLeft();

        if (SplitMessage.Length >= 2)//si le message fait plus de de charactere
        { 
            if(SplitMessage[0] == '!' && SplitMessage[1] == 'j' && !startPartie)// si le messsage commence par !j et que la partie n'est pas encore commencer
            {
                for (byte i = 0; i < joueurTwitch.Count && !result; i++)// pour chaque joueur twitch on test si n'est pas dans la list si oui on l'ajoute et on cree sont perso
                {
                    result = joueurTwitch[i].GetName() == name;
                }
                if(result)
                {
                    Debug.Log("Le joueur " + name +" existe deja.");
                }
                else
                {
                    CreatePlayer(name);
                }
            }
            else if(SplitMessage[0] == '!')// si il rentre une commande
            {
                for (byte i = 0; i < joueurTwitch.Count; i++)
                {
                    if(joueurTwitch[i].GetName() == name)
                    {
                        joueurTwitch[i].GetPlayerObject().transform.GetChild(0).GetComponent<Player>().SetInstruction(SplitMessage);
                    }
                }
            }
        }
    }

    private void CreatePlayer(string name)// on cree un joueur grace a sont nom
    {
        int rdIDSpawn = UnityEngine.Random.Range(0,spawnPoint.Length);
        GameObject plSpawned = Instantiate(spawnPlayer, spawnPoint[rdIDSpawn].transform.position, Quaternion.identity);
        plSpawned.transform.GetChild(0).GetComponent<Player>().SetPlayerName(name);
        joueurTwitch.Add(new TwitchPlayer(name, plSpawned));
        Destroy(Instantiate(particuleSpawn, plSpawned.transform.position, Quaternion.identity),5);
    }

    IEnumerator StartPartieC()// demare la partie
    {
        for (byte i = 0; i < joueurTwitch.Count; i++)
        {
            if (joueurTwitch[i].GetPlayerObject() == null)
            {
                joueurTwitch.Remove(joueurTwitch[i]);
                i = 0;
            }
        }
        while (joueurTwitch.Count == 0)
        {
            yield return new WaitForSeconds(0.1f);
        }
        TimerStart = true;
        while (TimerStart)
        {
            yield return null;
        }
        startPartie = true;
        partieUnJoueur = joueurTwitch.Count >= 2;
        StartCoroutine(EndGame());
    }
    IEnumerator EndGame()// on test si c'est la fin de la partie
    {
        while (!endPartie)
        {
            yield return new WaitForSeconds(1f);
            for (byte i = 0; i < joueurTwitch.Count; i++)
            {
                if (joueurTwitch[i].GetPlayerObject() != null)
                {
                    lastPlayerName = joueurTwitch[i].GetName();
                }
            }
            removePlayerLeft();
            if (partieUnJoueur)
            {
                endPartie = joueurTwitch.Count <= 1;
            }
            else
            {
                endPartie = joueurTwitch.Count == 0;
            }
            Debug.Log(endPartie + " " + partieUnJoueur + " " + joueurTwitch.Count);
        }
        if (lastPlayerName == "")
        {
            textConteur.text = "Personne n'a gagner.";
        }
        else
        {
            textConteur.text = lastPlayerName + " a gagner.";
        }
        yield return new WaitForSeconds(5f);
        Application.LoadLevel(0);
    }
    void OnTriggerEnter(Collider other)// on cree la prochaine Tile si on touche le trigger d'une deja existante
    {
        if(other.transform.tag == "Disposition")
        {
            other.transform.gameObject.GetComponent<TileManager>().Spawn();
            navSurface.BuildNavMesh();
        }
    }
    void OnTriggerExit(Collider other)// si on sort dune Tile on la detruit
    {
        if (other.transform.tag == "Disposition")
        {
            Destroy(other.transform.parent.gameObject);
        }
    }
}