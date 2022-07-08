using System;
using System.Collections;
using System.Collections.Generic;

namespace AStar {

	public class Grid {


		public float Scale => scale;
		public int Width => width;
		public int Height => height;
		public float OffsetX => offsetX;
		public float OffsetY => offsetY;
		public bool Uniform => uniform;


		//----------------------------------------------------------------------------------------------------------------------------------<


		private float scale = 1F;
		private int width = 1;
		private int height = 1;
		private float offsetX;
		private float offsetY;
		private bool uniform;
		private Node[,] grid;
		private Node start;
		private Node end;
		private bool disposed;


		//----------------------------------------------------------------------------------------------------------------------------------<


		public Grid(int width, int height) => Init(width, height, 0, 0, 1f, false);
		public Grid(int width, int height, float offsetX, float offsetY, float scale) => Init(width, height, offsetX, offsetY, scale, false);
		public Grid(int width, int height, float offsetX, float offsetY, float scale, bool uniform) => Init(width, height, offsetX, offsetY, scale, uniform);


		//----------------------------------------------------------------------------------------------------------------------------------<


		private void Init(int width, int height, float offsetX, float offsetY, float scale, bool uniform) {
			this.width = Math.Abs(width);
			this.height = Math.Abs(height);
			this.offsetX = offsetX;
			this.offsetY = offsetY;
			this.scale = scale;
			this.uniform = false; //this.uniform = uniform;

			grid = new Node[this.width, this.height];

			for(int x = 0; x < this.width; x++) {
				for(int y = 0; y < this.height; y++) {
					grid[x, y] = new Node(x, y);
					grid[x, y]._uniform = uniform;
				}
			}
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public void Dispose() {
			disposed = true;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		private void Clear() {
			for(int x = 0; x < width; x++) {
				for(int y = 0; y < height; y++) {
					grid[x, y].Clear();
				}
			}

			start = null;
			end = null;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public List<Node> GetPath(Node start, Node end) {
			if(disposed)
				return null;

			Clear();

			this.start = start;
			this.end = end;

			NodeHeap open = new NodeHeap(width * height);
			HashSet<Node> closed = new HashSet<Node>();

			open.Add(start);

			while(open.Count > 0) {
				Node current = open.Peek(0);

				open.Remove(current);
				closed.Add(current);

				//Found path
				if(current == end)
					return RetracePath();

				AddOpenNeighbors(current, open, closed);
			}

			//Did not find path
			return null;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		#region Non Uniform Functions


		private void AddOpenNeighbors(Node node, NodeHeap open, HashSet<Node> closed) {
			List<Node> points = null;

			if(uniform)
				points = GetJumpPoints(node);
			else
				points = GetNeighbors(node);

			for(int i = 0; i < points.Count; i++) {
				if(closed.Contains(points[i]))
					continue;

				//calc new cost
				int cost = node.G + GetDistance(node, points[i]) + points[i].Weight;

				bool isOpen = open.Contains(points[i]);

				if(cost < points[i].G || !isOpen) {
					points[i].SetCosts(cost, GetDistance(points[i], end));
					points[i].SetParent(node);

					if(!isOpen)
						open.Add(points[i]);
					else
						open.Update(points[i]);
				}
			}
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public void SmoothWeights(int size) {
			if(disposed)
				return;

			if(uniform)
				return;

			if(size < 1)
				return;

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
				for(int x = 1; x < width; x++) {
					int removeSample = Math.Clamp(x - kernelExtents - 1, 0, width);
					int addSample = Math.Clamp(x + kernelExtents, 0, width - 1);

					horizontal[x, y] = horizontal[x - 1, y] - grid[removeSample, y].Weight + grid[addSample, y].Weight;
				}
			}

			//Vertical pass
			for(int x = 0; x < width; x++) {
				//First x (0 column) in row y
				for(int y = -kernelExtents; y <= kernelExtents; y++) {
					int sample = Math.Clamp(y, 0, kernelExtents);
					vertical[x, 0] += horizontal[x, sample];
				}

				//For all other x columns in row y
				for(int y = 1; y < height; y++) {
					int removeSample = Math.Clamp(y - kernelExtents - 1, 0, height);
					int addSample = Math.Clamp(y + kernelExtents, 0, height - 1);

					vertical[x, y] = vertical[x, y - 1] - horizontal[x, removeSample] + horizontal[x, addSample];


					int weight = RoundToInt((float)vertical[x, y] / (kernelSize * kernelSize));

					grid[x, y].SetWeight(weight);
				}
			}
		}


		#endregion


		//----------------------------------------------------------------------------------------------------------------------------------<


		#region Utils


		private int GetDistance(Node start, Node end) {
			int cost = 0;

			int dx = end.X - start.X;
			int dy = end.Y - start.Y;

			int adx = Math.Abs(dx);
			int ady = Math.Abs(dy);

			if(adx < ady) {
				cost += 14 * adx;

				ady -= adx;
				adx = 0;
			} else {
				cost += 14 * ady;

				adx -= ady;
				ady = 0;
			}

			if(adx > 0)
				cost += 10 * adx;
			else
				cost += 10 * ady;

			return cost;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		private int RoundToInt(float f) {
			int i = (int)f;

			f -= i;

			if(f > 0.5f)
				i++;
			else if(f < -0.5f)
				i--;

			return i;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		private bool OnGrid(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;


		//----------------------------------------------------------------------------------------------------------------------------------<


		public Node WorldToNode(float x, float y) {
			if(disposed)
				return null;

			int xi = RoundToInt((x / scale) - offsetX);
			int yi = RoundToInt((y / scale) - offsetY);

			if (xi < 0 || xi >= width) return null;
			if (yi < 0 || yi >= height) return null;

			return grid[xi, yi];
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		public Node IndexToNode(int x, int y) {
			if(disposed)
				return null;

			if (x < 0 || x >= width) return null;
			if (y < 0 || y >= height) return null;

			return grid[x, y];
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		private List<Node> GetNeighbors(Node node) {
			List<Node> nodes = new List<Node>();

			for(int x = -1; x <= 1; x++) {
				for(int y = -1; y <= 1; y++) {
					if(x == 0 && y == 0)
						continue;

					int gx = node.X + x;
					int gy = node.Y + y;

					if(OnGrid(gx, gy)) {
						Node n = IndexToNode(gx, gy);

						if(!n.Blocked)
							nodes.Add(n);
					}
				}
			}

			return nodes;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		private List<Node> RetracePath() {
			List<Node> path = new List<Node>();
			Node current = end;

			while(current != start) {
				path.Add(current);
				current = current.Parent;
			}

			path.Reverse();

			return path;
		}


		#endregion


		//----------------------------------------------------------------------------------------------------------------------------------<


		#region Raycast Path Pruning


		// TODO
		private List<Node> PrunePath(List<Node> path) {
			if(!uniform)
				return path;

			List<Node> corners = new List<Node>();

			corners.Add(path[0]);

			int dirX = 0;
			int dirY = 0;

			dirX = path[1].X - path[0].X;
			dirY = path[1].Y - path[0].Y;

			int currentCorner = 0;
			int nextCorner = 0;

			for(int i = 1; i < path.Count-1; i++) {
				int dirX2 = path[i + 1].X - path[i].X;
				int dirY2 = path[i + 1].Y - path[i].Y;

				if(dirX2 != dirX || dirY2 != dirY) {
					dirX = dirX2;
					dirY = dirY2;

					if(nextCorner > currentCorner) {
						corners.Add(path[i]);
						currentCorner = i;
						nextCorner = i;

					} else {
						nextCorner = i + 1;
					}
				}


				if(nextCorner > currentCorner+1) {
					bool hit = Collides(path[currentCorner], path[nextCorner]);

					if(hit) {
						corners.Add(path[nextCorner - 1]);

						currentCorner = nextCorner - 1;
						nextCorner = currentCorner;

						dirX = path[currentCorner + 1].X - path[currentCorner].X;
						dirY = path[currentCorner + 1].Y - path[currentCorner].Y;
					}
				}
			}

			corners.Add(path[path.Count - 1]);

			return corners;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		// TODO
		private bool Collides(Node start, Node end) {



			return true;
		}


		#endregion


		//----------------------------------------------------------------------------------------------------------------------------------<


		#region Jump Point Search


		// TODO: Fix/Get Working
		private List<Node> GetJumpPoints(Node current) {
			List<Node> successors = new List<Node>();
			List<Node> neighbors = GetNeighbors(current);

			foreach(Node neighbor in neighbors) {
				int dx = Math.Clamp(neighbor.X - current.X, -1, 1);
				int dy = Math.Clamp(neighbor.Y - current.Y, -1, 1);

				Node jumpPoint = Jump(current.X, current.Y, dx, dy);

				if(jumpPoint != null)
					successors.Add(jumpPoint);
			}

			return successors;
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		// TODO: Fix/Get Working
		private Node Jump(int cx, int cy, int dx, int dy) {
			int nx = cx + dx;
			int ny = cy + dy;

			if(OnGrid(nx, ny))
				return null;

			if(grid[nx, ny].Blocked)
				return null;

			if(nx == end.X && ny == end.Y)
				return end;

			if(nx != 0 && ny != 0) {
				//Diagonal
				if(DiagonalObstacleCheck(nx, ny, dx, dy))
					return grid[nx, ny];

				if(Jump(nx, ny, dx, 0) != null || Jump(nx, ny, 0, dy) != null)
					return grid[nx, ny];
				
			} else {
				if(nx != 0) {
					//Horizontal
					if(HorizontalObstacleCheck(nx, ny, dx))
						return grid[nx, ny];

				} else {
					//Vertical
					if(VerticalObstacleCheck(nx, ny, dy))
						return grid[nx, ny];
				}
			}

			return Jump(nx, ny, dx, dy);
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		private bool HorizontalObstacleCheck(int x, int y, int dx) {
			return
				(grid[x, y - 1].Blocked && !grid[x + dx, y - 1].Blocked) ||
				(grid[x, y + 1].Blocked && !grid[x + dx, y + 1].Blocked);
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		private bool VerticalObstacleCheck(int x, int y, int dy) {
			return
				(grid[x + 1, y].Blocked && !grid[x + 1, y + dy].Blocked) ||
				(grid[x - 1, y].Blocked && !grid[x - 1, y + dy].Blocked);
		}


		//----------------------------------------------------------------------------------------------------------------------------------<


		private bool DiagonalObstacleCheck(int x, int y, int dx, int dy) {
			return
				(grid[x - dx, y].Blocked && !grid[x - dx, y + dy].Blocked) ||
				(grid[x, y - dy].Blocked && !grid[x + dx, y - dy].Blocked);
		}


		#endregion
	}
}
