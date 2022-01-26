#include "astar.h"
#include "internal.h"
#include <stdlib.h>


node_heap* createHeap(int maxSize) {
	node_heap* heap = (node_heap*)malloc(sizeof(node_heap));
	if (heap == NULL) return NULL;

	heap->max = maxSize;
	heap->data = (ASR_Node*)malloc(sizeof(ASR_Node*) * maxSize);

	return heap;
}


void heapAdd(node_heap* heap, ASR_Node* node) {
	heap->data[heap->count] = node;
	node->_heapIndex = heap->count;

	heapPercolateUp(heap, heap->count);

	heap->count++;
}


int heapContains(node_heap* heap, ASR_Node* node) {
	return node == heap->data[node->_heapIndex];
}


ASR_Node* heapRemove(node_heap* heap, int i) {
	ASR_Node* node = &(heap->data[i]);
	node->_heapIndex = 0;

	heap->count--;

	heap->data[i] = heap->data[heap->count];
	heap->data[i]->_heapIndex = i;

	heap->data[heap->count] = NULL;

	heapPercolateDown(heap, i);

	return node;
}


void heapPercolateUp(node_heap* heap, int i) {
	int parentIndex = (i - 1) / 2;

	ASR_Node* parent = heap->data[parentIndex];
	ASR_Node* node = heap->data[i];

	if (compareNodes(node, parent) < 0) {
		heapSwap(heap, i, parentIndex);
		heapPercolateUp(heap, parentIndex);
	}
}


void heapPercolateDown(node_heap* heap, int i) {
	int leftIndex = (2 * i) + 1;
	int rightIndex = leftIndex + 1;

	int swapIndex = 0;

	if (leftIndex < heap->count) {
		swapIndex = leftIndex;

		if (rightIndex < heap->count) {
			if (compareNodes(heap->data[rightIndex], heap->data[leftIndex]) < 0)
				swapIndex = rightIndex;
		}

		if (compareNodes(heap->data[i], heap->data[swapIndex]) > 0) {
			heapSwap(heap, i, swapIndex);
			heapPercolateDown(heap, swapIndex);
		}
	}
}


void heapSwap(node_heap* heap, int a, int b) {
	ASR_Node* swap = heap->data[a];

	heap->data[a] = heap->data[b];
	heap->data[b] = swap;

	heap->data[a]->_heapIndex = a;
	heap->data[b]->_heapIndex = b;
}


void freeHeap(node_heap* heap) {
	free(heap->data);
	free(heap);
}