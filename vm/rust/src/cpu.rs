#[derive(Debug)]
pub struct CPU {
    pub state: State,
    pub bus: Bus,
}

impl CPU {
    pub fn new(&b: Bus) -> Bus
    {
        CPU { bus: &b, state: &b.state }
    }

    pub fn step(&self)
    {
        self.state.accept(self.state.fetch());
        self.state.eval();
    }
}