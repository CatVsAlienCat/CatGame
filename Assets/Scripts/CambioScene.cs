using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioScene : MonoBehaviour
{
    public GameObject Player;   
    public GameObject Enemy_King;
    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerMovement player = collision.collider.GetComponent<PlayerMovement>();
            if (player != null)
            {
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(currentSceneIndex + 1);
            }
    }
/*
    void Update()
    {
        Victoria();
        Derrota();
    }
    protected virtual void Derrota()
    {
        if (Player.GetComponent<PlayerMovement>().Health <= 0){
            SceneManager.LoadScene(5);  
        }
        
    }
    protected virtual void Victoria()
    {
        if (Enemy_King.GetComponent<Enemy_King>().Health <= 0){
            SceneManager.LoadScene(4);  
        }
        
    }
    */
}
