#include "astar.h"
#include "internal.h"
#include <stdlib.h>

node_set* createSet(int maxSize) {
	node_set* set = (node_set*)malloc(sizeof(node_set));
	if (set == NULL) return NULL;

	set->max = maxSize;
	set->data = (ASR_Node*)malloc(sizeof(ASR_Node*) * maxSize);

	return set;
}


void setAdd(node_set* set, ASR_Node* node) {

}


int setContains(node_set* set, ASR_Node* node) {

}


void freeSet(node_set* set) {
	free(set->data);
	free(set);
}