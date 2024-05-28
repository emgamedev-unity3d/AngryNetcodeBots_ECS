using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation |
    WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct UpdatePlayerAnimatedProxyVisualSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (ghostOwner, localTransform) in
            SystemAPI.Query<
                RefRO<GhostOwner>,
                RefRO<LocalTransform>>()
                    .WithAll<PlayerTag>())
        {
            int clientId = ghostOwner.ValueRO.NetworkId;

            var clientPlayerVisualGO = 
                ClientVisualProxyManager.Instance.GetVisualModelForClient(clientId);

            clientPlayerVisualGO.transform.position = localTransform.ValueRO.Position;
        }
    }
}

