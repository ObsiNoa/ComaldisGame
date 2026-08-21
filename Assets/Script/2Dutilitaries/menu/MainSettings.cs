using UnityEngine;

public class MainSettings : MonoBehaviour
{
    private bool mouseShowed = false;
    [SerializeField] public GameObject videopanel;
    [SerializeField] public GameObject musicpanel;
    [SerializeField] public GameObject settingsCanvasRoot;

    void Start()
    {
        if (!Cursor.visible)
        {
            mouseShowed = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void OpenMusic()
    {
        //gameObject.SetActive(false);
        musicpanel.SetActive(true);
    }

    public void OpenVideo()
    {
        //gameObject.SetActive(false);
        videopanel.SetActive(true);
    }

    public void Retour()
    {
        if (mouseShowed)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        settingsCanvasRoot.SetActive(false);
    }
}