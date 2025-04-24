using UnityEngine.SceneManagement;
using UnityEngine;

public class AudioManager : MonoBehaviour
{   
    // VARIABLES.
    private string sceneName;

    // FUNCTIONS.
    void Update()
    {
        sceneName = SceneManager.GetActiveScene().name;
        // Audio keeps playing through selected scenes.
        if ((sceneName == "Tutorial1") || (sceneName == "Tutorial2") || (sceneName == "Tutorial3"))
        { 
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}