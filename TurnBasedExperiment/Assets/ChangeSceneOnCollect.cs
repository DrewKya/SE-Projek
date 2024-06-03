using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnCollect : MonoBehaviour
{
    public int objectsToCollect = 6; // Number of objects required to change the scene
    public string tagToCollect = "CanPickUp"; // Tag of the objects to collect
    private int objectsCollected = 0; // Counter for collected objects

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagToCollect))
        {
            objectsCollected++;
            Destroy(other.gameObject); // Destroy the collected object

            if (objectsCollected >= objectsToCollect)
            {
                Destroy(gameObject);
                SceneManager.LoadScene(1); // Change to scene 1
            }
        }
    }
}