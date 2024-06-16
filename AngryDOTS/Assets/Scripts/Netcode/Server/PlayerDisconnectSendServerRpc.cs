using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateAfter(typeof(NetworkReceiveSystemGroup))]
public partial struct PlayerDisconnectSendServerRpc : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkStreamDriver>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var connectionEventsForClient = 
            SystemAPI.GetSingleton<NetworkStreamDriver>().ConnectionEventsForTick;

        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var connectionEvent in connectionEventsForClient)
        {
            if (connectionEvent.State != ConnectionState.State.Disconnected)
                return;

            Debug.Log(
                $"[{state.WorldUnmanaged.Name}] {connectionEvent.ToFixedString()} connectionId:{connectionEvent.Id.Value}!");

            var disconnectedClientRpc = entityCommandBuffer.CreateEntity();

            // RPC to tell the server the client wants to join
            entityCommandBuffer.AddComponent(
                disconnectedClientRpc,
                new PlayerDisconnectRpc { clientIdThatDisconnected = connectionEvent.Id.Value });

            // RPC to tell the server which client requests to connect to it
            entityCommandBuffer.AddComponent(
                    disconnectedClientRpc,
                    new SendRpcCommandRequest
                    {
                        TargetConnection = Entity.Null
                        //^ setting this to Null, to breadcast the message to all clients
                    });
        }

        entityCommandBuffer.Playback(state.EntityManager);
    }
}