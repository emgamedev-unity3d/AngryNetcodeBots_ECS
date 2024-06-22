using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation |
    WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct PlayerDisconnectReceiveClientRpcSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        // query to search for an entity
        var builder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PlayerDisconnectRpc, // with a request to join a team
                ReceiveRpcCommandRequest>();// with a component to receive an rpc
              //^ the "send" rpc from the server gets translated to a receive on the clients

        state.RequireForUpdate(state.GetEntityQuery(builder));
    }

    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (playerDisconnectRpc, receiveRPC, disconnectServerRpcEntity)
            in SystemAPI.Query<PlayerDisconnectRpc, ReceiveRpcCommandRequest>()
                .WithEntityAccess())
        {
            if (receiveRPC.IsConsumed)
            {
                Debug.LogWarning("Disconnect Entity still exists!");
                return;
            }

            receiveRPC.Consume();

            // we don't need the entity holding the rpc anymore, message received
            entityCommandBuffer.DestroyEntity(disconnectServerRpcEntity);

            Debug.Log(
                $"Cleaning up visuals for clientId:{playerDisconnectRpc.clientIdThatDisconnected}");

            ClientVisualProxyManager.Instance.RemoveClientVisual(
                playerDisconnectRpc.clientIdThatDisconnected);
        }

        entityCommandBuffer.Playback(state.EntityManager);
    }
}