using UnityEngine;
using Unity.Networking.Transport;

public class ClientBehaviour : MonoBehaviour
{
    private NetworkDriver netDriver;
    private NetworkConnection netConnection;

    private void Start()
    {
        netDriver = NetworkDriver.Create();

        NetworkEndpoint endpoint = NetworkEndpoint.LoopbackIpv4.WithPort(7777);
        netConnection = netDriver.Connect(endpoint);
    }

    private void OnDestroy()
    {
        netDriver.Dispose();
    }

    private void Update()
    {
        netDriver.ScheduleUpdate().Complete();

        if (!netConnection.IsCreated)
        {
            return;
        }

        Unity.Collections.DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = netConnection.PopEvent(netDriver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server.");

                uint value = 1;
                netDriver.BeginSend(netConnection, out var writer);
                writer.WriteUInt(value);
                netDriver.EndSend(writer);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                uint value = stream.ReadUInt();
                Debug.Log($"Got the value {value} back from the server.");

                netConnection.Disconnect(netDriver);
                netConnection = default;
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server.");
                netConnection = default;
            }
        }
    }
}