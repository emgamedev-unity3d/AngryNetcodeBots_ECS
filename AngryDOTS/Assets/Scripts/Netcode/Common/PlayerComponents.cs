using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

public struct PlayerMoveInput : IInputComponentData
{
    public float Horizontal;
    public float Vertical;

    public float3 mouseScreenSpacePosition;
}

[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct FireGunInput : IInputComponentData
{
    [GhostField]
    public InputEvent fireGunAbility;
}

public struct PlayerMovementStateData : IComponentData
{
    [GhostField(Quantization = 1000)]
    public float forward;

    [GhostField(Quantization = 1000)]
    public float strafe;
}