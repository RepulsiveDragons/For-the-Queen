using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private void Awake()
    {
        startButton.onClick.AddListener(() => {
            //Loader.Load(Loader.Scene.LobbyMenu);
            SceneManager.LoadScene("LobbyMenu");
        });

        Time.timeScale = 1f;
    }
}
