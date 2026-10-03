using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageScenes : MonoBehaviour
{
    public static ManageScenes Instance;
    
    public delegate void OnSceneEvent();
    public event OnSceneEvent OnNextLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void ReloadScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadNextLevel()
    {
        StartCoroutine(FadeOutThenLoadNewScene());
    }

    private IEnumerator FadeOutThenLoadNewScene()
    {
        FadeManager.Instance.DoFade(0,1,1,0);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        FadeManager.Instance.DoFade(1,0,1,0);

    }
}
