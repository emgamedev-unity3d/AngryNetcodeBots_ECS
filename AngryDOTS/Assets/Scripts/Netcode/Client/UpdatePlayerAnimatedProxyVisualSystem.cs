using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

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
        foreach (var (ghostOwner, localTransform, playerMovementStateData) in
            SystemAPI.Query<
                RefRO<GhostOwner>,
                RefRO<LocalTransform>,
                RefRO<PlayerMovementStateData>>()
                    .WithAll<PlayerTag>())
        {
            int clientId = ghostOwner.ValueRO.NetworkId;

            var clientPlayerVisualGO = 
                ClientVisualProxyManager.Instance.GetVisualModelForClient(clientId);

            clientPlayerVisualGO.transform.position = localTransform.ValueRO.Position;
            clientPlayerVisualGO.transform.rotation = localTransform.ValueRO.Rotation;

            if (!clientPlayerVisualGO.TryGetComponent(out Animator playerAnimator))
                return;
            
            playerAnimator.SetFloat("Forward", playerMovementStateData.ValueRO.forward);
            playerAnimator.SetFloat("Strafe", playerMovementStateData.ValueRO.strafe);
        }
    }
}

