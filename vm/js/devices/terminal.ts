import * as os from "os";

export default() => {
    return {
        name: "terminal",
        addr: 0x1,
        write(address, data){
            switch(address)
            {
                case 0x5:
                    this.writeChar(String.fromCharCode(data));
                    break;
            }
        },
        read(address){
            throw new Error(`Access memory 0x${address.toString(16)}. Memory could not be read.`)
        },
        writeChar(c) {
            if(c.charCodeAt(0) == 0xA)
                process.stdout.write(os.EOL);
            else
                process.stdout.write(c);
        }
    }
}