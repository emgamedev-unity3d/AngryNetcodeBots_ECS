using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField]
    private float m_moveSpeed = 3.5f;

    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerTag> (entity);
            AddComponent<PlayerMoveInput>(entity);
            AddComponent<SetupPlayerClientWithCamera>(entity);
            AddComponent<SetupClientGhostWithModel>(entity);
            AddComponent(entity, new PlayerMoveSpeed { speed = authoring.m_moveSpeed});
        }
    }
}