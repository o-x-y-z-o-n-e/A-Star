struct Heap {

    data: Box<[*mut Node]>,
    count: mut u32,

}

//

impl Heap {

    pub fn new(max_size: u32) -> Box<Heap> {
        box Heap {
            data: Box::new([*mut Node; max_size]),
            count: 0,
        }
    }


    pub fn Add(&self, node: &Node) {
        self.data[self.count] = node;
        node._heap_index = self.count;

        self.PercolateUp(self.count);

        self.count++;
    }


    fn PercolateUp(&self, index: u32) {
        let parent_index = (index - 1) / 2;

        let parent: *Node = &self.data[parent_index];
        let node: *Node = &self.data[index];

        if Node.Compare(node, parent) > 0 {
            self.Swap(index, parent_index);
            self.PercolateUp(parent_index);
        }
    }

    fn PercolateDown(&self, index: u32) {
        left_index: u32 = 2 * index + 1;
        right_index: u32 = 2 * index + 2;

        swap_index: mut u32 = 0;

        if left_index < self.count {
            swap_index = left_index;

            if right_index < self.count {
                if self.data[right_index].F() < self.data[left_index].F() {
                    swap_index = right_index;
                }
            }

            if Node.Compare(self.data[index], self.data[swap_index]) < 0 {
                self.Swap(index, swap_index);
                self.PercolateDown(swap_index);
            }
        }
    }

    fn Swap(&self, first: u32, second: u32) {
        let temp: *Node = &self.data[first];

        self.data[first] = &self.data[second];
        self.data[second] = &temp;
    }

}