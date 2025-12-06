using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Game"; // Allow user to type scene name in Inspector

    public void PlayGame()
    {
        Debug.Log("Play Button Clicked!");
        
        // Try to load by next index first
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        // Check if next index is valid within build settings count (only possible if scenes are added)
        if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
        {
             SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning($"Scene Index {nextSceneIndex} not found. Trying to load by name: {gameSceneName}");
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();
    }
}
