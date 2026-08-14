using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalCanvasManager : MonoBehaviour
{
    [SerializeField] private TMP_Text compteurText;

    private int compteur = 0;
    private const int nombreTotal = 8;

    private bool quotaAtteint = false;

    private void Start()
    {
        ActualiserUI();
    }

    private void Update()
    {
        if (compteur >= nombreTotal && !quotaAtteint)
        {
            quotaAtteint = true;
            StartCoroutine(FinDeJournee());
        }
    }

    private IEnumerator FinDeJournee()
    {

        compteurText.text = "Vous avez rangé votre quota en carton";
        yield return new WaitForSeconds(0.5f);

        SceneLoader.SceneToLoad = "EndDayHubCariste";
        SceneManager.LoadScene("LoadingScreen");
    }

    public void AjouterBoite()
    {
        compteur++;
        ActualiserUI();
    }

    public void RetirerBoite()
    {
        compteur = Mathf.Max(0, compteur - 1);
        ActualiserUI();
    }

    private void ActualiserUI()
    {
        compteurText.text = compteur + "/" + nombreTotal;
    }
}