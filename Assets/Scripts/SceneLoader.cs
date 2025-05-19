
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A simple helper for loading Unity scenes by name or by build index.
/// Attach this to any GameObject in your scene. Then, in your UI Button
/// OnClick() list, drag the GameObject and select one of these methods.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Loads a scene by its exact name (without the .unity extension).
    /// </summary>
    /// <param name="sceneName">Name of the scene to load.</param>
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Loads a scene by its build index (as listed in Project Settings > Editor > Scene List).
    /// </summary>
    /// <param name="buildIndex">Build index of the scene.</param>
    public void LoadSceneByIndex(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}
