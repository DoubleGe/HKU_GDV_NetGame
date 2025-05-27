using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public static class UserGlobalData
{
    public static int clientID;
}

public static class ServerGlobalData
{
    public static string sessionID;

    private static int joinedCount;
    public static Dictionary<int, ClientData> clients;

    public static void AddClient(int clientID, int UID)
    {
        if(clients == null) clients = new Dictionary<int, ClientData>();

        clients.Add(clientID, new ClientData(UID));
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

    public ClientData(int UID)
    {
        this.UID = UID;
    }
}