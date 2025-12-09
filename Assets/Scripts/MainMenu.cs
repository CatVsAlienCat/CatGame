using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Game";

    public void PlayGame()
    {
        Debug.Log("¡Botón Play presionado!");
        
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning($"Escena con índice {nextSceneIndex} no encontrada. Intentando cargar por nombre: {gameSceneName}");
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void QuitGame()
    {
        Debug.Log("SALIR DEL JUEGO");
        Application.Quit();
    }
}
