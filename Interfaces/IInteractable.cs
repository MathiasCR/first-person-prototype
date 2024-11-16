public interface IInteractable
{
    bool StartActive { get; }
    bool IsActive { get; }
    bool IsInteractable { get; }
    bool IsInteracting { get; }
    float InteractionTime { get; }
    void Interact();
    void Activate();
    void StopInteracting();
}
