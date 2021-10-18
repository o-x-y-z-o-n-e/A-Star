using System;
using System.Collections.Generic;
using System.Text;

namespace AStar {

	public class Node {

		internal int _heapIndex = 0;


		int x; public int X => x;
		int y; public int Y => y;

		int g; public int G => g; //distance away from starting node
		int h; public int H => h; //distance away from ending node
		public int F => g + h;


		bool blocked = false; public bool Blocked => blocked;
		Node parent; public Node Parent => parent;


		//----------------------------------------------------------------------------------------------------------------------------------<


		public Node(int x, int y) {
			this.x = x;
			this.y = y;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public void Clear() {
			g = 0;
			h = 0;
			parent = null;
			_heapIndex = 0;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public void SetBlocked(bool blocked) => this.blocked = blocked;


		//----------------------------------------------------------------------------------------------------------------------------------<


		public void SetParent(Node parent) => this.parent = parent;


		//----------------------------------------------------------------------------------------------------------------------------------<


		public void SetCosts(int g, int h) {
			this.g = g;
			this.h = h;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public int Compare(Node node) {
			int c = F.CompareTo(node.F);
			if (c == 0) c = h.CompareTo(node.h);

			return -c;
		}

	}

}
