using UnityEngine;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    public Button boutonModeSecret;

    void Start()
    {
        bool jeuTermine = PlayerPrefs.GetInt("JeuTermine", 0) == 1;

        boutonModeSecret.interactable = jeuTermine;
    }
}