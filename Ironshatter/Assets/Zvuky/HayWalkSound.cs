using UnityEngine;

public class HayWalkSound : MonoBehaviour
{
    [Header("Hay Walk Sound")]
    [SerializeField] private AudioClip haySound;
    [SerializeField] private AudioSource audioSource;

    private bool playerInside = false;
    private Transform playerTransform;
    private Vector3 lastPlayerPosition;
    private bool isPlaying = false;

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

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

        float movedDistance = Vector3.Distance(playerTransform.position, lastPlayerPosition);
        bool isMoving = movedDistance > 0.001f;
        lastPlayerPosition = playerTransform.position;

        if (isMoving)
            StartHaySound();
        else
            StopHaySound();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerTransform = other.transform;
            lastPlayerPosition = other.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            playerTransform = null;
            StopHaySound();
        }
    }

    private void StartHaySound()
    {
        if (isPlaying || audioSource == null || haySound == null) return;
        isPlaying = true;
        audioSource.Play();
    }

    private void StopHaySound()
    {
        if (!isPlaying || audioSource == null) return;
        isPlaying = false;
        audioSource.Stop();
    }
}