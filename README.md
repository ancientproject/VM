## 4bit_cpu_host

##### Registers

```
loadi  - 0x1    - load value into reg_state at reg_index
add    - 0x2    - sum value at reg_index{2} in reg_state
dead   - 0xDEAD - halt cpu
swap   - 0x3    - swipe two reg_value at reg_index{2}
dump_l - 0xF    - dump reg_value at last reg_index
dump_p - 0xE    - dump reg_value at current reg_index
```
