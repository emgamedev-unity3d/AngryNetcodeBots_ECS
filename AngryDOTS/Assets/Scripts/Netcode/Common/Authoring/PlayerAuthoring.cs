using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField]
    private float m_moveSpeed = 4.5f;

    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerTag>(entity);
            AddComponent<PlayerMoveInput>(entity);
            AddComponent<SetupPlayerClientWithCamera>(entity);
            AddComponent<SetupClientGhostWithModel>(entity);
            AddComponent(entity, new MoveSpeed { Value = authoring.m_moveSpeed });
            AddComponent(
                entity,
                new PlayerMovementStateData { forward = 0f, strafe = 0f });
        }
    }
}