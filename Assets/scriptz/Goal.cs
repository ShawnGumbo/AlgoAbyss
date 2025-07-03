using System.Collections;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public AudioSource goalSound;
    public float resetDelay = 5f;

    private bool hasWon = false;
    private PlayerMovement3D playerMovement;
    private MazeGenerator3D mazeGenerator;
    private Rigidbody playerRb;

    private Vector3 resetPosition = new Vector3(2f, 4.5f, 2f);

    void Start()
    {
        // Cache references
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement3D>();
            playerRb = player.GetComponent<Rigidbody>();
        }

        mazeGenerator = FindObjectOfType<MazeGenerator3D>();

        if (goalSound == null)
            Debug.LogWarning("Goal sound is not assigned!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasWon)
        {
            hasWon = true;

            // Disable player movement
            if (playerMovement != null)
                playerMovement.SetMovementEnabled(false);

            // Play sound
            if (goalSound != null)
                goalSound.Play();

            // Start reset countdown
            StartCoroutine(CountdownAndRegenerate());
        }
    }

    private IEnumerator CountdownAndRegenerate()
    {
        yield return new WaitForSeconds(resetDelay);

        // Regenerate maze
        if (mazeGenerator != null)
            mazeGenerator.RegenerateMaze();

        // Reset player position and physics
        if (playerRb != null)
        {
            playerRb.MovePosition(resetPosition);
            Physics.SyncTransforms();
            playerRb.linearVelocity = Vector3.zero;
        }

        if (playerMovement != null)
            playerMovement.SetMovementEnabled(true);

        hasWon = false;
    }
}
