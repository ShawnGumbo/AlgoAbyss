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
    private MinimapRenderer minimap;

    private Vector3 resetPosition = new Vector3(2f, 4.5f, 2f);
    private Vector2Int goalCell;

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
        minimap = FindObjectOfType<MinimapRenderer>();

        if (goalSound == null)
            Debug.LogWarning("Goal sound is not assigned!");

        if (mazeGenerator != null)
        {
            // Exit cell on your maze: (width - 2, depth - 2)
            int exitX = mazeGenerator.width - 2;
            int exitZ = mazeGenerator.depth - 2;
            goalCell = new Vector2Int(exitX, exitZ);

            // Set goal GameObject position to exit (scale by cellSize)
            Vector3 goalWorldPos = new Vector3(exitX * mazeGenerator.cellSize, transform.position.y, exitZ * mazeGenerator.cellSize);
            transform.position = goalWorldPos;

            // Mark goal on minimap
            if (minimap != null)
            {
                minimap.SetGoalPosition(exitX, exitZ);
            }
        }
        else
        {
            Debug.LogWarning("MazeGenerator3D not found!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasWon)
        {
            hasWon = true;

            if (playerMovement != null)
                playerMovement.SetMovementEnabled(false);

            if (goalSound != null)
                goalSound.Play();

            StartCoroutine(CountdownAndRegenerate());
        }
    }

    private IEnumerator CountdownAndRegenerate()
    {
        yield return new WaitForSeconds(resetDelay);

        // Regenerate maze
        if (mazeGenerator != null)
            mazeGenerator.RegenerateMaze();

        // Reset player position
        if (playerRb != null)
        {
            playerRb.MovePosition(resetPosition);
            Physics.SyncTransforms();
            playerRb.linearVelocity = Vector3.zero;
        }

        if (playerMovement != null)
            playerMovement.SetMovementEnabled(true);

        // Update goal position after regeneration
        if (mazeGenerator != null)
        {
            int exitX = mazeGenerator.width - 2;
            int exitZ = mazeGenerator.depth - 2;
            goalCell = new Vector2Int(exitX, exitZ);

            Vector3 goalWorldPos = new Vector3(exitX * mazeGenerator.cellSize, transform.position.y, exitZ * mazeGenerator.cellSize);
            transform.position = goalWorldPos;

            if (minimap != null)
                minimap.SetGoalPosition(exitX, exitZ);
        }

        hasWon = false;
    }
}
