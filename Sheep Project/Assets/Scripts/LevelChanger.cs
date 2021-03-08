using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Aurélien
/// </summary>
public class LevelChanger : MonoBehaviour
{
    public static LevelChanger lc; 

    void Awake()
    {
        if (lc != null)
        {
            Destroy(this.gameObject);
        }
        {
            lc = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void Previous()
    {
        int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

        if(currentSceneBuildIndex != 0)
        {
            SceneManager.LoadScene(currentSceneBuildIndex - 1);
        }
    }

    public void Next()
    {
        int currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneBuildIndex != (SceneManager.sceneCountInBuildSettings-1))
        {
            SceneManager.LoadScene(currentSceneBuildIndex + 1);
        }
    }
}
