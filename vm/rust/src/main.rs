#![allow(dead_code)]

mod vm;

fn main() {
    let mut state = vm::State::new();
    println!("test");
    state.fetch(0xABCDEFE0);
    println!("State: {:#x?}", state);
}
