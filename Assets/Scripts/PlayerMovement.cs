using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // You can change this speed in the Unity Inspector
    private Rigidbody2D rb;
    private Vector2 moveInput;
    public Camera mainCamera;
    private Vector2 mousePosition;

    public GameObject bulletPrefab; // The prefab you just made
    public Transform firePoint;     // An empty GameObject at the "tip" of your player

    // Start is called before the first frame update
    void Start()
    {
        // Get the Rigidbody component we attached to this player
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from keyboard (Arrow keys or WASD)
        float moveX = Input.GetAxisRaw("Horizontal"); // -1 for left, 1 for right
        float moveY = Input.GetAxisRaw("Vertical");   // -1 for down, 1 for up

        moveInput = new Vector2(moveX, moveY).normalized; // .normalized stops you from moving faster diagonally
        mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Check for Left-Click
        if (Input.GetButtonDown("Fire1")) // "Fire1" is Left-Click by default
        {
            Shoot();
        }
    }

    // FixedUpdate is called on a fixed physics timer (better for physics)
    void FixedUpdate()
    {
        // Apply the movement to the Rigidbody's velocity
        rb.linearVelocity = moveInput * moveSpeed;

        // Calculate the direction from the player to the mouse
        Vector2 lookDirection = mousePosition - rb.position;
    
        // Calculate the angle and apply it to the player's rotation
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;
    }

    void Shoot()
    {
        // Create a new bullet from the prefab, at the firePoint's position and rotation
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}