using Unity.Entities;
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

        foreach (var playerInput in 
            SystemAPI.Query<RefRW<PlayerMoveInput>>()
                .WithAll<GhostOwnerIsLocal>())
        {
            playerInput.ValueRW.Horizontal = horizontalInput;
            playerInput.ValueRW.Vertical = verticalInput;
        }
    }
}