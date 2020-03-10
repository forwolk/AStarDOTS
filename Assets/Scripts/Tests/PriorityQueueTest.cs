using System;
using NUnit.Framework;
using Pathfinding;
using Unity.Collections;

namespace Tests
{   
    /* Copyright (C) Anton Trukhan <anton.truhan@gmail.com>, 2020 - All Rights Reserved.
    */
    struct TestPriorityQueueNode : IComparable<TestPriorityQueueNode>, IEquatable<TestPriorityQueueNode>
    {
        public int Value;
        
        public int CompareTo(TestPriorityQueueNode other)
        {
            return Math.Sign(this.Value - other.Value);
        }

        public bool Equals(TestPriorityQueueNode other)
        {
            return this.Value == other.Value;
        }

        public int InternalIndex { get; set; }
    }
    
    [TestFixture]
    public class PriorityQueueTest
    {
        [Test]
        public void PriorityQueueUpdateTest()
        {
            var heap = new PriorityQueue<TestPriorityQueueNode>( HeapType.Min, Allocator.Persistent );
            var minItem = new TestPriorityQueueNode { Value = 2 };
            heap.Add( new TestPriorityQueueNode { Value = 8 } );
            heap.Add( minItem );
            heap.Add( new TestPriorityQueueNode { Value = 5 }  );
            heap.Add( new TestPriorityQueueNode { Value = 10 }  );
            heap.Add( new TestPriorityQueueNode { Value = 3 }  );
            
            heap.Remove(minItem);

            Assert.AreEqual( 3, heap.Peek().Value );
            
            heap.Dispose();
        }
        
        [Test]
        public void PriorityQueueUpdateTest2()
        {
            var heap = new PriorityQueue<TestPriorityQueueNode>( HeapType.Max, Allocator.Persistent );
            var maxItem = new TestPriorityQueueNode { Value = 10 };
            heap.Add( new TestPriorityQueueNode { Value = 8 } );
            heap.Add( new TestPriorityQueueNode { Value = 2 } );
            heap.Add( new TestPriorityQueueNode { Value = 5 }  );
            heap.Add( maxItem );
            heap.Add( new TestPriorityQueueNode { Value = 3 }  );

            heap.Remove(maxItem);

            Assert.AreEqual( 8, heap.Peek().Value );
            
            heap.Dispose();
        }
    }
}