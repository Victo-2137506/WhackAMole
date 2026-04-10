using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(AudioSource))]

// Le code est inspiré des notes de cours : https://envimmersif-cegepvicto.github.io/exercice_feedback_vr/
public class FeedbackGrab : MonoBehaviour
{
    [Header("Haptique")]
    [SerializeField] private float amplitudeGrab = 0.5f;
    [SerializeField] private float dureeGrab = 0.3f;

    [Header("Audio")]
    [SerializeField] private AudioClip sonGrab;

    private XRGrabInteractable grabInteractable;
    private AudioSource audioSource;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        audioSource = GetComponent<AudioSource>();

        // Configurer l'AudioSource pour du son positionnel
        audioSource.spatialBlend = 1f; // 100% 3D
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.maxDistance = 5f;
    }

    void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrabEntered);
        grabInteractable.selectExited.AddListener(OnGrabExited);
    }

    void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabEntered);
        grabInteractable.selectExited.RemoveListener(OnGrabExited);
    }

    private void OnGrabEntered(SelectEnterEventArgs args)
    {
        // Récupérer le contrôleur depuis l'interactor
        var controller = args.interactorObject.transform.GetComponent<XRBaseInputInteractor>();

        controller.SendHapticImpulse(amplitudeGrab, dureeGrab);

        // Jouer le son à la position de l'objet
        audioSource.PlayOneShot(sonGrab);
    }

    private void OnGrabExited(SelectExitEventArgs args)
    {
        // Vibration plus courte et moins forte au relâchement
        var controller = args.interactorObject.transform.GetComponent<XRBaseInputInteractor>();

        controller.SendHapticImpulse(amplitudeGrab * 0.3f, dureeGrab * 0.5f);
    }
}