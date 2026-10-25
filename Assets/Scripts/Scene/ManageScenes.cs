using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageScenes : MonoBehaviour
{
    public static ManageScenes Instance;

    [Header("Transition Settings")]
    [Tooltip("Delay in seconds before loading a scene after a button click.")]
    public float loadDelay = 1.0f;

    private GameObject fadeCanvas;
    private FadeManager fadeManager;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        fadeCanvas = GameObject.FindWithTag("FadeCanvas");
        fadeManager =  fadeCanvas.GetComponent<FadeManager>();
        
    }
    // --- Public UI Button Callbacks ---

    public void RestartGame()
    {
        StartCoroutine(LoadSceneWithDelay(2));
    }

    public void ReloadScene()
    {
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadSceneWithDelay(currentBuildIndex));
    }

    public void MainMenu()
    {
        StartCoroutine(LoadSceneWithDelay(0));
    }

    public void ExitGame()
    {
        StartCoroutine(ExitWithDelay());
    }

    public void LoadNextLevel()
    {
        StartCoroutine(FadeOutThenLoadNewScene());
    }

    public void UnloadLevel()
    {
        StartCoroutine(FadeOut());
    }

    // --- Coroutines & Helper Methods ---

    private IEnumerator LoadSceneWithDelay(int buildIndex)
    {
        if (fadeManager != null)
        {
            fadeManager.DoFade(0, 1, loadDelay, 0);
        }

        yield return new WaitForSeconds(loadDelay);
        SceneManager.LoadScene(buildIndex);

        if (fadeManager != null)
        {
            fadeManager.DoFade(1, 0, 1, 0);
        }
    }

    private IEnumerator ExitWithDelay()
    {
        if (fadeManager != null)
        {
            fadeManager.DoFade(0, 1, loadDelay, 0);
        }

        yield return new WaitForSeconds(loadDelay);
        Application.Quit();
    }

    private IEnumerator FadeOutThenLoadNewScene()
    {
        Debug.Log("FadeOutThenLoadNewScene");
        fadeManager.DoFade(0, 1, 1, 0);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private IEnumerator FadeOut()
    {
        if (fadeManager != null)
        {
            fadeManager.DoFade(0, 1, loadDelay, 0);
        }

        yield return new WaitForSeconds(loadDelay);
    }
    
    public IEnumerator LoadCutScene()
    {
        fadeManager.DoFade(0,1,1,0);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        fadeManager.DoFade(1,0,1,0);
    }
}