using System;
using System.Collections.Generic;
using System.Text;

namespace AStar {

	public class Node {

		internal int _heapIndex = 0;
		internal bool _uniform = false;
		internal Node Parent => parent;


		//----------------------------------------------------------------------------------------------------------------------------------<


		public int X => x;					// Horizontal coordinate on the grid.
		public int Y => y;					// Vertical coordinate on the grid.
		public int G => g;					// Distance away from starting node.
		public int H => h;					// Distance away from ending node.
		public int F => g + h;              // Sum of G & H.
		public int Weight => weight;
		public bool Blocked => blocked;


		//----------------------------------------------------------------------------------------------------------------------------------<


		private int x;
		private int y;
		private int g;
		private int h;
		private int weight;
		private bool blocked;
		private Node parent;


		//----------------------------------------------------------------------------------------------------------------------------------<


		internal Node(int x, int y) {
			this.x = x;
			this.y = y;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		internal void Clear() {
			g = 0;
			h = 0;
			parent = null;
			_heapIndex = 0;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public void SetBlocked(bool blocked) => this.blocked = blocked;


		//----------------------------------------------------------------------------------------------------------------------------------<


		public void SetWeight(int weight) {
			if(_uniform)
				return;

			if(weight < 0)
				weight = 0;

			this.weight = weight;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		internal void SetParent(Node parent) => this.parent = parent;


		//----------------------------------------------------------------------------------------------------------------------------------<


		internal void SetCosts(int g, int h) {
			this.g = g;
			this.h = h;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		internal int Compare(Node node) {
			int c = F.CompareTo(node.F);
			if (c == 0) c = h.CompareTo(node.h);

			return c;
		}

	}

}
