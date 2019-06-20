export default() => {
    return {
        name: "<unk-dev>",
        addr: 0xFF,
        read(address) {
            throw new Error(`Access memory 0x${address.toString(16)}. Memory could not be read.`)
        },
        write(address, data) {
            throw new Error(`Access memory 0x${address.toString(16)}. Memory could not be write.`)
        }
    }
}