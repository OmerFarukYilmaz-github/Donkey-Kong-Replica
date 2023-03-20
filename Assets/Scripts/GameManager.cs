using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int level;
    private int lives;
    private int score;

    private void Start()
    {
        level = SceneManager.GetActiveScene().buildIndex;
    }

    private void LoadLevel(int index)
    {
        Camera camera = Camera.main;

        // Don't render anything while loading the next scene to create
        // a simple scene transition effect
        if (camera != null) 
        {
            camera.cullingMask = 0;
        }

        Invoke(nameof(LoadScene), 1f);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(level);
    }

    public void LevelComplete()
    {
        Debug.Log("win");
        score += 1000;

        int nextLevel = (level % SceneManager.sceneCountInBuildSettings)  + 1;

    }

    public void LevelFailed()
    {
        lives--;

        if (lives <= 0) 
        {
            LoadScene();
        } 
        else
        {
            LoadLevel(level);
        }
    }

}
