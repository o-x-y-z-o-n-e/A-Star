#include "astar.h"
#include "internal.h"
#include <stdlib.h>


//----------------------------------------------------------------------


ASR_Grid* ASR_Create(int width, int height) {
	ASR_Grid* grid = (ASR_Grid*)malloc(sizeof(ASR_Grid));
	if (grid == NULL) return NULL;

	grid->_width = width * (width < 0 ? -1 : 1);
	grid->_height = height * (height < 0 ? -1 : 1);

	grid->_map = (ASR_Node**)malloc(sizeof(ASR_Node) * width * height);

	grid->_start = NULL;
	grid->_end = NULL;

	return grid;
}


ASR_Path* ASR_FindPath(ASR_Grid* grid, ASR_Node* start, ASR_Node* end) {
	clearGrid(grid);

	grid->_start = start;
	grid->_end = end;

	char found = 0;

	node_heap* open = createHeap(grid->_width * grid->_height);
	node_set* closed = createSet(grid->_width * grid->_height);

	heapAdd(open, start);

	while (open->count > 0) {
		ASR_Node* current = open->data[0];

		heapRemove(open, current->_heapIndex);
		setAdd(closed, current);

		if (current == end) {
			found = 1;
			break;
		}

		addOpenNeighbors(grid, current, open, closed);
	}

	freeHeap(open);
	freeSet(closed);

	if (found) {
		return tracePath(grid);
	} else
		return NULL;
}


void ASR_Destroy(ASR_Grid* grid) {
	free(grid->_map);
	free(grid);
}


ASR_Node* ASR_GetNode(ASR_Grid* grid, int x, int y) {
	if (x < 0 || x > grid->_width) return NULL;
	if (y < 0 || y > grid->_height) return NULL;

	return &grid->_map[x][y];
}


void ASR_GetDimensions(ASR_Grid* grid, int* w, int* h) {
	(*w) = grid->_width;
	(*h) = grid->_height;
}


int ASR_IsOnGrid(ASR_Grid* grid, int x, int y) {
	return (x < 0 || x >= grid->_width || y < 0 || y >= grid->_height);
}


//----------------------------------------------------------------------


void clearGrid(ASR_Grid* grid) {
	for (int x = 0; x < grid->_width; x++) {
		for (int y = 0; y < grid->_height; y++) {
			clearNode(&grid->_map[x][y]);
		}
	}

	grid->_start = NULL;
	grid->_end = NULL;
}


void addOpenNeighbors(ASR_Grid* grid, ASR_Node* node, node_heap* open, node_set* closed) {
	int count = 0;
	ASR_Node** points = getNeighbors(grid, node, &count);

	for (int i = 0; i < count; i++) {
		if (setContains(closed, points[i]))
			continue;

		int cost = node->_g + getDistance(node, points[i]) + points[i]->_weight;

		int isOpen = heapContains(open, points[i]);

		if (cost < points[i]->_g || !isOpen) {
			points[i]->_g = cost;
			points[i]->_h = getDistance(points[i], grid->_end);
			points[i]->_parent = node;

			if (!isOpen) {
				heapAdd(open, points[i]);
			} else {
				heapPercolateUp(open, points[i]->_heapIndex);
			}
		}
	}
}


ASR_Node** getNeighbors(ASR_Grid* grid, ASR_Node* node, int* count) {
	ASR_Node* nodes[8];
	(*count) = 0;

	for (int x = -1; x <= 1; x++) {
		for (int y = -1; y <= 1; y++) {
			if (x == 0 && y == 0) continue;

			int gx = node->_x + x;
			int gy = node->_y + y;

			if (ASR_IsOnGrid(grid, gx, gy)) {
				ASR_Node* n = ASR_GetNode(grid, gx, gy);

				if(!n->_blocked) {
					nodes[*count] = n;
					(*count)++;
				}
			}
		}
	}

	return nodes;
}