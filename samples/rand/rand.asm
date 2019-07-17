#{
    ~label 'z'    0x05 ~label 'a'    0x06 ~label 'b' 0x07
    ~label 'bios' 0x45 ~label 'tick' 0x05
}
.dup &(![~z]) &(0x1)

.locals init #(
    [0x0] u32
    [0x1] u8
) 
; ...
.prune
.dup &(![~z]) &(0x1)

.locals init #(
    [0x0] f64,
    [0x1] u8,
    [0x2] u8,
    [0x3] u8,
    [0x4] u8,
    [0x5] u8
) 
; ...
.prune
.dup &(![~z]) &(0x1)

.locals init #(
    [0x0] f64
    [0x1] u8
    [0x2] u8
    [0x3] u8
    [0x4] u8
    [0x5] u8
) 
; ...
.prune
.dup &(![~z]) &(0x1)