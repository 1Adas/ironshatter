using UnityEngine;

public class ChestUse : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject key;        
    [SerializeField] Animator animator;

    PlayerController controller;

    [Header("Chest State")]
    [SerializeField] bool locked = true;
    bool opened = false;

    [Header("Sound")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip openSound;

    void Start()
    {
        controller = player.GetComponent<PlayerController>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (opened) return;

        if (locked)
        {
            if (controller.heldObject == key)
            {
                UnlockAndOpen();
            }
            else
            {
                Debug.Log("Chest is locked.");
            }
            return;
        }

        OpenChest();
    }

    void UnlockAndOpen()
    {
        Debug.Log("ano");
        locked = false;

        Destroy(key);

        OpenChest();
    }

    void OpenChest()
    {
        opened = true;

        if (animator)
            animator.SetTrigger("Open");

        if (audioSource && openSound)
            audioSource.PlayOneShot(openSound);

        Debug.Log("Chest opened!");
    }
}
