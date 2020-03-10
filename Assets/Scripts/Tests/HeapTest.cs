using System;
using NUnit.Framework;
using Pathfinding;
using Unity.Collections;

namespace Tests
{
    /* Copyright (C) Anton Trukhan <anton.truhan@gmail.com>, 2020 - All Rights Reserved.
    */
    struct TestNode : IComparable<TestNode>, IEquatable<TestNode>
    {
        public int Value;
        
        public int CompareTo(TestNode other)
        {
            return Math.Sign(this.Value - other.Value);
        }

        public bool Equals(TestNode other)
        {
            return this.Value == other.Value;
        }
    }
        
    
    [TestFixture]
    public class HeapTest
    {
        [Test]
        public void MinHeapInsertionTest()
        {
            var heap = new PriorityQueue<int>(HeapType.Min, Allocator.Persistent);
            heap.Add(8);
            heap.Add(2);
            heap.Add(5);
            heap.Add(10);
            heap.Add(3);
            
            Assert.AreEqual( 2, heap.Peek() );
            
            heap.Dispose();
        }
        
        [Test]
        public void MinHeapRemovalTest()
        {
            var heap = new PriorityQueue<int>(HeapType.Min, Allocator.Persistent);
            heap.Add(8);
            heap.Add(2);
            heap.Add(5);
            heap.Add(10);
            heap.Add(3);

            heap.Remove();
            heap.Remove();
            
            Assert.AreEqual( 5, heap.Peek() );
            
            heap.Dispose();
        }
        
        [Test]
        public void MinHeapClassRemovalTest()
        {
            var heap = new PriorityQueue<TestNode>( HeapType.Min, Allocator.Persistent );
            heap.Add( new TestNode { Value = 8 } );
            heap.Add( new TestNode { Value = 2 } );
            heap.Add( new TestNode { Value = 5 }  );
            heap.Add( new TestNode { Value = 10 }  );
            heap.Add( new TestNode { Value = 3 }  );

            heap.Remove();
            heap.Remove();
            
            Assert.AreEqual( 5, heap.Peek().Value );
            
            heap.Dispose();
        }

        [Test]
        public void MaxHeapInsertionTest()
        {
            var heap = new PriorityQueue<int>(HeapType.Max, Allocator.Persistent);
            heap.Add(8);
            heap.Add(2);
            heap.Add(5);
            heap.Add(10);
            heap.Add(3);
            
            Assert.AreEqual( 10, heap.Peek() );
            
            heap.Dispose();
        }
        
        [Test]
        public void MaxHeapRemovalTest()
        {
            var heap = new PriorityQueue<int>(HeapType.Max, Allocator.Persistent);
            heap.Add(8);
            heap.Add(2);
            heap.Add(5);
            heap.Add(10);
            heap.Add(3);

            heap.Remove();
            
            Assert.AreEqual( 8, heap.Peek() );
            
            heap.Dispose();
        }
    }
}