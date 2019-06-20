import { Bus } from "./components"
import term from "./devices/terminal";

const bus = new Bus();

bus.add(term());


bus.state.load([0xABCDEFE0, 0xF150078C, 0xF150075C, 0xF150079C, 0xF15000AC]);


while(bus.state.halt == 0) {
    bus.cpu.step();
}