using UnityEngine;
using UnityEngine.Events;

public class TriggerInvoke : MonoBehaviour
{
    [SerializeField, TagSelector] private string tagToInteractWithTrigger;
    [SerializeField] private bool destroyAfterOnce;

    private enum TriggerInteractionType
    {
        TriggerEnter,
        TriggerStay,
        TriggerExit
    }

    [SerializeField] private TriggerInteractionType interactionType;

    public UnityEvent OnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (interactionType != TriggerInteractionType.TriggerEnter) return;
        OnTrigger?.Invoke();
        if (destroyAfterOnce) Destroy(this);
    }

    private void OnTriggerStay(Collider other)
    {
        if (interactionType != TriggerInteractionType.TriggerStay) return;
        OnTrigger?.Invoke();
        if (destroyAfterOnce) Destroy(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (interactionType != TriggerInteractionType.TriggerExit) return;
        OnTrigger?.Invoke();
        if (destroyAfterOnce) Destroy(this);
    }
}
