#nullable enable
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Nuclear.Utilities
{
    public sealed class DropZone : UIBehaviour
    {
        [field: SerializeField] public UnityEvent<DropZone, TemporaryDragItem> OnDraggedItemEnter { get; private set; } = new();
        [field: SerializeField] public UnityEvent<DropZone, TemporaryDragItem> OnDraggedItemExit { get; private set; } = new();
        [field: SerializeField] public UnityEvent<DropZone, TemporaryDragItem> OnDraggedItemDrop { get; private set; } = new();

        public void Enter(TemporaryDragItem temporaryDragItem)
        {
            OnDraggedItemEnter.Invoke(this, temporaryDragItem);
        }

        public void Exit(TemporaryDragItem temporaryDragItem)
        {
            OnDraggedItemExit.Invoke(this, temporaryDragItem);
        }

        public void Drop(TemporaryDragItem temporaryDragItem)
        {
            OnDraggedItemDrop.Invoke(this, temporaryDragItem);
        }
    }
}