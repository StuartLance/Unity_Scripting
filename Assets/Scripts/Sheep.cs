using System.Collections;
using UnityEngine;

public class Sheep : MonoBehaviour
{
    public float runSpeed; // 1
    public float gotHayDestroyDelay; // 2
    private bool hitByHay; // 3

    public float dropDestroyDelay; // 1
    private Collider myCollider; // 2
    private Rigidbody myRigidbody; // 3

    private SheepSpawner sheepSpawner;

    [Header("Jumping")]
    [Tooltip("Maximum vertical height of the jump (set in Unity Inspector).")]
    public float jumpHeight = 0.5f;
    [Tooltip("Duration of the jump in seconds.")]
    public float jumpDuration = 0.5f;
    [Tooltip("Minimum time between jump attempts (seconds).")]
    public float jumpIntervalMin = 2f;
    [Tooltip("Maximum time between jump attempts (seconds).")]
    public float jumpIntervalMax = 6f;

    private bool isJumping;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myCollider = GetComponent<Collider>();
        myRigidbody = GetComponent<Rigidbody>();

        // Start the scheduler that triggers random jumps while the sheep is active
        StartCoroutine(JumpScheduler());
    }

    public void SetSpawner(SheepSpawner spawner)
    {
        sheepSpawner = spawner;
    }

    // Update is called once per frame
    void Update()
    {
        // Always move forward while alive (jump coroutine only modifies vertical offset)
        transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
    }

    private void Drop()
    {
        sheepSpawner.RemoveSheepFromList(gameObject);

        myRigidbody.isKinematic = false; // 1
        myCollider.isTrigger = false; // 2
        Destroy(gameObject, dropDestroyDelay); // 3
    }


    private void HitByHay()
    {
        sheepSpawner.RemoveSheepFromList(gameObject);

        hitByHay = true; // 1
        runSpeed = 0; // 2

        Destroy(gameObject, gotHayDestroyDelay); // 3
    }

    private void OnTriggerEnter(Collider other) // 1
    {
        if (other.CompareTag("Hay") && !hitByHay) // 2
        {
            Destroy(other.gameObject); // 3
            HitByHay(); // 4
        }
        else if (other.CompareTag("DropSheep"))
        {
            Drop();
        }
    }

    private IEnumerator JumpScheduler()
    {
        // Keep scheduling jumps until the object is destroyed or marked hitByHay
        while (true)
        {
            float wait = Random.Range(jumpIntervalMin, jumpIntervalMax);
            yield return new WaitForSeconds(wait);

            // Conditions to attempt a jump:
            // - not already jumping
            // - not hit by hay
            // - Rigidbody is kinematic (so transform-based jump is appropriate) or absent
            if (!isJumping && !hitByHay && (myRigidbody == null || myRigidbody.isKinematic))
            {
                StartCoroutine(JumpOver());
            }
        }
    }

    private IEnumerator JumpOver()
    {
        isJumping = true;

        float baseY = transform.position.y;
        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            float t = elapsed / jumpDuration;
            float yOffset = 4f * jumpHeight * t * (1f - t); // simple parabola peak at t=0.5

            Vector3 pos = transform.position;
            pos.y = baseY + yOffset;
            transform.position = pos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final Y reset exactly
        Vector3 finalPos = transform.position;
        finalPos.y = baseY;
        transform.position = finalPos;

        isJumping = false;
    }
}
