#include "astar.h"
#include "internal.h"
#include <stdlib.h>


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

}