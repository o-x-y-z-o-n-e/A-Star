struct Node {
    _heap_index: mut u32,
    x: u32,
    y: u32,
    g: mut u32,
    h: mut u32,
    weight: mut u32,
    blocked: mut bool,
    parent: *mut Node,
}

//

impl Node {

    pub fn new(x: u32, y: u32) -> Box<Node> {
        box Node {
            x: x,
            y: y,
        }
    }

    pub fn F(&self) -> u32 {
        self.g + self.h
    }


    pub fn Compare(first: &Node, second: &Node) -> i32 {
        let mut c: i32 = 0;

        if first.F() < second.F() {
            c = -1;
        } else if first.F() == second.F() {
            if first.h < second.h {
                c = -1;
            } else if first.h == second.h {
                c = 0;
            } else {
                c = 1;
            }
        } else {
            c = 1;
        }

        -c
    }
}