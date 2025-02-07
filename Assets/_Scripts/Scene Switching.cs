using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] public string sceneName; // The name of the scene to load

    public void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has the Player tag
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
