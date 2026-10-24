using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageScenes : MonoBehaviour
{
    public static ManageScenes Instance;
    


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

    public void RestartGame()
    {
        SceneManager.LoadScene(2);
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
    
    public void UnloadLevel()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOutThenLoadNewScene()
    {
        yield return FadeOut();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        FadeManager.Instance.DoFade(1,0,1,0);
    }
    
    private IEnumerator FadeOut()
    {
        FadeManager.Instance.DoFade(0,1,1,0);
        yield return new WaitForSeconds(1);
    }

    /*public void LoadCutScene()
    {
        FadeManager.Instance.DoFade(0,1,1,0);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        FadeManager.Instance.DoFade(1,0,1,0);
    }*/
}
