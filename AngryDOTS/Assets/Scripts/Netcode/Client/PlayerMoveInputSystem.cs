using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial struct PlayerMoveInputSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        //Arrow Key Input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        float2 mouseScreenPosition = float2.zero;
        mouseScreenPosition.x = Input.mousePosition.x;
        mouseScreenPosition.y = Input.mousePosition.y;

        foreach (var playerInput in 
            SystemAPI.Query<RefRW<PlayerMoveInput>>()
                .WithAll<GhostOwnerIsLocal>())
        {
            playerInput.ValueRW.Horizontal = horizontalInput;
            playerInput.ValueRW.Vertical = verticalInput;

            playerInput.ValueRW.mouseScreenSpacePosition = mouseScreenPosition;
        }
    }
}