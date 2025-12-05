using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioScene : MonoBehaviour
{
    public GameObject Player;
    public int i=1;
   
   void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerMovement player = collision.collider.GetComponent<PlayerMovement>();
            if (player != null)
            {
                i++;
                SceneManager.LoadScene(i);
                

            }
        
    }
}
