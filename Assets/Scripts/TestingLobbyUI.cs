using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField] private Button clientButton;

    private void Awake()
    {
        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }
}
