using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

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

            state.RequireForUpdate<PlayerSpawner>();
            //^ required to spawn the player
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            var playerPrefab = SystemAPI.GetSingleton<PlayerSpawner>().playerPrefab;

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

                // Instantiate the player
                var newPlayer = entityCommandBuffer.Instantiate(playerPrefab);
                entityCommandBuffer.SetName(newPlayer, $"Player_client:{clientId}");

                float3 spawnPosition = NewRandomPosition();
                var newTransform = LocalTransform.FromPosition(spawnPosition);
                entityCommandBuffer.SetComponent(newPlayer, newTransform);

                entityCommandBuffer.SetComponent(
                    newPlayer,
                    new GhostOwner { NetworkId = clientId });
                // ^^ setting this on server side, to associate it with the proper client

                entityCommandBuffer.AppendToBuffer(
                    receiveRPC.SourceConnection, // bind to client connection
                    new LinkedEntityGroup { Value = newPlayer });
                // ^so that we can destroy this whent the client disconnects
            }

            entityCommandBuffer.Playback(state.EntityManager);
        }

        private readonly float3 NewRandomPosition()
        {
            return new float3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
        }
    }
}