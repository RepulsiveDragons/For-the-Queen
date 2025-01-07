using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class Loader : MonoBehaviour
{
    public enum Scene
    {
        Main,
        MainMenu,
        LobbyMenu,
        LobbyScene,
        LoadingScene,
    }

    private static Scene targetScene;

    public static void Load (Scene targetScene)
    {
        Loader.targetScene = targetScene;
        SceneManager.LoadScene(Scene.MainMenu.ToString());
    }

    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}
