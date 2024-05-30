using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct PlayeRotationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        if (!Application.isFocused)
            return;

        foreach (var (input, transform) in
            SystemAPI.Query<
                RefRO<PlayerMoveInput>,
                RefRW<LocalTransform>>()
                    .WithAll<Simulate>()
                    .WithAll<GhostOwnerIsLocal>())
        {
            Ray ray = Camera.main.ScreenPointToRay(input.ValueRO.mouseScreenSpacePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                float3 hitPoint = hit.point;

                Vector3 playerToMouse = hitPoint - transform.ValueRO.Position;
                playerToMouse.y = 0f;
                playerToMouse.Normalize();

                Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
                transform.ValueRW.Rotation = newRotation;
            }
        }
    }
}