#{
    ~label 'reactor_bios' 0x46
    ~label 'pc0' 0xF3
    ~label 'buffer' 0x0
    ~label 'float_flag' 0x18
    ~label 'fast_forward' 0x19
    ~label 'true' 0x1
    ~label 'false' 0x0
}
.ldx &(![~float_flag]) <| $(![~true])

; load power-value
.rfd &(![~reactor_bios]) &(![~pc0]) 
.pull &(![~buffer])

; |x arctan(x) cos(x)|
.cos &(![~buffer])
.atan &(![~buffer])

.pull &(0x2) 
.pull &(0x3)
 
.mul &(0x1) &(0x2) &(0x3)
.ldi &(0x2) <| $(![~false])
.ldi &(0x3) <| $(![~false])
.swap &(0x1) &(0x2)
.mul &(![~buffer]) &(![~buffer]) &(0x1)
.ldx &(![~fast_forward]) <| $(![~true])
.abs &(![~buffer])
; result in ~buffer cell
.brk.s

