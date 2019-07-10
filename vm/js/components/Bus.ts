import { State, CPU } from "./"
import corruptedDevice from "../devices/corruptedDevice";

export class Bus {
    public state: State;
    public cpu: CPU;
    public boundaries: number[];
    public devices: any[];
    constructor(){
        this.state = new State(this);
        this.cpu = new CPU(this);
        this.boundaries = [];
        this.devices = [];
    }

    public write(address, data)
    {
        var device = this.find(address);
        device.write((address - device.addr), data);
    }
    public find(address)
    {
        var idx = this.binarySearch(this.boundaries, address);
        if (idx < 0) idx = -idx - 2;
        if (idx < 0) return corruptedDevice();
        return this.devices[idx];
    }
    public add(device)
    {
        this.devices.push(device);
        this.boundaries.push(device.addr);
        this.boundaries.sort();
        this.devices.sort();
    }

    private binarySearch(array, value) {
        var guess,
            min = 0,
            max = array.length - 1;
        while(min <= max){
            guess = Math.floor((min + max) /2);
            if(array[guess] === value)
                return guess;
            else if(array[guess] < value)
                min = guess + 1;
            else
                max = guess - 1;	
        }
        return (0-1);
    }
}
