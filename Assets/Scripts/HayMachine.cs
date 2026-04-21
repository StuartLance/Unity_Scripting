using UnityEngine;

public class HayMachine : MonoBehaviour
{
    public float movementSpeed;
    public float horizontalBoundary = 22;

    public GameObject hayBalePrefab; // 1
    public Transform haySpawnpoint; // 2
    public float shootInterval; // 3
    private float shootTimer; // 4

    private int shotCounter; // counts shots to make every 10th a powershot

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdateShooting();

    }

    private void UpdateMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal"); // 1

        if (horizontalInput < 0 && transform.position.x > -horizontalBoundary) // 1
        {
            transform.Translate(transform.right * -movementSpeed * Time.deltaTime);
        }
        else if (horizontalInput > 0 && transform.position.x < horizontalBoundary) // 2
        {
            transform.Translate(transform.right * movementSpeed * Time.deltaTime);
        }
    }

    private void UpdateShooting()
    {
        shootTimer -= Time.deltaTime; // 1

        if (shootTimer <= 0 && Input.GetKey(KeyCode.Space)) // 2
        {
            shootTimer = shootInterval; // 3
            ShootHay(); // 4
        }
    }

    private void ShootHay()
    {
        shotCounter++;
        bool isPowerShot = (shotCounter % 10) == 0; // every 10th shot is a powershot

        GameObject hay = Instantiate(hayBalePrefab, haySpawnpoint.position, Quaternion.identity);

        if (isPowerShot)
        {
            // Increase the hay's move speed for this instance only
            Move mover = hay.GetComponent<Move>();
            if (mover != null)
            {
                mover.movementSpeed *= 2f;
            }
        }
    }
}
