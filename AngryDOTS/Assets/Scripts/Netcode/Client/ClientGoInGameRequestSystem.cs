using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;


[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation |
    WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ClientGoInGameRequestSystem : ISystem
{
    private EntityQuery m_pendingNetworkIdQuery;

    public void OnCreate(ref SystemState state)
    {
        // query to search for an entity
        var builder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<NetworkId>() // with a client network id
            .WithNone<NetworkStreamInGame>(); // but no connection established to the server

        m_pendingNetworkIdQuery = state.GetEntityQuery(builder);
        state.RequireForUpdate(m_pendingNetworkIdQuery);
        state.RequireForUpdate<NewClientJoinRequest>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        var pendingNetworkIds = m_pendingNetworkIdQuery.ToEntityArray(Allocator.Temp);

        foreach (var pendingNetworkId in pendingNetworkIds)
        {
            // add a network stream component to the client
            entityCommandBuffer.AddComponent<NetworkStreamInGame>(pendingNetworkId);

            // entity to hold the rpc component to connect to server
            var requestTeamEntity = entityCommandBuffer.CreateEntity();

            // RPC to tell the server the client wants to join
            entityCommandBuffer.AddComponent(
                requestTeamEntity, new GoInGameRequest { });

            // RPC to tell the server which client requests to connect to it
            entityCommandBuffer.AddComponent(
                requestTeamEntity,
                new SendRpcCommandRequest { TargetConnection = pendingNetworkId });
        }

        entityCommandBuffer.Playback(state.EntityManager);
    }
}
