#[derive(Debug)]
pub struct Bus {
    pub state: State
}
pub struct Device {
    pub address: u32
}
impl Device {
    pub fn write(&self, address: u32, data: u32)
    {

    }
}
impl Bus {
    pub fn new(&st: State) -> Bus
    {
        Bus { state: st }
    }
    pub fn find(&self, address: u32) -> Device
    {
        Device { address: 0x1 }
    }
}