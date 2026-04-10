using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Script représentant une taupe dans le célébre jeu Whack-A-Mole en VR.
/// Gère son apparition, sa détection de collision avec le marteau.
/// </summary>

[RequireComponent(typeof(AudioSource))]
public class Mole : MonoBehaviour
{
    [Header("Haptique")]
    [SerializeField] private float amplitudeGrab = 0.5f;
    [SerializeField] private float dureeGrab = 0.1f;

    [Header("Audio")]
    [SerializeField] private AudioClip sonApparition;

    private AudioSource audioSource;
    private bool peutFrapper = false;
    public bool estSortie = false;

    /// <summary>
    /// Initialisation du compasant AudioSource avec des paramètres 3D (code inspiré des notes de cours)
    /// </summary>
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.maxDistance = 5f;
    }

    /// <summary>
    /// Fait sortir la taupe du trou.
    /// </summary>
    public void SortirTrou()
    {
        estSortie = true;
        peutFrapper = true; // Active la possibilité de le frapper
        if (sonApparition != null)
            audioSource.PlayOneShot(sonApparition);
    }

    /// <summary>
    /// Fait rentrer la taupe dans le trou.
    /// </summary>
    void estEntrer()
    {
        peutFrapper = false; // Désactive la possibilité de le frapper
        estSortie = false;
    }

    /// <summary>
    /// Appelé lorsque la taupe est frappé.
    /// </summary>
    void estFrapper()
    {
        peutFrapper = false;
        estSortie = false;
        GameManager.Instance.CalculerPoints(10); // Donne 10 points lorsque une taupe est frappée
        Destroy(gameObject); // Détruit la taupe
    }

    /// <summary>
    /// Méthode detectant une collision avec le marteau.
    /// </summary>
    /// <param name="other">Collider de l'objet entrant en collision</param>
    void OnTriggerEnter(Collider other)
    {
        // Si la taupe ne peut pas être frappée, on ignore
        if (!peutFrapper) return;

        // Vérifie si l'objet ou son parent possède le tag "Marteau"
        bool estMarteau = other.CompareTag("Marteau") ||
                          (other.transform.parent != null && other.transform.parent.CompareTag("Marteau"));

        if (estMarteau)
        {
            // Récupère le composant
            XRGrabInteractable grab = other.GetComponentInParent<XRGrabInteractable>();

            // Vérifie que le marteau est bien tenu par le joueur
            if (grab != null && grab.isSelected)
            {
                XRBaseInputInteractor controller = grab.GetOldestInteractorFocusing() as XRBaseInputInteractor;

                // Envoie une vibration haptique
                if (controller != null)
                    controller.SendHapticImpulse(amplitudeGrab * 0.3f, dureeGrab * 0.5f);
            }

            // Appele la méthode estFrapper()
            estFrapper();
        }
    }
}