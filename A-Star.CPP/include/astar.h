#ifndef ASTAR_H
#define ASTAR_H


typedef struct ASR_Node ASR_Node;
struct ASR_Node {
	int _heapIndex;
	ASR_Node* _parent;
	int _g;
	int _h;

	int _x;
	int _y;

	int _weight;
	char _blocked;
};


typedef struct ASR_Grid {
	int _width;
	int _height;
	ASR_Node** _map;
	ASR_Node* _start;
	ASR_Node* _end;
} ASR_Grid;


typedef struct ASR_Path {
	ASR_Node** nodes;
	int length;
} ASR_Path;


ASR_Grid* ASR_Create(int width, int height);
ASR_Path* ASR_FindPath(ASR_Grid* grid, ASR_Node* start, ASR_Node* end);
void ASR_FreePath(ASR_Path* path);
void ASR_Destroy(ASR_Grid* grid);

int ASR_IsOnGrid(ASR_Grid* grid, int x, int y);
ASR_Node* ASR_GetNode(ASR_Grid* grid, int x, int y);
void ASR_GetDimensions(ASR_Grid* grid, int* w, int* h);
void ASR_GetCoords(ASR_Node* node, int* x, int* y);
void ASR_SetBlocked(ASR_Node* node, char blocked);
void ASR_SetWeight(ASR_Node* node, int weight);

#endif