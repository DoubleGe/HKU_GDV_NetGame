using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public static class UserGlobalData
{
    public static int clientID;
    public static bool isMyTurn;
}

public static class ServerGlobalData
{
    public static string sessionID;

    private static int joinedCount;
    public static Dictionary<int, ClientData> clients;

    public static void AddClient(int clientID, int UID, string username)
    {
        if(clients == null) clients = new Dictionary<int, ClientData>();

        clients.Add(clientID, new ClientData(UID, username));
    } 

    public static bool RemoveClient(int clientID)
    {
        if(clients == null) return false;

        if (clients.ContainsKey(clientID))
        {
            clients.Remove(clientID);
            return true;
        }
        return false;
    }
}

public class ClientData
{
    public int UID;
    public string username;

    public ClientData(int UID, string username)
    {
        this.UID = UID;
        this.username = username;
    }
}