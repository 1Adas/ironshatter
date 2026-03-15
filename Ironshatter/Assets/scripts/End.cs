using UnityEngine;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    [SerializeField] private string EndOfGame = "KonecneMenu";

    // Používáme OnTriggerEnter, protože kostka je nastavená jako Trigger
    private void OnTriggerEnter(Collider other)
    {
        // Kontrola, zda hráè protnul objekt s tagem Finish
        if (other.CompareTag("End"))
        {
            Debug.Log("endos");
            SceneManager.LoadScene(EndOfGame);
        }
    }
}