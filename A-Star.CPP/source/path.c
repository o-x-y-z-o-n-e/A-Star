#include "astar.h"
#include "internal.h"
#include <stdlib.h>

ASR_Path* tracePath(ASR_Grid* grid) {
	ASR_Path* path = (ASR_Path*)malloc(sizeof(ASR_Path));
	if (path == NULL) return NULL;

	ASR_Node* current = grid->_end;
	ASR_Node* start = grid->_start;
	int length = 0;

	while (current != start) {
		length++;
		current = current->_parent;
	} length++; //include start node

	current = grid->_end;

	path->nodes = (ASR_Node*)malloc(sizeof(ASR_Node*) * length);
	if (path->nodes == NULL) {
		free(path);
		return NULL;
	}

	for (int i = 0; i < length; i++) {
		path->nodes[length - 1 - i] = current->_parent;
		current = current->_parent;
	}

	path->length = length;
	return path;
}

void ASR_FreePath(ASR_Path* path) {
	free(path->nodes);
	free(path);
}