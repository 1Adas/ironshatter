using UnityEngine;

// Tento script prehrava zvuk kroku, kdyz hrac chodi pres seno.
// Pridej ho na GameObject sena, ktery ma Collider s Is Trigger = true.
// Na AudioSource nastav Loop = true.
public class HayWalkSound : MonoBehaviour
{
    [Header("Hay Walk Sound")]
    // Zvukovy soubor se zvukem chuze v senu - pretahni v Inspektoru
    [SerializeField] private AudioClip haySound;
    // AudioSource komponenta - pokud je prazdna, najde se automaticky
    [SerializeField] private AudioSource audioSource;

    // True pokud se hrac prave nachazi uvnitr triggeru sena
    private bool playerInside = false;
    // Transform hrace a jeho pozice minuly frame - slouzi k detekci pohybu
    private Transform playerTransform;
    private Vector3 lastPlayerPosition;
    // Sleduje jestli zvuk prave hraje, aby se zbytecne nevolalo Play/Stop kazdy frame
    private bool isPlaying = false;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Nastavime clip a zajistime loop - zvuk bude bezet dokud se hrac pohybuje
        if (audioSource != null && haySound != null)
        {
            audioSource.clip = haySound;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (!playerInside || playerTransform == null)
        {
            StopHaySound();
            return;
        }

        // Zjistime pohyb porovnanim aktualni a minule pozice hrace
        float movedDistance = Vector3.Distance(playerTransform.position, lastPlayerPosition);
        bool isMoving = movedDistance > 0.001f;
        lastPlayerPosition = playerTransform.position;

        if (isMoving)
            StartHaySound();
        else
            StopHaySound();
    }

    // Zavola se, kdyz nejaky objekt vstoupi do triggeru
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerTransform = other.transform;
            lastPlayerPosition = other.transform.position;
        }
    }

    // Zavola se, kdyz objekt opusti trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            playerTransform = null;
            StopHaySound();
        }
    }

    // Spusti zvuk - jen pokud jeste nehraje
    private void StartHaySound()
    {
        if (isPlaying || audioSource == null || haySound == null) return;
        isPlaying = true;
        audioSource.Play();
    }

    // Zastavi zvuk - jen pokud hraje
    private void StopHaySound()
    {
        if (!isPlaying || audioSource == null) return;
        isPlaying = false;
        audioSource.Stop();
    }
}