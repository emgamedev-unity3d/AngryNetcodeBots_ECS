using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

public struct PlayerMoveInput : IInputComponentData
{
    public float Horizontal;
    public float Vertical;

    public float2 mouseScreenSpacePosition;
}

public struct PlayerMovementStateData : IComponentData
{
    [GhostField(Quantization = 1000)]
    public float forward;

    [GhostField(Quantization = 1000)]
    public float strafe;
}