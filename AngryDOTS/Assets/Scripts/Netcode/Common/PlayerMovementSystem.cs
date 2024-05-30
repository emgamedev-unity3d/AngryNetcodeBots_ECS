using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct PlayerMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (input, moveSpeed, transform, playerMovementStateData) in
            SystemAPI.Query<
                RefRO<PlayerMoveInput>,
                RefRO<MoveSpeed>,
                RefRW<LocalTransform>,
                RefRW<PlayerMovementStateData>>()
                    .WithAll<Simulate>())
        {
            float speed = moveSpeed.ValueRO.Value * SystemAPI.Time.DeltaTime;

            var moveInput = new float2(input.ValueRO.Horizontal, input.ValueRO.Vertical);
            moveInput = math.normalizesafe(moveInput) * speed;
            transform.ValueRW.Position += new float3(moveInput.x, 0f, moveInput.y);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                float3 hitPoint = hit.point;

                Vector3 playerToMouse = hitPoint - transform.ValueRO.Position;
                playerToMouse.y = 0f;
                playerToMouse.Normalize();

                Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
                transform.ValueRW.Rotation = newRotation;
            }

            //TODO: for animation data

            ////Camera Direction
            //var cameraForward = Camera.main.transform.forward;
            //var cameraRight = Camera.main.transform.right;
            //cameraForward.y = 0f;
            //cameraRight.y = 0f;

            //Vector3 desiredDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;
            //Vector3 movement = new Vector3(desiredDirection.x, 0f, desiredDirection.z);
            //float forw = Vector3.Dot(movement, math.forward(transform.ValueRO.Rotation));
            //float stra = Vector3.Dot(movement, math.right());

            //if (math.lengthsq(moveInput) >= 0.001f)
            //{

            //}
            //else
            //{
            //}
        }
    }
}