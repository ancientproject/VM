##  

<!-- Logo -->
<p align="center">
  <a href="#">
    <img height="128" width="128" src="https://raw.githubusercontent.com/0xF6/ancient_cpu/master/Rune/resource/icon.png">
  </a>
</p>

<!-- Name -->
<h1 align="center">
  AncientVM 🔥
</h1>
<!-- desc -->
<h4 align="center">
  8bit CPU Virtual Machine & Ancient Assembler-style language
</h4>

  
![image](https://user-images.githubusercontent.com/13326808/60311909-e71fa900-9961-11e9-96f0-bf4c4a45681c.png)

##### Flags

in env:   
```yaml
- VM_TRACE         : 1\0 - enable or disable trace logging (default 0)
- VM_ERROR         : 1\0 - enable or disable error logging (default 1)
- VM_KEEP_MEMORY   : 1\0 - when halt cpu disable or enable clearing memory table (default 0 - clearing)
- VM_MEM_FAST_WRITE: 1\0 - enable or disable fast-write mode to devices (see fast-mode addressing)
```

in runtime:
```yaml
.ldx &(0x11) $(0x0) - disable trace

- 0x11 : 1\0 - enable or disable trace logging (default 0)
- 0x12 : 1\0 - enable or disable error logging (default 1)
- 0x13 : 1\0 - when halt cpu disable or enable clearing memory table (default 0 - clearing)
- 0x14 : 1\0 - enable or disable fast-write mode to devices (see fast-mode addressing)
```

##### fast-mode addressing        
Write speedUp to device memory (x12~ times), but disables the ability to write to certain sections of device memory.

##### Registers

```csharp
-- legend:

- cell_id  - memory cell in processor cache
- value    - hex number
- &()      - reference cell id
- $()      - value
- ![~name] - reference label define
- <| and |>- pipe operator
- ~-       - when operator
```


```csharp
// refer and jumper

// set reference current program offset to cell_id
.ref_t &(cell_id)
// read from cell_id offset program and go to
.jump_t &(cell_id)

// other jumper
.jump_e &(cell_id) ~- &(0x9) &(0x6) // if 0x9 cell value more or equal 0x6 cell value
.jump_g &(cell_id) ~- &(0x9) &(0x6) // if 0x9 cell value more 0x6 cell value 
.jump_u &(cell_id) ~- &(0x9) &(0x6) // if 0x9 cell value less 0x6 cell value 
.jump_y &(cell_id) ~- &(0x9) &(0x6) // if 0x9 cell value less or equal 0x6 cell value 


// manage processor

.halt  // halting cpu
.warm  // warm-up cpu


// etc

.swap &(source) &(target) // swap value

// math instruction
.mul &(result_cell) &(cellValue1) &(cellValue2)
.add &(result_cell) &(cellValue1) &(cellValue2)
.div &(result_cell) &(cellValue1) &(cellValue2)
.sub &(result_cell) &(cellValue1) &(cellValue2)
.pow &(result_cell) &(cellValue1) &(cellValue2)
.sqrt &(result_cell) &(cellValue)
                              
// manage device and etc
.mvt &($device_id) &(action_id) <| $(value)       // push raw value to device_id.action_id in bus
.mvd &($device_id) &(action_id) <| &(cell_value)  // push value from cell to device_id.action_id in bus
.mvx &($device_id) &(action_id) <| &(value_ref)   // encode memory and send char-data to device

// float-point
.ldx &(0x18) <| $(0x1) // set float mode
// remark: all math operation support float mode
.orb &(n)             // grub next 'n'-count values and stage to stack
.val @float_t("10.4") // encode float value
.pull &(target_cell)  // read from stack float value and insert to target_cell

// debugger
.brk_s // standard break - now break
.brk_n // break on next cycle execute
.brk_a // break on after next cycle execute

// other
.inc &(cell) // cell++
.dec &(cell) // cell--
// raw write instruction
.raw 0xABCDEFE0 // (warm)

.mvj &($device_id) &(action_id) <| @string_t("test string") // cast string to mvt instruction
```

##### Devices spec

###### UI-LED
```CSharp
AddressDev : 0xB
LightAction: 0xD
OffAction  : 0xE
```

