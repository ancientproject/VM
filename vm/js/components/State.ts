import * as _ from "ramda";
import { Bus } from ".";
export class State {
    private _bus: Bus;
    iid; pc; r1; r2; r3; u1; u2; x1; x2;
    tc: boolean; ec: boolean; km: boolean; fw: boolean;
    curAddr: number; lastAddr: number;
    halt: number;
    regs: number[];
    mem: number[];
    constructor(bus: Bus){
        this._bus = bus;
        this.iid = this.r1 = 
        this.r2 = this.r3 = 
        this.u1 = this.u2 = 
        this.x1 = this.x2 = 0xFF;

        this.pc = 0;

        this.tc = process.env.FLAME_TRACE == "1";
        this.ec = process.env.FLAME_ERROR != "0";
        this.km = process.env.FLAME_KEEP_MEMORY == "1";
        this.fw = process.env.FLAME_MEM_FAST_WRITE == "1";

        this.curAddr = 0xFFFF;
        this.lastAddr = 0xFFFF;

        this.halt = 0;
        this.regs = new Array(32);
        this.regs = this.regs.fill(0x0, 0, 32);

        this.mem = [];
    }
    public fetch() {
        this.lastAddr = this.curAddr;
        if(this.mem.length != this.pc || this.halt != 0)
            return (this.curAddr = this.mem[this.pc++]);
        this.mem.fill(0xFF, 0, 24);
        return 0x0;
    }
    public load(params: number[]){
        let mem = this.mem;
        params.forEach(element => {
            mem.push(element);
        });
    }
    public accept(memory) {
        this.iid = ((memory & 0xF0000000) >>> 28);
        this.r1  = ((memory & 0xF000000 ) >>> 24);
        this.r2  = ((memory & 0xF00000  ) >>> 20);
        this.r3  = ((memory & 0xF0000   ) >>> 16);
        this.u1  = ((memory & 0xF000    ) >>> 12);
        this.u2  = ((memory & 0xF00     ) >>> 8);
        this.x1  = ((memory & 0xF0      ) >>> 4);
        this.x2  =  (memory & 0xF             );
    }
    public eval() {
        switch(this.iid) {
            case 0xA:
                break;
            case 0x0:
                console.log(this._bus.state);
                this._bus.cpu.halt();
                break;
            case 0xF:
                if(_.all((x) => x == 0xF)([this.r1,this.r2,this.r3,this.u1,this.u2,this.x1,this.x2]))
                    this._bus.cpu.halt();
                if(this.x2 == 0xC)
                    this._bus.find(this.r1 & 0xFF).write(this.r2 & 0xFF, (this.r3 << 12 | this.u1 << 8 | this.u2 << 4 | this.x1) & 0xFFFFFFF);
                break;
            case 0x1:
                if(this.x2 == 0xA)
                {
                    this.regs[((this.r1 << 4) | this.r2)] = ((this.u1 << 4) | this.u2);
                    break;
                }
                if (this.u2 != 0)
                    this.regs[this.r1] = ((this.u1 << 4) | this.u2);
                else
                    this.regs[this.r1] = this.u1;
                break;
            case 0x2:
                this.regs[this.r1] = this.regs[this.r2] + this.regs[this.r3];
                break;
            case 0x4:
                this.regs[this.r1] = this.regs[this.r2] - this.regs[this.r3];
                break;
            case 0x5:
                this.regs[this.r1] = this.regs[this.r2] * this.regs[this.r3];
                break;
            case 0x6:
                if (this.regs[this.r3] == 0)
                {
                    this._bus.cpu.halt(0xC);
                    break;
                }
                this.regs[this.r1] = this.regs[this.r2] / this.regs[this.r3];
                break;
            case 0x8:// when u2 == 0xF && x1 == 0x0: // 0x8F000F00
                if(this.u2 == 0xF && this.x1 == 0x0)
                this.pc = this.regs[this.r1];
                break;
            case 0x8:// when u2 == 0xF && x1 == 0x0: // 0x8F000F00
                if(this.u2 == 0xF && this.x1 == 0x0)
                    this.pc = this.regs[this.r1];
                break;
            default:
                console.log(`Unk opcode: ${this.iid.toString(16)}`)
                break;
        }
    }
}