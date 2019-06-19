## 🔥 FlameVM 
##### (8bit_cpu_host and compiler combine-based-parser)
  
![image](https://user-images.githubusercontent.com/13326808/58775994-0597bc00-85d1-11e9-99c3-e6f7208cd37b.png)


and

##### 8bit_cpu_ui_host

![image](https://user-images.githubusercontent.com/13326808/59545647-ce5ad080-8f29-11e9-8e2a-700cca936d82.png)



##### Try

🌧 Download lates release

💥 Save it to file 'test.asm'
```asm

; set label to index in cell 0xC
.ref_t &(0xC)
; push to device char 'n'(0x6E)
.push_a &(0x1) &(0x6) <| @char_t('n')
; push to device char 'a'(0x79)
.push_a &(0x1) &(0x6) <| @char_t('a')
; push to device char 'y'(0x61)
.push_a &(0x1) &(0x6) <| @char_t('y')
; push to device char ' '(0x20)
.push_a &(0x1) &(0x6) <| @char_t(' ')
; push to device char '\n'(0x0A)
.push_a &(0x1) &(0x6) <| $(0x0A)

; stash memory in device
.push_a &(0x1) &(0x7) <| $(0x00)
; clear memory in device
.push_a &(0x1) &(0x3) <| $(0x00)

; jump to label by index in cell 0xC
.jump_t &(0xC)

```

🐝 Compile binary: `acc.exe -o superBinary -s test.asm`    
⚡️ Execute result: `vm.exe superBinary.exf`    

👑 Complete! You're beautiful!✨ 

##### Samples
See `~/samples/*.asm` files

##### Compiler instruction implemented:
Complete: `.ref_t, .jump_t, .addi, .push_a, .swap, .push_t, .push_x, .push_d, .div, .add, .sub, .pow, .mul, .warm, .halt`    
Todo: `.push_y, .push_b, .move_t, and etc`    

##### Registers

```csharp
.ref_t &(cell_id)                                 // set current program offset to shared memory at cell_id
.push_a &($device_id) &(action_id) <| &(params)  // push params to device_id.action_id 
.swap &(cell_id_source) &(cell_id_target)         // swap memory at two cell index
.halt                                             // shutdown cpu_host
.warm                                             // up cpu_host (warm up cpu cells)
// math instruction
.mul &(cellResult) &(cellValue1) &(cellValue2)
.add &(cellResult) &(cellValue1) &(cellValue2)
.div &(cellResult) &(cellValue1) &(cellValue2)
.sub &(cellResult) &(cellValue1) &(cellValue2)
.pow &(cellResult) &(cellValue1) &(cellValue2)
.sqrt &(cellResult) &(cellValue)
// jump instruction
.jump_t &(cell_id)                  // get program offset in shared memory at cell_id and goto to offset
.jump_e &(cell_id) ~- &(0x9) &(0x6) // if 0x9 cell value more or equal 0x6 cell value
.jump_g &(cell_id) ~- &(0x9) &(0x6) // if 0x9 cell value more 0x6 cell value 
.jump_u &(cell_id) ~- &(0x9) &(0x6) // if 0x9 cell value less 0x6 cell value 
.jump_y &(cell_id) ~- &(0x9) &(0x6) // if 0x9 cell value less or equal 0x6 cell value 

.push_j &($device_id) &(action_id) <| @string_t("test string") // transform instruction, casted to array push_a
```

##### LED Device in CPU UI Host
```CSharp
AddressDev : 0xB
LightAction: 0xD
OffAction  : 0xE
```


##### Env variables

```yaml
- FLAME_TRACE         : 1\0 - enable or disable trace logging (default 0)
- FLAME_ERROR         : 1\0 - enable or disable error logging (default 1)
- FLAME_KEEP_MEMORY   : 1\0 - when halt cpu disable or enable clearing memory table (default 0 - clearing)
- FLAME_MEM_FAST_WRITE: 1\0 - enable or disable fast-write mode to devices (see fast-mode addressing)
```
##### fast-mode addressing        
Write speedUp to device memory (x12~ times), but disables the ability to write to certain sections of device memory.

