using UnityEngine;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    [SerializeField] private string EndOfGame = "KonecneMenu";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("End"))
        {
            Debug.Log("endos");
            SceneManager.LoadScene(EndOfGame);
        }
    }
}