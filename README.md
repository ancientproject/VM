## 4bit_cpu_host

##### Try

🌧 Download lates release

File 'test.asm'
```asm

.ref_t &(0xC)                      // label set at 0xC
.push_a &(0x1) &(0x6) @>> &(0x6E)  // push 'n'(0x6E) to terminal device(0x1 - ID) and call StageChar(0x6) action
.push_a &(0x1) &(0x6) @>> &(0x79)  // push 'y'(0x79) to terminal device(0x1 - ID) and call StageChar(0x6) action
.push_a &(0x1) &(0x6) @>> &(0x61)  // push 'a'(0x61) to terminal device(0x1 - ID) and call StageChar(0x6) action
.push_a &(0x1) &(0x6) @>> &(0x20)  // push ' '(0x20) to terminal device(0x1 - ID) and call StageChar(0x6) action

.push_a &(0x1) &(0x7) @>> &(0x00)  // call PushRel(0x7) action in terminal device(0x1)
.push_a &(0x1) &(0x3) @>> &(0x00)  // call ClearStack(0x3) action in terminal device(0x1)

.jump_t &(0xC)                     // goto label 0xC
```

🐝 Compile binary: `acc.exe -o nameOfBinary -s .\test.asm`    
⚡️ Execute result: `vm.exe nameOfBinary.exf`    

👑 Complete! You're beautiful!✨ 

##### Compiler instruction implemented:
Complete: `.ref_t, .jump_t, .addi, .push_a, .swap`    
Todo: `.halt, .mul, .div, .sub, .sum, .push_x, .push_y, .push_b, .move_t, and etc`    

##### Registers

```csharp
.ref_t &(cell_id)                                 // set current program offset to shared memory at cell_id
.jump_t &(cell_id)                                // get program offset in shared memory at cell_id and goto to offset
.push_a &($device_id) &(action_id) @>> &(params)  // push params to device_id.action_id 
.swap &(cell_id_source) &(cell_id_target)         // swap memory at two cell index
```
