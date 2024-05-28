using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation |
    WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateAfter(typeof(SetupClientGhostWithModelSystem))]
public partial struct SetupPlayerClientWithCameraFollowSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SetupPlayerClientWithCamera>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (localTransform, ghostOwner, entity) in
            SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<GhostOwner>>()
                    .WithAll<PlayerTag>()
                    .WithAll<SetupPlayerClientWithCamera>()
                    .WithAll<Simulate>()
                    .WithAll<GhostOwnerIsLocal>()
                    .WithEntityAccess())
        {
            int clientId = ghostOwner.ValueRO.NetworkId;

            var clientPlayerVisualGO = 
                ClientVisualProxyManager.Instance.GetVisualModelForClient(clientId);

            ClientVisualProxyManager.Instance.SetupVirtualCameraFollowAndLookAt(
                clientPlayerVisualGO);

            entityCommandBuffer.RemoveComponent<SetupPlayerClientWithCamera>(entity);
        }

        entityCommandBuffer.Playback(state.EntityManager);
    }
}
