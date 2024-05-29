//using Unity.Entities;
//using Unity.NetCode;
//using UnityEngine;
//
//[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation |
//    WorldSystemFilterFlags.ThinClientSimulation)]
//[UpdateAfter(typeof(NetworkReceiveSystemGroup))]
//public partial struct PlayerDisconnectCleanupVisualSystem : ISystem
//{
//    public void OnCreate(ref SystemState state)
//    {
//        state.RequireForUpdate<NetworkStreamDriver>();
//    }
//
//    public void OnUpdate(ref SystemState state)
//    {
//        var connectionEventsForClient = 
//            SystemAPI.GetSingleton<NetworkStreamDriver>().ConnectionEventsForTick;
//
//        foreach (var evt in connectionEventsForClient)
//        {
//            if(evt.State == ConnectionState.State.Disconnected)
//            {
//                Debug.Log(
//                    $"[{state.WorldUnmanaged.Name}] {evt.ToFixedString()} connectionId:{evt.Id.Value}!");
//
//                ClientVisualProxyManager.Instance.RemoveClientVisual(evt.Id.Value);
//            }
//        }
//    }
//}