import { State } from ".";

export class CPU {
    private _bus;
    private state: State;
    constructor(bus){
        this._bus = bus;
        this.state = bus.state;
    }

    public step() {
        try
        {
            this.state.accept(this.state.fetch());
            this.state.eval();
        }
        catch (e) { }
    }
    public halt(x = 0x0) {
        this.state.halt = 1;
        console.error(`halt: ${x.toString(16)}`);
    }
}