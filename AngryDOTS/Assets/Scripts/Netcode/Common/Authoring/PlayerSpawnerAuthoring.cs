using Unity.Entities;
using UnityEngine;

public class PlayerSpawnerAuthoring : MonoBehaviour
{
    [SerializeField]
    private GameObject m_playerPrefab;

    public class PlayerSpawnerBaker : Baker<PlayerSpawnerAuthoring>
    {
        public override void Bake(PlayerSpawnerAuthoring authoring)
        {
            var playerSpawnerEntity = GetEntity(TransformUsageFlags.None);
            AddComponent(playerSpawnerEntity, new PlayerSpawner
            {
                playerPrefab = GetEntity(authoring.m_playerPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}
