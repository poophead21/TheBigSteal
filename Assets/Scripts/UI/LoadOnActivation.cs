using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOnActivation : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(Fade());
    }
    
    private IEnumerator Fade()
    {
        if (ManageScenes.Instance != null)
        {
            yield return new WaitForSeconds(0.5f);
            ManageScenes.Instance.LoadNextLevel();
        }

    }
}
