using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

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

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.maxDistance = 5f;
    }

    public void SortirTrou()
    {
        estSortie = true;
        peutFrapper = true;
        if (sonApparition != null)
            audioSource.PlayOneShot(sonApparition);
    }

    void estEntrer()
    {
        peutFrapper = false;
        estSortie = false;
    }

    void estFrapper()
    {
        peutFrapper = false;
        estSortie = false;
        GameManager.Instance.CalculerPoints(10);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!peutFrapper) return;

        // Vérifie le tag sur l'objet ET sur son parent
        bool estMarteau = other.CompareTag("Marteau") ||
                          (other.transform.parent != null && other.transform.parent.CompareTag("Marteau"));

        if (estMarteau)
        {
            XRGrabInteractable grab = other.GetComponentInParent<XRGrabInteractable>();
            if (grab != null && grab.isSelected)
            {
                XRBaseInputInteractor controller = grab.GetOldestInteractorFocusing() as XRBaseInputInteractor;
                if (controller != null)
                    controller.SendHapticImpulse(amplitudeGrab, dureeGrab);
            }
            estFrapper();
        }
    }
}