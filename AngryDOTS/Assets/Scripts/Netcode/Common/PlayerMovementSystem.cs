using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (input, moveSpeed, transform) in
            SystemAPI.Query<
                RefRO<PlayerMoveInput>,
                RefRO<PlayerMoveSpeed>,
                RefRW<LocalTransform>>()
                    .WithAll<Simulate>())
        {
            float speed = moveSpeed.ValueRO.speed * SystemAPI.Time.DeltaTime;

            var moveInput = new float2(input.ValueRO.Horizontal, input.ValueRO.Vertical);
            moveInput = math.normalizesafe(moveInput) * speed;
            transform.ValueRW.Position += new float3(moveInput.x, 0, moveInput.y);
        }
    }
}