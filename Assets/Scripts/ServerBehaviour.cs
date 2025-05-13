using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

public class ServerBehaviour : MonoBehaviour
{
    private NetworkDriver netDriver;
    private NativeList<NetworkConnection> netConnection;
    
    void Start()
    {
        netDriver = NetworkDriver.Create();
        netConnection = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        NetworkEndpoint endpoint = NetworkEndpoint.AnyIpv4.WithPort(7777);
        if (netDriver.Bind(endpoint) != 0)
        {
            Debug.LogError("Failed to bind to port 7777.");
            return;
        }
        netDriver.Listen();
    }

    private void OnDestroy()
    {
        if (netDriver.IsCreated)
        {
            netDriver.Dispose();
            netConnection.Dispose();
        }
    }

    void Update()
    {
        netDriver.ScheduleUpdate().Complete();

        for (int i = 0; i < netConnection.Length; i++)
        {
            if (!netConnection[i].IsCreated)
            {
                netConnection.RemoveAtSwapBack(i);
                i--;
            }
        }

        NetworkConnection c;
        while ((c = netDriver.Accept()) != default)
        {
            netConnection.Add(c);
            Debug.Log("Accepted a connection.");
        }

        for (int i = 0; i < netConnection.Length; i++)
        {
            DataStreamReader stream;
            NetworkEvent.Type cmd;
            while ((cmd = netDriver.PopEventForConnection(netConnection[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    uint number = stream.ReadUInt();
                    Debug.Log($"Got {number} from a client, adding 2 to it.");
                    number += 2;

                    netDriver.BeginSend(NetworkPipeline.Null, netConnection[i], out var writer);
                    writer.WriteUInt(number);
                    netDriver.EndSend(writer);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from the server.");
                    netConnection[i] = default;
                    break;
                }
            }
        }
    }
}
