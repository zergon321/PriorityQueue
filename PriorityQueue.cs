using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PriorityQueueImplementation
{
    /// <summary>
    /// Represents a FIFO-collection of items ordered by priority.
    /// </summary>
    /// <typeparam name="T">Type of item saved in priority queue.</typeparam>
    public class PriorityQueue<T> : IEnumerable<T>, IEnumerable
    {
        private SortedDictionary<int, Queue<T>> innerQueues;
        private int highestPriority;

        /// <summary>
        /// Indicates whether the queue is empty or not.
        /// </summary>
        public bool IsEmpty => innerQueues.Count == 0;
        /// <summary>
        /// Number of items in whole priority queue.
        /// </summary>
        public int Count => this.Count();
        /// <summary>
        /// The highest priority whose items are enqueued.
        /// </summary>
        public int HighestPriority
        {
            get
            {
                if (IsEmpty)
                    throw new InvalidOperationException("Priority queue doesn't contain items");

                return highestPriority;
            }
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue<T> class. 
        /// </summary>
        public PriorityQueue()
        {
            innerQueues = new SortedDictionary<int, Queue<T>>();
            highestPriority = -1;
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue<T> class with specified priority comparer.
        /// </summary>
        /// <param name="priorityComparer">Comparer for sorting items by their priorities.</param>
        public PriorityQueue(IComparer<int> priorityComparer)
        {
            innerQueues = new SortedDictionary<int, Queue<T>>(priorityComparer);
            highestPriority = -1;
        }

        private Queue<T> getHighestPriorityQueue() => innerQueues[highestPriority];

        /// <summary>
        /// Returns a value indicating whether the queue contains items of the given priority.
        /// </summary>
        /// <param name="priority">The priority to locate in the queue.</param>
        /// <returns>true if queue contains at least one item of specified priority, otherwise false.</returns>
        public bool ContainsPriority(int priority) => innerQueues.ContainsKey(priority);

        /// <summary>
        /// Returns a value indicating whether the queue contains items of the given priority.
        /// </summary>
        /// <param name="value">The item to locate in the queue.</param>
        /// <returns>true if the queue contains the item, otherwise false.</returns>
        public bool ContainsItem(T value) => this.Contains(value);

        /// <summary>
        /// Adds an item to the priority queue by specified priority.
        /// </summary>
        /// <param name="priority">The priority for item.</param>
        /// <param name="item">The item to add to the queue.</param>
        public void Enqueue(int priority, T item)
        {
            // Specification of negative values is forbidden.
            if (priority < 0)
                throw new ArgumentException("Priority value must be more or equal to zero.");

            // If an inner queue for the priority doesn't exist yet.
            if (!ContainsPriority(priority))
            {
                Queue<T> newQueue = new Queue<T>();

                newQueue.Enqueue(item);
                innerQueues.Add(priority, newQueue);
            }
            // If the inner queue for the priority already exists.
            else
            {
                Queue<T> existingQueue = innerQueues[priority];

                existingQueue.Enqueue(item);
            }

            // It's necessary to reassign the highest priority.
            if (innerQueues.Count == 1 || priority < highestPriority)
                highestPriority = priority;
        }

        /// <summary>
        /// Removes the first item with the highest priority.
        /// </summary>
        /// <returns>A value of the first item of the priority queue.</returns>
        public T Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Priority queue doesn't contain items.");

            Queue<T> queueForRemovingItem = getHighestPriorityQueue();
            T elem = queueForRemovingItem.Dequeue();

            // If there are no more items of the highest priority, its inner queue should be removed and the highest priority should be reassigned.
            if (queueForRemovingItem.Count == 0)
            {
                innerQueues.Remove(highestPriority);

                // If the queue is empty now, the highest priority should be assigned with -1.
                if (IsEmpty)
                    highestPriority = -1;
                // Otherwise it should be assigned with the highest priority among the remaining.
                else
                    highestPriority = innerQueues.First().Key;
            }

            return elem;
        }

        /// <summary>
        /// Returns the first item of the priority queue without removing it from the queue.
        /// </summary>
        /// <returns>A value of the first item in the priority queue.</returns>
        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Priority queue doesn't contain items");

            Queue<T> queueForPeekingItem = getHighestPriorityQueue();
            T elem = queueForPeekingItem.Peek();

            return elem;
        }

        /// <summary>
        /// Removes all items from the priority queue.
        /// </summary>
        public void Clear()
        {
            innerQueues.Clear();
            highestPriority = -1;
        }

        public IEnumerator<T> GetEnumerator() => innerQueues.SelectMany(priorityGroup => priorityGroup.Value).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}