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

