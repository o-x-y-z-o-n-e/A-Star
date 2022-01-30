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
	//if (set->count >= set->max) return;

	int hash = getNodeHash(node);

	int i = 0;
	while (i < set->max) {
		int index = (hash + (i*i)) % set->max;

		if (set->data[i] == NULL) {
			set->data[i] = node;
			(set->count)++;
			return;
		}

		i++;
	}
}


int setContains(node_set* set, ASR_Node* node) {
	int hash = getNodeHash(node);

	int i = 0;
	while (i < set->count) {
		int index = (hash + (i * i)) % set->max;

		if (set->data[i] == NULL)
			return 0;

		if (set->data[i] == node)
			return 1;
		
		i++;
	}

	return 0;
}


void setRemove(node_set* set, ASR_Node* node) {
	if (set->count <= 0) return;

	int hash = getNodeHash(node);

	int i = 0;
	while (i < set->count) {
		int index = (hash + (i * i)) % set->max;

		if (set->data[i] == NULL)
			return;

		if (set->data[i] == node) {
			set->data[i] = NULL;
			(set->count)--;
			return;
		}

		i++;
	}
}


void freeSet(node_set* set) {
	free(set->data);
	free(set);
}