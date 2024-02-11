#nullable enable
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Nuclear.Utilities
{
    public sealed class DropZone : UIBehaviour
    {
        [field: SerializeField] public UnityEvent<DropZone, DraggedItem> OnDraggedItemEnter { get; private set; } = new();
        [field: SerializeField] public UnityEvent<DropZone, DraggedItem> OnDraggedItemExit { get; private set; } = new();
        [field: SerializeField] public UnityEvent<DropZone, DraggedItem> OnDraggedItemDrop { get; private set; } = new();

        public void Enter(DraggedItem draggedItem)
        {
            OnDraggedItemEnter.Invoke(this, draggedItem);
        }

        public void Exit(DraggedItem draggedItem)
        {
            OnDraggedItemExit.Invoke(this, draggedItem);
        }

        public void Drop(DraggedItem draggedItem)
        {
            OnDraggedItemDrop.Invoke(this, draggedItem);
        }
    }
}