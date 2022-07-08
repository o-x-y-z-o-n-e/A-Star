using System;
using System.Collections.Generic;

namespace AStar {

	internal class NodeHeap {

		Node[] data;
		int count;


		//----------------------------------------------------------------------------------------------------------------------------------<


		public NodeHeap(int maxSize) {
			data = new Node[maxSize];
			count = 0;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public int Count => count;
		public Node Peek(int i) => data[i];
		public bool Contains(Node node) => node == data[node.HeapIndex];
		public void Update(Node node) => PercolateUp(node.HeapIndex);
		public void Remove(Node node) => Remove(node.HeapIndex);


		//----------------------------------------------------------------------------------------------------------------------------------<


		public void Add(Node node) {
			data[count] = node;
			node.HeapIndex = count;

			PercolateUp(count);

			count++;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public Node Remove(int i) {
			Node node = data[i];
			node.HeapIndex = 0;

			count--;

			data[i] = data[count];
			data[i].HeapIndex = i;

			data[count] = null;
			
			PercolateDown(i);

			return node;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		private void PercolateUp(int i) {
			int parentIndex = (i - 1) / 2;

			Node parent = data[parentIndex];
			Node node = data[i];

			if(node.Compare(parent) < 0) {
				Swap(i, parentIndex);
				PercolateUp(parentIndex);
			}
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		private void PercolateDown(int i) {
			int leftIndex = 2 * i + 1;
			int rightIndex = 2 * i + 2;

			int swapIndex;

			if(leftIndex < count) {
				swapIndex = leftIndex;

				if(rightIndex < count) {
					if(data[rightIndex].F < data[leftIndex].F)
						swapIndex = rightIndex;
				}

				if(data[i].Compare(data[swapIndex]) > 0) {
					Swap(i, swapIndex);
					PercolateDown(swapIndex);
				}
			}
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		private void Swap(int a, int b) {
			Node na = data[a];
			Node nb = data[b];

			data[a] = nb;
			data[b] = na;

			na.HeapIndex = b;
			nb.HeapIndex = a;
		}
	}

}
