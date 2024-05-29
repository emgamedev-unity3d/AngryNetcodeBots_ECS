using Unity.Entities;
using Unity.NetCode;

public struct NewClientJoinRequest : IComponentData
{
}

public struct SetupPlayerClientWithCamera : IComponentData
{
}

public struct SetupClientGhostWithModel : IComponentData
{
}

public struct PlayerIsDisconnecting : IComponentData
{
}