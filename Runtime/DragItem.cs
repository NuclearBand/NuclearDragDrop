#nullable enable
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Nuclear.Utilities
{
    public sealed class DragItem : UIBehaviour, IBeginDragHandler, IDragHandler, 
        IPointerDownHandler, IPointerUpHandler
    {
        public float DragTimeThreshold = 0.1f;
        [field: SerializeField] public UnityEvent<DragItem, DraggedItem> OnBeginDragEvent { get; private set; } = new();
        [field: SerializeField] public UnityEvent<DragItem, DropZone?> OnEndDragEvent { get; private set; }  = new();

        public RectTransform? DraggedItemParent;

        private static DraggedItem? _draggedItem;
        private float? _pressedTime;

        public void OnBeginDrag(PointerEventData eventData)
        {
            // уже драгаем другой айтем
            if (_draggedItem != null)
            {
                return;
            }
            
            // сдвинули палец раньше времени
            if (!(_pressedTime != null && Time.time - _pressedTime.Value >= DragTimeThreshold))
            {
                _pressedTime = null;
                return;
            }
            
            CreateDraggedItem(eventData);
        }

        private void CreateDraggedItem(PointerEventData eventData)
        {
            _draggedItem = Instantiate(gameObject, DraggedItemParent, true).AddComponent<DraggedItem>();
            
            var thisTransform = transform;
            var draggedItemTransform = _draggedItem.transform;
            var startPosition = thisTransform.position;
            draggedItemTransform.SetPositionAndRotation(startPosition, thisTransform.rotation);
            draggedItemTransform.localScale = thisTransform.localScale;
            
            Destroy(_draggedItem.gameObject.GetComponent<DragItem>());
            eventData.pointerDrag = _draggedItem.gameObject;
            _draggedItem.BeginDrag(this, eventData.position, OnEndDrag);
            
            OnBeginDragEvent.Invoke(this, _draggedItem);
        }

        private void OnEndDrag(PointerEventData eventData)
        {
            Destroy(_draggedItem!.gameObject);
            _draggedItem = null;

            DropZone? dropZoneComponent = null;
            var dropZone = eventData.pointerCurrentRaycast.gameObject;
            if (dropZone != null)
            {
                dropZoneComponent = dropZone.GetComponent<DropZone>();
            }

            OnEndDragEvent.Invoke(this, dropZoneComponent);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pressedTime = Time.time;
            var scrollRect = GetComponentInParent<ScrollRect>();
            if (scrollRect != null)
                scrollRect.StopMovement();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pressedTime = null;
        }

        // нельзя удалять, иначе OnBeginDrag не срабатывает
        public void OnDrag(PointerEventData eventData){ }
    }
}