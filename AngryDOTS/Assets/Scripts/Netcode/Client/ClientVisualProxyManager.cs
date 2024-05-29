using Cinemachine;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;

public class ClientVisualProxyManager : MonoBehaviour
{
    public static ClientVisualProxyManager Instance => s_instance;
    static ClientVisualProxyManager s_instance;

    [SerializeField]
    private GameObject m_clientVisualPrefab;

    [SerializeField]
    private CinemachineVirtualCamera m_virtualCamera;

    private readonly Dictionary<int, GameObject> m_clientVisualModels = new();

    private void Awake()
    {
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
            s_instance = this;
    }

    public void SetupVirtualCameraFollowAndLookAt(GameObject m_playerVisual)
    {
        m_virtualCamera.Follow = m_playerVisual.transform;
        m_virtualCamera.LookAt = m_playerVisual.transform;
    }

    public void AddClientVisual(int clientId, LocalTransform localTransform)
    {
        if (m_clientVisualModels.ContainsKey(clientId))
        {
            Debug.LogError($"Error: client id:{clientId} already has a visual model");
            return;
        }

        m_clientVisualModels[clientId] = Instantiate(
            m_clientVisualPrefab,
            localTransform.Position,
            localTransform.Rotation);

        Debug.Log($"Added a new visual model for client id:{clientId}");
    }

    public GameObject GetVisualModelForClient(int clientId)
    {
        if (!m_clientVisualModels.ContainsKey(clientId))
        {
            Debug.LogError($"Error: client id:{clientId} does not have a visual model");
            return null;
        }

        return m_clientVisualModels[clientId];
    }

    public void RemoveClientVisual(int clientId)
    {
        if (!m_clientVisualModels.ContainsKey(clientId))
        {
            Debug.LogError($"Error: client id:{clientId} does not have a visual model");
            return;
        }

        m_clientVisualModels.Remove(clientId, out var clientVisual);

        DestroyImmediate(clientVisual);

        Debug.Log($"Removed the visual model for client id:{clientId}");
    }
}
