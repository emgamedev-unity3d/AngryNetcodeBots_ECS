using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Server
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ServerProcessGameEntryRequestSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            // query to search for an entity
            var builder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GoInGameRequest, // with a request to join a team
                ReceiveRpcCommandRequest>();// with a component to receive an rpc
                                            //^ the "send" rpc from the client gets translated to a
                                            //  receive on the server

            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (teamRequest, receiveRPC, requestEntity) 
                in SystemAPI.Query<GoInGameRequest, ReceiveRpcCommandRequest>()
                    .WithEntityAccess())
            {
                // we don't need the entity holding the rpc anymore, message received
                entityCommandBuffer.DestroyEntity(requestEntity);

                entityCommandBuffer.AddComponent<NetworkStreamInGame>(
                    receiveRPC.SourceConnection);
                // ^ same target connection specified in client

                int clientId = SystemAPI.GetComponent<NetworkId>(receiveRPC.SourceConnection).Value;

                Debug.Log($"Newly connected client with id: {clientId}");

                // TODO: Instantiate player here
            }

            entityCommandBuffer.Playback(state.EntityManager);
        }
    }
}