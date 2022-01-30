#ifndef INTERNAL_H
#define INTERNAL_H

#include "astar.h"


typedef struct node_heap {
	ASR_Node** data;
	int max;
	int count;
} node_heap;


typedef struct node_set {
	ASR_Node** data;
	int max;
	int count;
} node_set;


//grid & node
void clearGrid(ASR_Grid* grid);
void clearNode(ASR_Node* node);
int getNodeHash(ASR_Node* node);
int getF(ASR_Node* node);
int compareNodes(ASR_Node* left, ASR_Node* right);
int getDistance(ASR_Node* start, ASR_Node* end);
void addOpenNeighbors(ASR_Grid* grid, ASR_Node* node, node_heap* open, node_set* closed);
ASR_Node** getNeighbors(ASR_Grid* grid, ASR_Node* node, int* count);
ASR_Path* tracePath(ASR_Grid* grid);

//heap
node_heap* createHeap(int maxSize);
void heapAdd(node_heap* heap, ASR_Node* node);
int heapContains(node_heap* heap, ASR_Node* node);
ASR_Node* heapRemove(node_heap* heap, int i);
void heapPercolateUp(node_heap* heap, int i);
void heapPercolateDown(node_heap* heap, int i);
void heapSwap(node_heap* heap, int a, int b);
void freeHeap(node_heap* heap);


//set
node_set* createSet(int maxSize);
void setAdd(node_set* set, ASR_Node* node);
int setContains(node_set* set, ASR_Node* node);
void freeSet(node_set* set);

#endif