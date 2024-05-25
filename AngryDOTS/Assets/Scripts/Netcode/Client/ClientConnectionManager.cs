using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ClientConnectionManager : MonoBehaviour
{
    [SerializeField]
    UIDocument m_connectionMenu;

    private DropdownField m_connectionModeDropdown;
    private TextField m_connectionAddressTextField;
    private TextField m_connectionPortTextField;
    private Button m_connectButton;

    private const int k_CONNECTION_MODE_HOST = 0;
    private const int k_CONNECTION_MODE_CLIENT = 1;
    private const int k_CONNECTION_MODE_SERVER = 2;

    private string Address => m_connectionAddressTextField.text;
    private ushort Port => ushort.Parse(m_connectionPortTextField.text);

    // Start is called before the first frame update
    private void Start()
    {
        m_connectionModeDropdown = m_connectionMenu.rootVisualElement.Q<DropdownField>();

        m_connectionAddressTextField =
            m_connectionMenu.rootVisualElement.Q<TextField>("ConnectionAddressTextField");
        m_connectionPortTextField =
            m_connectionMenu.rootVisualElement.Q<TextField>("ConnectionPortTextField");

        m_connectButton = m_connectionMenu.rootVisualElement.Q<Button>();

        m_connectButton.clicked += OnConnectButtonClicked;

        m_connectionModeDropdown.RegisterValueChangedCallback(OnDropdownValueChanged);
    }

    private void OnDisable()
    {
        m_connectionModeDropdown.UnregisterValueChangedCallback(OnDropdownValueChanged);
        m_connectButton.clicked -= OnConnectButtonClicked;
    }

    private void OnDropdownValueChanged(ChangeEvent<string> evt)
    {
        switch (m_connectionModeDropdown.index)
        {
            case k_CONNECTION_MODE_HOST:
                m_connectButton.text = "Start as Host";
                break;

            case k_CONNECTION_MODE_CLIENT:
                m_connectButton.text = "Start as Client";
                break;

            case k_CONNECTION_MODE_SERVER:
                m_connectButton.text = "Start as Server";
                break;
        }
    }

    private void OnConnectButtonClicked()
    {
        DestroyLocalSimulationWorld();
        SceneManager.LoadScene(1); // loading the moba scene

        switch (m_connectionModeDropdown.index)
        {
            case k_CONNECTION_MODE_HOST:
                StartServer();
                StartClient();
                break;

            case k_CONNECTION_MODE_CLIENT:
                StartClient();
                break;

            case k_CONNECTION_MODE_SERVER:
                StartServer();
                break;

            default:
                Debug.LogError("Error: Unknown connection mode", gameObject);
                break;
        }
    }

    private void DestroyLocalSimulationWorld()
    {
        // destroying the default worlds, since we don't need them, and we'd like to create
        // client/server worlds as needed

        foreach (var world in World.All)
        {
            if (world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break; // we found it, so break!
            }
        }
    }
    private void StartServer()
    {
        var serverWorld = ClientServerBootstrap.CreateServerWorld("DOTS Server World");

        // what ip and port to listen for incoming connections
        var serverEndpoint = NetworkEndpoint.AnyIpv4.WithPort(Port);
        {
            using var networkDriverQuery = serverWorld.EntityManager.CreateEntityQuery(
                ComponentType.ReadWrite<NetworkStreamDriver>());

            networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(
                serverEndpoint);
        }
    }

    private void StartClient()
    {
        var clientWorld = ClientServerBootstrap.CreateClientWorld("DOTS Client World");

        // what ip and port to connect to
        var clientEndpoint = NetworkEndpoint.Parse(Address, Port);
        {
            using var networkDriverQuery = clientWorld.EntityManager.CreateEntityQuery(
                ComponentType.ReadWrite<NetworkStreamDriver>());

            networkDriverQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(
                clientWorld.EntityManager,
                clientEndpoint);
        }

        // creating entity that represents the request to join the game
        var teamRequestEntity = clientWorld.EntityManager.CreateEntity();
        clientWorld.EntityManager.AddComponentData(
            teamRequestEntity,
            new NewClientJoinRequest { });

        // doing this because we disposed the default world
        World.DefaultGameObjectInjectionWorld = clientWorld;
    }
}
