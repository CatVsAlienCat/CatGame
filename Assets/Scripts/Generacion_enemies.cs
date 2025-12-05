using UnityEngine;

public class Generacion_enemies : MonoBehaviour
{
    
    public GameObject prefabEnemigo;
    public Transform puntoDeAparicion;
    public float tiempoDeSpawn = 4f;

    void Start()
    {
        InvokeRepeating("SpawnEnemigo", 4f, tiempoDeSpawn);
    }

    void SpawnEnemigo()
    {
        

        Instantiate(prefabEnemigo, puntoDeAparicion.position, Quaternion.identity);
    }
}
