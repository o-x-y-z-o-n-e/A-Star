using System;
using System.Collections.Generic;

namespace AStar {

	class Program {
		static void Main(string[] args) {
			Grid grid = new Grid(11, 6);

			Node a = grid.Get(7, 4);
			Node b = grid.Get(4, 1);
		}
	}


	public class Grid {

		int width;
		int height;
		Node[,] grid;


		Node start;
		Node end;


		//


		public Grid(int width, int height) {
			this.width = Math.Abs(width);
			this.height = Math.Abs(height);

			grid = new Node[this.width, this.height];

			for(int x = 0; x < this.width; x++) {
				for (int y = 0; y < this.height; y++) {
					grid[x, y] = new Node(x, y);
				}
			}
		}


		//


		public Node Get(int x, int y) {
			if (x < 0 || x >= width) return null;
			if (y < 0 || y >= height) return null;

			return grid[x, y];
		}


		//


		public List<Node> GetPath(Node start, Node end) {
			Clear();

			this.start = start;
			this.end = end;

			List<Node> open = new List<Node>();
			HashSet<Node> closed = new HashSet<Node>();

			open.Add(start);

			while (open.Count > 0) {
				Node current = GetCheapest(open);

				open.Remove(current);
				closed.Add(current);

				//Found path
				if (current == end) return RetracePath();

				AddOpenNeighbors(current, open, closed);
			}


			//Did not find path
			return null;
		}


		//


		void Clear() {
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					grid[x, y].Clear();
				}
			}

			start = null;
			end = null;
		}


		//


		Node GetCheapest(List<Node> list) {
			Node cheapest = list[0];
			for(int i = 1; i < list.Count; i++) {
				if(list[i].F <= cheapest.F) {
					if (list[i].H < cheapest.H) cheapest = list[i];
				}
			}

			return cheapest;
		}


		//


		void AddOpenNeighbors(Node node, List<Node> open, HashSet<Node> closed) {
			List<Node> near = GetNeighbors(node);

			for(int i = 0; i < near.Count; i++) {
				if (closed.Contains(near[i])) continue;

				//calc new cost
				int distance = node.G + GetDistance(node, near[i]);

				bool isOpen = open.Contains(near[i]);

				if(distance < near[i].G || !isOpen) {
					near[i].SetCosts(distance, GetDistance(near[i], end));
					near[i].SetParent(node);

					if(!isOpen) open.Add(near[i]);
				}
			}
		}


		//


		List<Node> GetNeighbors(Node node) {
			List<Node> nodes = new List<Node>();

			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					if (x == 0 && y == 0) continue;

					if (x >= 0 && x < width && y >= 0 && y < height) {
						Node n = Get(x, y);

						if (!n.Blocked) nodes.Add(n);
					}
				}
			}

			return nodes;
		}


		//


		List<Node> RetracePath() {
			List<Node> path = new List<Node>();
			Node current = end;

			while (current != start) {
				path.Add(current);
				current = current.Parent;
			}

			path.Reverse();

			return path;
		}


		//


		int GetDistance(Node start, Node end) {
			bool step = true;
			int sx = start.X;
			int sy = start.Y;

			int cost = 0;
			while (step) {
				bool xMove = false;
				bool yMove = false;

				if (sx < end.X) {
					xMove = true;
					sx++;
				} else if (sx > end.X) {
					xMove = true;
					sx--;
				}

				if (sy < end.Y) {
					yMove = true;
					sy++;
				} else if (sy > end.Y) {
					yMove = true;
					sy--;
				}

				if (xMove && yMove) {
					cost += 14;
				} else if (xMove || yMove) {
					cost += 10;
				}

				if (sx == end.X && sy == end.Y) step = false;
			}

			return cost;
		}

	}


	//


	public class Node {

		int x; public int X => x;
		int y; public int Y => y;

		int g; public int G => g; //distance away from starting node
		int h; public int H => h; //distance away from ending node
		public int F => g + h;


		bool blocked = false; public bool Blocked => blocked;
		Node parent; public Node Parent => parent;


		//


		public Node(int x, int y) {
			this.x = x;
			this.y = y;
		}


		//


		public void Clear() {
			g = 0;
			h = 0;
			parent = null;
		}


		//


		public void SetBlocked(bool blocked) => this.blocked = blocked;
		public void SetParent(Node parent) => this.parent = parent;
		public void SetCosts(int g, int h) {
			this.g = g;
			this.h = h;
		}

	}

}
