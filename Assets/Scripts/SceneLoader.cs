using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Public method to load a scene by build index
    public void LoadSceneByNumber(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    // Public method to quit the game
    public void QuitGame()
    {
        Application.Quit();

       

    }
}