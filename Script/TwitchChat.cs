using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TwitchPlayer
{
    private string namePL = "";
    private GameObject PlayerObject = null;

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
    [SerializeField] private GameObject spawnPlayer = null;
    [SerializeField] private List<TwitchPlayer> joueurTwitch = new List<TwitchPlayer>();
    [SerializeField] private GameObject[] spawnPoint = null;
    [SerializeField] private GameObject particuleSpawn = null;

    void Start()
    {
        Connect();
        spawnPoint = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    void Update()
    {
        if (!twitchClient.Connected)
        {
            Connect();
        }
        ReadChat();
        StartCoroutine(StartPartie());
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
                var chatName = message.Substring(0, splitPoint);
                chatName = chatName.Substring(1);
                splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1);
                GameCommande(chatName, message);
                Debug.Log(chatName +" :" + message);
            }
        }
    }

    private void GameCommande(string name,string message)
    {
        char[] SplitMessage = message.ToLower().ToCharArray();
        bool result = false;

        for (byte i = 0; i < joueurTwitch.Count; i++)
        {
            if (joueurTwitch[i].GetPlayerObject() == null)
            {
                joueurTwitch.Remove(joueurTwitch[i]);
                i = 0;
            }
        }

        if (SplitMessage.Length >= 2)
        { 
            if(SplitMessage[0] == '!' && SplitMessage[1] == 'j')
            {
                for (byte i = 0; i < joueurTwitch.Count && !result; i++)
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
            else if(SplitMessage[0] == '!')
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

    private void CreatePlayer(string name)
    {
        int rdIDSpawn = UnityEngine.Random.Range(0,spawnPoint.Length);
        GameObject plSpawned = Instantiate(spawnPlayer, spawnPoint[rdIDSpawn].transform.position, Quaternion.identity);
        plSpawned.transform.GetChild(0).GetComponent<Player>().SetPlayerName(name);
        joueurTwitch.Add(new TwitchPlayer(name, plSpawned));
        Destroy(Instantiate(particuleSpawn, plSpawned.transform.position, Quaternion.identity),5);
    }

    IEnumerator StartPartie()
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
        yield return new WaitForSeconds(5f);
        while (true)
        {
            yield return null;
            //transform.Translate(Vector3.left * Time.fixedDeltaTime * 0.0008f);
        }
    }

}