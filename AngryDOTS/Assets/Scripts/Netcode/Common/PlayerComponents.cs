using Unity.Entities;
using Unity.NetCode;

public struct PlayerMoveSpeed : IComponentData
{
    public float speed;
}

public struct PlayerMoveInput : IInputComponentData
{
    public float Horizontal;
    public float Vertical;
}