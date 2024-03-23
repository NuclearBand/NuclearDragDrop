#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Nuclear.Utilities
{
    public sealed class TemporaryDragItem : UIBehaviour, IDragHandler, IEndDragHandler
    {
        public DragItem DragItem { get; private set; } = null!;
        
        private Vector2 _dragBeginOffset;
        private Action<PointerEventData> _onEndDrag = null!;
        private List<DropZone> _dropZones = null!;
        
        private DropZone? _currentDropZone;

        public void BeginDrag(DragItem dragItem, Vector2 touchPosition, Action<PointerEventData> onEndDrag)
        {
            DragItem = dragItem;
            _onEndDrag = onEndDrag;
            var startPosition = transform.position;
            _dragBeginOffset = touchPosition - new Vector2(startPosition.x, startPosition.y);
            _dropZones = FindObjectsByType<DropZone>(FindObjectsSortMode.None).ToList();
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position - _dragBeginOffset;
            var currentDropZone = GetCurrentDropZone(eventData);
            if (_currentDropZone == currentDropZone) 
                return;
            
            if (_currentDropZone != null)
                _currentDropZone.Exit(this);

            _currentDropZone = currentDropZone;
            if (_currentDropZone != null)
                _currentDropZone.Enter(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_currentDropZone != null)
                _currentDropZone.Drop(this);
            _onEndDrag(eventData);
        }

        private DropZone? GetCurrentDropZone(PointerEventData eventData)
        {
            var currentDropZones = _dropZones.Where(d =>
                    RectTransformUtility.RectangleContainsScreenPoint((RectTransform)d.transform, eventData.position))
                .ToList();
                    
            currentDropZones.Sort((d1, d2) => CompareTransformsByHierarchy(d1.transform, d2.transform));
            return currentDropZones.FirstOrDefault();
        }
        
        private static int CompareTransformsByHierarchy(Transform transform1, Transform transform2)
        {
            if (transform1.IsChildOf(transform2))
            {
                return -1;
            }

            if (transform2.IsChildOf(transform1))
            {
                return 1;
            }
            
            var t1 = transform1;
            var t2 = transform2;
            var parent1 = t1.parent;
            var parent2 = t2.parent;

            while (parent2 != null)
            {
                while (parent1 != null)
                {
                    if (parent1 == parent2)
                    {
                        return t1.GetSiblingIndex() > t2.GetSiblingIndex() ? -1 : 1;
                    }

                    t1 = parent1;
                    parent1 = t1.parent;
                    
                }

                t2 = parent2;
                parent2 = t2.parent;
                t1 = transform1;
                parent1 = t1.parent;
            }

            return 0;
        }
    }
}