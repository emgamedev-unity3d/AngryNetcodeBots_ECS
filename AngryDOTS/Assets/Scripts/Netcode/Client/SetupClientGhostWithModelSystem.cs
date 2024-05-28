using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation |
    WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateBefore(typeof(SetupPlayerClientWithCameraFollowSystem))]
public partial struct SetupClientGhostWithModelSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SetupClientGhostWithModel>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (localTransform, ghostOwner, entity) in 
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<GhostOwner> >()
                    .WithAll<PlayerTag>()
                    .WithAll<SetupClientGhostWithModel>()
                    .WithAll<Simulate>()
                    .WithEntityAccess())
        {
            int clientId = ghostOwner.ValueRO.NetworkId;

            ClientVisualProxyManager.Instance.AddClientVisual(
                clientId,
                localTransform.ValueRO);

            entityCommandBuffer.RemoveComponent<SetupClientGhostWithModel>(entity);
        }

        entityCommandBuffer.Playback(state.EntityManager);
    }
}