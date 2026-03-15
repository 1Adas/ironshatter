using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f; // degrees to open from closed
    public float openSpeed = 120f; // degrees per second
    public Transform hinge; // optional transform representing hinge position

    [Header("Door Sounds")]
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorCloseSound;
    [SerializeField] private AudioSource audioSource;

    bool isOpen = false;
    bool rotating = false;
    float rotatedSoFar = 0f; // degrees rotated from closed position (0..openAngle)
    float remainingAngle = 0f;
    int rotationDirection = 1; // 1 = opening (positive), -1 = closing

    Vector3 hingePosition;
    Vector3 hingeAxis;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (hinge != null)
        {
            hingePosition = hinge.position;
            hingeAxis = hinge.up;
        }
        else
        {
            // Fallback hinge: approximate one side of the door using local right
            float halfWidth = 0.5f * Mathf.Max(transform.localScale.x, 0.001f);
            hingePosition = transform.position - transform.right * halfWidth;
            hingeAxis = transform.up;
        }
    }

    void Update()
    {
        if (!rotating) return;

        float step = openSpeed * Time.deltaTime;
        float actualStep = Mathf.Min(step, remainingAngle);

        transform.RotateAround(hingePosition, hingeAxis, rotationDirection * actualStep);

        // Update trackers
        rotatedSoFar += rotationDirection * actualStep;
        rotatedSoFar = Mathf.Clamp(rotatedSoFar, 0f, openAngle);

        remainingAngle -= actualStep;
        if (remainingAngle <= 0.0001f)
        {
            rotating = false;
            isOpen = rotationDirection == 1 ? true : false;
        }
    }

    // Toggle door open/close
    public void Interact()
    {
        // If not currently rotating, start opening or closing depending on state
        if (!rotating)
        {
            if (!isOpen)
            {
                rotationDirection = 1;
                remainingAngle = openAngle;
                rotatedSoFar = 0f;
                PlaySound(doorOpenSound);
            }
            else
            {
                rotationDirection = -1;
                remainingAngle = openAngle;
                rotatedSoFar = openAngle;
                PlaySound(doorCloseSound);
            }
            rotating = true;
            return;
        }

        // If rotating, reverse direction and set remaining based on how far we've rotated
        if (rotationDirection == 1)
        {
            // was opening, now close back to 0
            rotationDirection = -1;
            remainingAngle = rotatedSoFar;
            PlaySound(doorCloseSound);
        }
        else
        {
            // was closing, now open to openAngle
            rotationDirection = 1;
            remainingAngle = openAngle - rotatedSoFar;
            PlaySound(doorOpenSound);
        }
    }

    // Convenience methods
    public void Open()
    {
        if (isOpen) return;
        rotationDirection = 1;
        remainingAngle = openAngle - rotatedSoFar;
        rotating = true;
        PlaySound(doorOpenSound);
    }

    public void Close()
    {
        if (!isOpen) return;
        rotationDirection = -1;
        remainingAngle = rotatedSoFar;
        rotating = true;
        PlaySound(doorCloseSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
