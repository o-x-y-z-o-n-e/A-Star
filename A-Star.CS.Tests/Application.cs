using System;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;


namespace AStar.CS.Tests {

	class Application {

		static void Main(string[] args) {
			if(args.Length < 1) {
				Console.WriteLine("No input file passed!");
				Console.WriteLine("Pass \"-help\" for a list of commands.");
				return;
			}

			if(args[0] == "-help" || args[0] == "-h") {
				Console.WriteLine("Help Section:");
				return;
			}

			string inputFile = args[0];
			string outputFile = "";

			if (args.Length > 1) outputFile = args[1];
			else outputFile = "out.bmp";

			if(!File.Exists(inputFile)) {
				Console.WriteLine("Input file does not exist!");
				return;
			}

			try {
				Image<Rgba32> input = Image.Load<Rgba32>(inputFile);
				Image<Rgba32> output = input.Clone();

				Grid grid = new Grid(input.Width, input.Height);

				int gx=-1, gy=-1;
				int rx=-1, ry=-1;

				for (int x = 0; x < grid.Width; x++) {
					for(int y = 0; y < grid.Height; y++) {
						byte r = input[x, y].R;
						byte g = input[x, y].G;
						byte b = input[x, y].B;

						if (r == 255 && g == 255 && b == 255) grid.Get(x, y).SetBlocked(true);
						else if (r == 255 && g == 0 && b == 0) {
							rx = x;
							ry = y;
						} else if (r == 0 && g == 255 && b == 0) {
							gx = x;
							gy = y;
						}
					}
				}

				if(rx < 0 || ry < 0 || gx < 0 || gy < 0) {
					Console.WriteLine("No start and end points found!");
					return;
				}

				Node start = grid.Get(gx, gy);
				Node end = grid.Get(rx, ry);

				List<Node> path = grid.GetPath(start, end);

				foreach(Node node in path) {
					if(node != start && node != end) {
						output[node.X, node.Y].FromRgb24(new Rgb24(0, 0, 255));
					}
				}

				output.SaveAsBmp(outputFile);

			} catch(Exception e) {
				Console.WriteLine(e);
				return;
			}
		}

	}
}
