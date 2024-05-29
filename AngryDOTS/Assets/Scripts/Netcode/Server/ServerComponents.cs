using Unity.NetCode;

public struct PlayerDisconnectRpc : IRpcCommand
{
    public int clientIdThatDisconnected;
}