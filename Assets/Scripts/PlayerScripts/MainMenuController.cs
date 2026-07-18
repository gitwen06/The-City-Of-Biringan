using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void ClickPlay()
    {
        SceneManager.LoadScene(1);
    }
}
