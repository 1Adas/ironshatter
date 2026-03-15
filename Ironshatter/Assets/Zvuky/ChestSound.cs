using UnityEngine;
using System.Collections;

// Tento script řídí celou sekvenci otevření truhly:
// 1. Přehraje animaci odpadnutí zámku
// 2. Počká, až animace doběhne
// 3. Přehraje zvuk a animaci otevření víka
// Přidej ho na hlavní GameObject truhly.
public class ChestSound : MonoBehaviour
{
    [Header("Sounds")]
    // Zvuk odpadnutí zámku – přetáhni v Inspectoru
    [SerializeField] private AudioClip lockFallSound;
    // Zvuk otevření víka truhly – přetáhni v Inspectoru
    [SerializeField] private AudioClip chestOpenSound;
    // AudioSource komponenta – pokud je prázdná, najde se automaticky
    [SerializeField] private AudioSource audioSource;

    [Header("Animation")]
    // Animator na tomto GameObjectu (nebo přetáhni konkrétní)
    [SerializeField] private Animator animator;
    // Název triggeru pro animaci odpadnutí zámku v Animatoru
    [SerializeField] private string lockAnimTrigger = "LockFall";
    // Název triggeru pro animaci otevření víka v Animatoru
    [SerializeField] private string openAnimTrigger = "Open";
    // Jak dlouho trvá animace zámku, než se spustí otevření (v sekundách)
    [SerializeField] private float lockAnimDuration = 1.0f;

    // Zabraňuje opakovanému spuštění
    private bool isOpen = false;

    private void Start()
    {
        // Pokud AudioSource nebyla přiřazena v Inspectoru, najdeme ji automaticky
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Pokud Animator nebyl přiřazen v Inspectoru, najdeme ho automaticky
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // Připoj tuto metodu na Event Trigger nebo zavolej z jiného scriptu
    public void OpenChest()
    {
        if (isOpen) return;
        isOpen = true;

        StartCoroutine(OpenSequence());
    }

    // Sekvence: zámek odpadne → počkáme → víko se otevře
    private IEnumerator OpenSequence()
    {
        // Krok 1: Přehrajeme zvuk a animaci odpadnutí zámku
        if (audioSource != null && lockFallSound != null)
            audioSource.PlayOneShot(lockFallSound);

        if (animator != null)
            animator.SetTrigger(lockAnimTrigger);

        // Krok 2: Počkáme, než animace zámku doběhne
        yield return new WaitForSeconds(lockAnimDuration);

        // Krok 3: Přehrajeme zvuk a animaci otevření víka
        if (audioSource != null && chestOpenSound != null)
            audioSource.PlayOneShot(chestOpenSound);

        if (animator != null)
            animator.SetTrigger(openAnimTrigger);
    }

    // Resetuje truhlu do zavřeného stavu (např. při resetu levelu)
    public void ResetChest()
    {
        isOpen = false;
    }

    // Automaticky otevře truhlu při kliknutí myší (vyžaduje Collider na GameObjectu)
    private void OnMouseDown()
    {
        OpenChest();
    }
}

