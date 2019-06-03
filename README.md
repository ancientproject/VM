## 🔥 4bit_cpu_host

![image](https://user-images.githubusercontent.com/13326808/58775994-0597bc00-85d1-11e9-99c3-e6f7208cd37b.png)



##### Try

🌧 Download lates release

File 'test.asm'
```asm

.ref_t &(0xC)                          // label set at 0xC
.push_a &(0x1) &(0x6) |> &(0x6E)       // push 'n'(0x6E) to terminal device(0x1 - ID) and call StageChar(0x6) action
.push_a &(0x1) &(0x6) |> &(0x79)       // push 'y'(0x79) to terminal device(0x1 - ID) and call StageChar(0x6) action
.push_a &(0x1) &(0x6) |> @char_t('a')  // push 'a'(0x61) to terminal device(0x1 - ID) and call StageChar(0x6) action
.push_a &(0x1) &(0x6) |> @char_t(' ')  // push ' '(0x20) to terminal device(0x1 - ID) and call StageChar(0x6) action

.push_a &(0x1) &(0x7) |> &(0x00)  // call PushRel(0x7) action in terminal device(0x1)
.push_a &(0x1) &(0x3) |> &(0x00)  // call ClearStack(0x3) action in terminal device(0x1)

.jump_t &(0xC)                     // goto label 0xC
```

🐝 Compile binary: `acc.exe -o nameOfBinary -s .\test.asm`    
⚡️ Execute result: `vm.exe nameOfBinary.exf`    

👑 Complete! You're beautiful!✨ 

##### Samples
See `~/samples/*.asm` files

##### Compiler instruction implemented:
Complete: `.ref_t, .jump_t, .addi, .push_a, .swap, .push_t, .push_x, .push_d, .div, .add, .sub, .pow, .mul, .warm, .halt`    
Todo: `.push_y, .push_b, .move_t, and etc`    

##### Registers

```csharp
.ref_t &(cell_id)                                 // set current program offset to shared memory at cell_id
.jump_t &(cell_id)                                // get program offset in shared memory at cell_id and goto to offset
.push_a &($device_id) &(action_id) |> &(params)  // push params to device_id.action_id 
.swap &(cell_id_source) &(cell_id_target)         // swap memory at two cell index
.halt                                             // shutdown cpu_host
.warm                                             // up cpu_host (warm up cpu cells)
// math instruction
.mul &(cellID1) &(cellID2)
.add &(cellID1) &(cellID2)
.div &(cellID1) &(cellID2)
.sub &(cellID1) &(cellID2)
.pow &(cellID1) &(cellID2)

.push_j &($device_id) &(action_id) <| @string_t("test string") // transform instruction, casted to array push_a
```
