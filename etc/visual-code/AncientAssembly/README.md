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
<p align="center">
  <a href="#">
    <img src="https://dev.azure.com/0xF6/AncientVM/_apis/build/status/0xF6.ancient_cpu?branchName=master">
    <img src="https://img.shields.io/:license-MIT-blue.png">
    <img src="https://img.shields.io/github/release/0xF6/ancient_cpu.png">
  </a>
  <a href="https://t.me/ivysola">
    <img src="https://img.shields.io/badge/Ask%20Me-Anything-1f425f.png">
  </a>
  <a href="https://www.codacy.com/app/0xF6/cpu_4bit?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=0xF6/cpu_4bit&amp;utm_campaign=Badge_Grade"><img src="https://api.codacy.com/project/badge/Grade/e033b506944447289b2ef39478fc8234"/></a>
  </a>
</p>

![image](https://user-images.githubusercontent.com/13326808/60311909-e71fa900-9961-11e9-96f0-bf4c4a45681c.png)




##### Env flags 
```yaml
- VM_TRACE         : 1\0    - enable or disable trace logging (default 0)
- VM_ERROR         : 1\0    - enable or disable error logging (default 1)
- VM_KEEP_MEMORY   : 1\0    - when halt cpu disable or enable clearing memory table (default 0 - clearing)
- VM_MEM_FAST_WRITE: 1\0    - enable or disable fast-write mode to devices (see fast-mode addressing)
- C69_BIOS_HPET    : 1\0    - enable using hardware hper timer (default 0)
- VM_WARMUP_DEV    : 1\0    - enable warm-up devices on plug-connect (default 1)
- VM_SHUTDOWN_DEV  : 1\0    - enable shutdown devices on halting processor (default 1)
- VM_SYM_ENCODING  : "utf8" - set encoding of debug symbols (default "IBM037")
```

##### Memory table: 
  
example:  
```assembler
.ldx &(0x11) $(0x0) - disable trace
``` 
list: 
```yaml
- 0x11 : 1\0 - enable or disable trace logging (default 0)
- 0x12 : 1\0 - enable or disable error logging (default 1)
- 0x13 : 1\0 - when halt cpu disable or enable clearing memory table (default 0 - clearing)
- 0x14 : 1\0 - enable or disable fast-write mode to devices (see fast-mode addressing)
- 0x18 : 1\0 - enable float-mode
- 0x19 : 1\0 - enable stack-forward flag
- 0x20 : 1\0 - control stack flag (north flag)
- 0x21 : 1\0 - control stack flag (east flag)
- 0x22 : 1\0 - bios read-access flag
```

##### Bios table: 

###### public memory [READ]: 
```yaml
- 0x00 - return current ticks (u32)
- 0x01 - return hpet enabled or not
- 0x02 - return memory channel
- 0xAX - private memory randge
```
###### private memory [READ]:
```yaml
- 0xA1 : return hpet enabled or not
- 0xA2 : return use virtual stack forwarding or not
- 0xA3 : return use forward in standalone memory sector or not
- 0xA4 : return using guarding with violation memory write or not (default bios_guard_flag has enabled)
```

###### public\private memory [WRITE]: 
  
```yaml
- 0x1 : 1\0 - set hpet use or not (default value depends on the firmware)
- 0xF : reseting hpet and system timers
- 0xD : call system interrupts for N-value ms
- 0xC : call clearing RAM (need enabled bios_guard_flag, and disabled southFlag)
- 0xA : set at private memory range value (need southFlag)
```


##### remarks:
###### fast-mode addressing        
`Write speedUp to device memory (x12~ times), but disables the ability to write to certain sections of device memory.`
###### READ\WRITE operation for bios
`Need southFlag enabled for READ\WRITE operation for private memory, otherwise will be calling CorruptedMemoryException and halting cpu`
###### bios_guard_flag
`Some memory segments are not allowed to READ\WRITE operation when bios_guard_flag is enabled`
##### Command docs

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
// advanced math (need float-mode) (advanced math operation send result to stack)
.abs &(cell_value)
.acos &(cell_value)
.atan &(cell_value)
.acosh &(cell_value)
.atanh &(cell_value)
.asin &(cell_value)
.asinh &(cell_value)
.cbrt &(cell_value) 
.cell &(cell_value) // celling
.cos &(cell_value)
.cosh &(cell_value)
.flr &(cell_value) // floor
.exp &(cell_value) // exponent
.log &(cell_value)
.log10 &(cell_value)
.tan &(cell_value)
.tanh &(cell_value)
.sin &(cell_value)
.sinh &(cell_value)
.trc &(cell_value) // truncate
.bitd &(cell_value) // bit decrement
.biti &(cell_value) // bit increment
.atan2 &(cell_value) &(cell_value)
.min &(cell_value) &(cell_value)
.max &(cell_value) &(cell_value)

                              
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