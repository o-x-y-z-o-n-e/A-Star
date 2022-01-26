using System;
using System.Collections.Generic;

namespace AStar {

	internal class Heap {


		Node[] data;
		int count;


		//----------------------------------------------------------------------------------------------------------------------------------<


		public Heap(int maxSize) {
			data = new Node[maxSize];
			count = 0;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public int Count => count;
		public Node Peek(int i) => data[i];
		public bool Contains(Node node) => node == data[node._heapIndex];
		public void Update(Node node) => PercolateUp(node._heapIndex);
		public void Remove(Node node) => Remove(node._heapIndex);


		//----------------------------------------------------------------------------------------------------------------------------------<


		public void Add(Node node) {
			data[count] = node;
			node._heapIndex = count;

			PercolateUp(count);

			count++;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public Node Remove(int i) {
			Node node = data[i];
			node._heapIndex = 0;

			count--;

			data[i] = data[count];
			data[i]._heapIndex = i;

			data[count] = null;
			
			PercolateDown(i);

			return node;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		void PercolateUp(int i) {
			int parentIndex = (i - 1) / 2;

			Node parent = data[parentIndex];
			Node node = data[i];

			if(node.Compare(parent) < 0) {
				Swap(i, parentIndex);
				PercolateUp(parentIndex);
			}
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		void PercolateDown(int i) {
			int leftIndex = 2 * i + 1;
			int rightIndex = 2 * i + 2;

			int swapIndex = 0;

			if (leftIndex < count) {
				swapIndex = leftIndex;

				if(rightIndex < count) {
					if (data[rightIndex].F < data[leftIndex].F) swapIndex = rightIndex;
				}

				if(data[i].Compare(data[swapIndex]) > 0) {
					Swap(i, swapIndex);
					PercolateDown(swapIndex);
				}
			}
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		void Swap(int a, int b) {
			Node na = data[a];
			Node nb = data[b];

			data[a] = nb;
			data[b] = na;

			na._heapIndex = b;
			nb._heapIndex = a;
		}
	}

}
