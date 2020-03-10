using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Pathfinding
{
    /* Copyright (C) Anton Trukhan <anton.truhan@gmail.com>, 2020 - All Rights Reserved.
    */
    
    public enum HeapType { Max, Min }
    
    /// <summary>
    /// Binary heap suitable for being an underlying structure for Priority queue
    /// </summary>
    /// <typeparam name="T">Any class/struct implementing IComparable & IEquttable interface
    /// using Structs instead of Classes as it will allow for a better performance due to better
    /// memory layout and hence, better caching
    /// </typeparam>
    [NativeContainer]
    public struct PriorityQueue<T> : IDisposable where T:struct, IComparable<T>, IEquatable<T>
    {
        //Event if T is a struct - copying it can be more expensive than just moving indexes around
        //So it is better to keep two arrays. Index Array and item array
        private NativeList<T> nodes;
        private readonly int heapifyUpValue;
        private readonly int heapifyDownValue;

        #region - Public API
        public PriorityQueue(HeapType heapType, Allocator allocator)
        {
            heapifyUpValue = heapType == HeapType.Max ? 1 : -1;
            heapifyDownValue = heapType == HeapType.Max ? -1 : 1;
            nodes = new NativeList<T>(allocator);
        }

        public PriorityQueue(HeapType heapType, NativeList<T> content, Allocator allocator)
        {
            heapifyUpValue = heapType == HeapType.Max ? 1 : -1;
            heapifyDownValue = heapType == HeapType.Max ? -1 : 1;

            nodes = new NativeList<T>(content.Length, allocator);
            BuildHeap(content);
        }
        
        /// <summary>
        /// Inserts nodes into heap in linear O(n) time
        /// </summary>
        /// <param name="content"></param>
        private void BuildHeap(NativeList<T> content)
        {
            nodes = content;
            //Lowest level has no children so we start from the previous to last level of the heap
            //Partial fill of the last level is taken into accound
            var startNode = nodes.Length / 2 - 1;
            for (var i = startNode; i >= 0; i--)
            {
                HeapifyDown( i );
            }
            
        }

        public T Peek()
        {
            return nodes[0];
        }
                
        public void Add(T item)
        {
            nodes.Add( item );
            Heapify( nodes.Length - 1 );
        }
        
        public T Remove()
        {
            var removedItem = nodes[0];
            nodes.RemoveAtSwapBack(0);
            HeapifyDown(0);
            return removedItem;
        }
        
        public void Remove(T item)
        {
            var index = -1;
            for (var i = 0; i < nodes.Length; ++i)
            {
                if (nodes[i].Equals(item))
                {
                    index = i;
                    break;
                }
            }
            
            if (index == -1)
            {
                throw new Exception($"Item {item} was not present in heap");
            }

            nodes.RemoveAtSwapBack(index);
            HeapifyDown(index);
        }
        
        public int Count => nodes.Length;

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetParentIndex(int index)
        {
            var parent = (int) ( (index - 1) / 2f );
            return parent;
        }
        
        private int Heapify(int index)
        {
            int i = index;

            while ( i > 0)
            {
                var parent = GetParentIndex(i);
                var insertedNode = nodes[i];
                var parentNode = nodes[parent];
                
                if ( insertedNode.CompareTo( parentNode ) == heapifyUpValue )
                {
                    Exchange(nodes, i, parent);
                    index = parent;
                }
                else
                {
                    return index;
                }
                
                i = parent;
            }

            return index;
        }

        private int HeapifyDown(int index)
        {           
            int left = (index << 1) + 1;
            int right = left + 1;

            while ( left  < nodes.Length )
            {
                var currentNode = nodes[index];
                
                //First we check if both children are bigger/smaller than parent
                var leftNodeComparison = currentNode.CompareTo( nodes[left] ) == heapifyDownValue;
                var rightNodeComparison = right < nodes.Length && currentNode.CompareTo( nodes[right] ) == heapifyDownValue;
                
                //If they both don't meet the requirements - we abort the Heap-down
                if ( !leftNodeComparison && !rightNodeComparison )
                {
                    return index;
                }
                
                //If they both meet the requirements, we select the one which is smaller / bigger
                if ( leftNodeComparison == rightNodeComparison )
                {
                    leftNodeComparison = nodes[right].CompareTo( nodes[left] ) == heapifyDownValue;
                }

                var exchangeIndex = leftNodeComparison ? left : right;
                Exchange(nodes, index, exchangeIndex);
                index = exchangeIndex;
                
                left = (index << 1) + 1;
                right = left + 1;
            }

            return index;
        }
        
        private void Exchange( NativeList<T> list, int index1, int index2)
        {
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }


        public void Dispose()
        {
            nodes.Dispose();
        }
    }
}