using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    // VARIABLES
    public string FirstScene, SecondScene;
    public Sprite playImage, pauseImage;
    public Button pauseButton;
    public VideoPlayer videoPlayer;

    private string nextScene;

    // FUNCTIONS
    public void Active() 
    { // Set Scenes.
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == FirstScene)
        {
            nextScene = SecondScene;
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            nextScene = FirstScene;
            SceneManager.LoadScene(nextScene);
        }
        // Play & pause videoplayer, set images.
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            pauseButton.image.sprite = playImage;
        }
        else
        {
            videoPlayer.Play();
            pauseButton.image.sprite = pauseImage;
        }
    }
}