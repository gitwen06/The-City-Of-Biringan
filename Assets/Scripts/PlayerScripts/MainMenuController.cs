using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject SaveHUD;
    public void ClickPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenSaveHUD()
    {
        SaveHUD.SetActive(true);
        SavesController.instance.PopulateSlots();
    }
}
