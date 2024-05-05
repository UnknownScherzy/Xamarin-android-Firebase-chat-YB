namespace App26.Entity
{
    public interface ISelectable
    {
        void OnSelectionStarted();
        void Select(EntityPreview entityPreview);
        void OnSelectionFinished();
    }
}