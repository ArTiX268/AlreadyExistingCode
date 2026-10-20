namespace ArTiX.Interaction
{
    public interface IInteractable
    {
        public void Interact();
        public void EnterCanInteract();
        public void ExitCanInteract();
    }
}