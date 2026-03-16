using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class InteractableDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f; 
    public float openSpeed = 120f; 
    public Transform hinge; 

    [Header("Door Sounds")]
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorCloseSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject key;
    [SerializeField] GameObject player;

    private PlayerController controller;
    bool isOpen = false;
    bool rotating = false;
    float rotatedSoFar = 0f; 
    float remainingAngle = 0f;
    int rotationDirection = 1; 

    Vector3 hingePosition;
    Vector3 hingeAxis;

    void Start()
    {
        controller = player.GetComponent<PlayerController>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (hinge != null)
        {
            hingePosition = hinge.position;
            hingeAxis = hinge.up;
        }
        else
        {
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
        if (controller.heldObject == key) {
        if (!rotating)
        {
            
            if (!isOpen)
            {
                rotationDirection = 1;
                remainingAngle = openAngle;
                rotatedSoFar = 0f;
                PlaySound(doorOpenSound);
                Debug.Log(controller.heldObject == key);
                    Destroy(key);
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
    }
        if (rotationDirection == 1)
        {
            rotationDirection = -1;
            remainingAngle = rotatedSoFar;
            PlaySound(doorCloseSound);
        }
        else
        {
            rotationDirection = 1;
            remainingAngle = openAngle - rotatedSoFar;
            PlaySound(doorOpenSound);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}
