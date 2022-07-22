﻿using Blazor.Diagrams.Core.Models.Base;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Blazor.Diagrams.Core
{
    public abstract class BaseLayer<T> : IReadOnlyList<T> where T : Model
    {
        private readonly List<T> _items = new List<T>();

        public event Action<T>? Added;
        public event Action<T>? Removed;

        public BaseLayer(DiagramBase diagram)
        {
            Diagram = diagram;
        }

        public virtual void Add(T item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            _items.Add(item);
            OnItemAdded(item);
            Added?.Invoke(item);
            Diagram.Refresh();
        }

        public virtual void Add(IEnumerable<T> items)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));

            Diagram.Batch(() =>
            {
                foreach (var item in items)
                {
                    _items.Add(item);
                    OnItemAdded(item);
                    Added?.Invoke(item);
                }
            });
        }

        public virtual void Remove(T item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (_items.Remove(item))
            {
                OnItemRemoved(item);
                Removed?.Invoke(item);
                Diagram.Refresh();
            }
        }

        public virtual void Remove(IEnumerable<T> items)
        {
            if (items is null)
                throw new ArgumentNullException(nameof(items));

            Diagram.Batch(() =>
            {
                foreach (var item in items)
                {
                    if (_items.Remove(item))
                    {
                        OnItemRemoved(item);
                        Removed?.Invoke(item);
                    }
                }
            });
        }

        public bool Contains(T item) => _items.Contains(item);

        public void Clear()
        {
            if (Count == 0)
                return;

            Diagram.Batch(() =>
            {
                for (var i = _items.Count - 1; i >= 0; i--)
                {
                    var item = _items[i];
                    _items.RemoveAt(i);
                    OnItemRemoved(item);
                    Removed?.Invoke(item);
                }
            });
        }

        protected virtual void OnItemAdded(T item) { }

        protected virtual void OnItemRemoved(T item) { }

        public DiagramBase Diagram { get; }

        public int Count => _items.Count;
        public T this[int index] => _items[index];
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}
