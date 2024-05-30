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

        var mousePos = Input.mousePosition;

        float3 mouseScreenPosition = mousePos;

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