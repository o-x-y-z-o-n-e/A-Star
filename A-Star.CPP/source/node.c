#include "astar.h"
#include "internal.h"
#include <stdlib.h>
#include <stdint.h>


//----------------------------------------------------------------------


void ASR_GetCoords(ASR_Node* node, int* x, int* y) {
	(*x) = node->_x;
	(*y) = node->_y;
}


void ASR_SetBlocked(ASR_Node* node, char blocked) {
	node->_blocked = blocked;
}


void ASR_SetWeight(ASR_Node* node, int weight) {
	node->_weight = weight;
}


//----------------------------------------------------------------------


void clearNode(ASR_Node* node) {
	node->_heapIndex = 0;
	node->_parent = NULL;
	node->_g = 0;
	node->_h = 0;
}


int getF(ASR_Node* node) {
	return node->_g + node->_h;
}


int compareNodes(ASR_Node* left, ASR_Node* right) {
	int comp = 0;
	if (getF(left) < getF(right)) comp = -1;
	else if (getF(left) > getF(right)) comp = 1;

	if (comp == 0) {
		if (left->_h < right->_h) comp = -1;
		else if (left->_h > right->_h) comp = 1;
	}

	return comp;
}


int getDistance(ASR_Node* start, ASR_Node* end) {
	int cost = 0;
	int dx = end->_x - start->_x;
	int dy = end->_y - start->_y;
	int adx = dx * (dx < 0 ? -1 : 1);
	int ady = dy * (dy < 0 ? -1 : 1);

	if (adx < ady) {
		cost += 14 * adx;
		ady -= adx;
		adx = 0;
	} else {
		cost += 14 * ady;
		adx -= ady;
		ady = 0;
	}

	if (adx > 0)
		cost += 10 * adx;
	else
		cost += 10 * ady;

	return cost;
}


int getNodeHash(ASR_Node* node) {
	uint32_t a = (uint32_t)node;

	a -= (a << 6);
	a ^= (a >> 17);
	a -= (a << 9);
	a ^= (a << 4);
	a -= (a << 3);
	a ^= (a << 10);
	a ^= (a >> 15);

	return (int)a;
}