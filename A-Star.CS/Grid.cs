using System;
using System.Collections;
using System.Collections.Generic;

namespace AStar {

	public class Grid {

		
		float scale = 1f; public float Scale => scale;
		int width = 1; public int Width => width;
		int height = 1; public int Height => height;
		float offsetX = 0; public float OffsetX => offsetX;
		float offsetY = 0; public float OffsetY => offsetY;

		Node[,] grid = null;


		Node start = null;
		Node end = null;


		//----------------------------------------------------------------------------------------------------------------------------------<


		public Grid(int width, int height) => Init(width, height, 0, 0, 1f);
		public Grid(int width, int height, float offsetX, float offsetY, float scale) => Init(width, height, offsetX, offsetY, scale);


		//----------------------------------------------------------------------------------------------------------------------------------<


		void Init(int width, int height, float offsetX, float offsetY, float scale) {
			this.width = Math.Abs(width);
			this.height = Math.Abs(height);
			this.offsetX = offsetX;
			this.offsetY = offsetY;
			this.scale = scale;

			grid = new Node[this.width, this.height];

			for (int x = 0; x < this.width; x++) {
				for (int y = 0; y < this.height; y++) {
					grid[x, y] = new Node(x, y);
				}
			}
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public Node WorldToNode(float x, float y) {
			int xi = RoundToInt((x / scale) - offsetX);
			int yi = RoundToInt((y / scale) - offsetY);

			if (xi < 0 || xi >= width) return null;
			if (yi < 0 || yi >= height) return null;

			return grid[xi, yi];
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public Node IndexToNode(int x, int y) {
			if (x < 0 || x >= width) return null;
			if (y < 0 || y >= height) return null;

			return grid[x, y];
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public List<Node> GetPath(Node start, Node end) {
			Clear();

			this.start = start;
			this.end = end;

			Heap open = new Heap(width * height);
			HashSet<Node> closed = new HashSet<Node>();

			open.Add(start);

			while (open.Count > 0) {
				Node current = open.Peek(0);

				open.Remove(current);
				closed.Add(current);

				//Found path
				if (current == end) return RetracePath();

				AddOpenNeighbors(current, open, closed);
			}


			//Did not find path
			return null;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		void Clear() {
			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					grid[x, y].Clear();
				}
			}

			start = null;
			end = null;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		void AddOpenNeighbors(Node node, Heap open, HashSet<Node> closed) {
			List<Node> near = GetNeighbors(node);

			for(int i = 0; i < near.Count; i++) {
				if (closed.Contains(near[i])) continue;

				//calc new cost
				int cost = node.G + GetDistance(node, near[i]) + near[i].Weight;

				bool isOpen = open.Contains(near[i]);

				if(cost < near[i].G || !isOpen) {
					near[i].SetCosts(cost, GetDistance(near[i], end));
					near[i].SetParent(node);

					if (!isOpen) open.Add(near[i]);
					else open.Update(near[i]);
				}
			}
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		List<Node> GetNeighbors(Node node) {
			List<Node> nodes = new List<Node>();

			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					if (x == 0 && y == 0) continue;

					int gx = node.X + x;
					int gy = node.Y + y;

					

					if (gx >= 0 && gx < width && gy >= 0 && gy < height) {
						Node n = IndexToNode(gx, gy);

						if (!n.Blocked) nodes.Add(n);
					}
				}
			}

			return nodes;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


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


		//----------------------------------------------------------------------------------------------------------------------------------<


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


		//----------------------------------------------------------------------------------------------------------------------------------<


		int RoundToInt(float f) {
			int i = (int)f;

			f -= i;

			if (f > 0.5f) i++;
			else if (f < -0.5f) i--;

			return i;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public void SmoothWeights(int size) {
			if (size < 1) return;

			int kernelSize = 1 + size * 2;
			int kernelExtents = (kernelSize - 1) / 2;

			int[,] horizontal = new int[width, height];
			int[,] vertical = new int[width, height];

			//Horizontal pass
			for(int y = 0; y < height; y++) {
				//First x (0 column) in row y
				for(int x = -kernelExtents; x <= kernelExtents; x++) {
					int sample = Math.Clamp(x, 0, kernelExtents);
					horizontal[0, y] += grid[sample, y].Weight;
				}

				//For all other x columns in row y
				for (int x = 1; x < width; x++) {
					int removeSample = Math.Clamp(x - kernelExtents - 1, 0, width);
					int addSample = Math.Clamp(x + kernelExtents, 0, width - 1);

					horizontal[x, y] = horizontal[x - 1, y] - grid[removeSample, y].Weight + grid[addSample, y].Weight;
				}
			}

			//Vertical pass
			for (int x = 0; x < width; x++) {
				//First x (0 column) in row y
				for (int y = -kernelExtents; y <= kernelExtents; y++) {
					int sample = Math.Clamp(y, 0, kernelExtents);
					vertical[x, 0] += horizontal[x, sample];
				}

				//For all other x columns in row y
				for (int y = 1; y < height; y++) {
					int removeSample = Math.Clamp(y - kernelExtents - 1, 0, height);
					int addSample = Math.Clamp(y + kernelExtents, 0, height - 1);

					vertical[x, y] = vertical[x, y - 1] - horizontal[x, removeSample] + horizontal[x, addSample];


					int weight = RoundToInt((float)vertical[x, y] / (kernelSize * kernelSize));

					grid[x, y].SetWeight(weight);
				}
			}
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		List<Node> PrunePath(List<Node> path) {
			List<Node> corners = new List<Node>();

			corners.Add(path[0]);

			int dirX = 0;
			int dirY = 0;

			dirX = path[1].X - path[0].X;
			dirY = path[1].Y - path[0].Y;

			int currentCorner = 0;
			int nextCorner = 0;

			for (int i = 1; i < path.Count-1; i++) {
				int dirX2 = path[i + 1].X - path[i].X;
				int dirY2 = path[i + 1].Y - path[i].Y;

				if (dirX2 != dirX || dirY2 != dirY) {
					dirX = dirX2;
					dirY = dirY2;

					if (nextCorner != currentCorner) {
						corners.Add(path[i]);
						currentCorner = i;
						nextCorner = i;

					} else {
						nextCorner = i + 1;

						//raycast from current corner to next corner
						//if collides, add previous raycast check as corner
						//else continue to next node in path
					}

					
				}
			}

			//DOUBLE CHECK THAT START & END NODES ARE INCLUDED IN PATH INSIDE FindPath()

			corners.Add(path[path.Count - 1]);

			return corners;
		}
	}
}
