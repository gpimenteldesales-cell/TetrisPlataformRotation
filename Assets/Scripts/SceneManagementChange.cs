using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagementChange : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }
    public void Quit()
    {
        Application.Quit();
    }
}
