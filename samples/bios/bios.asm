#{
    ~label 'memory' 0x0
    ~label 'bios' 0x2
    ~label 'hpet' 0x1
    ~label 'channel' 0x2
    ~label 'boot' 0xF
}
; set page to 0x400 (bios stage)
.raw 0x03300000EF
; set memory channel zero
.mva &(![~memory]) &(0x0)         <| $(0x1)
; enable hpet
.mva &(![~bios])   &(![~hpet])    <| $(0x1)
; set memory channel reading
.mva &(![~bios])   &(![~channel]) <| $(0x0)
; start booting
.mva &(![~bios])   &(![~boot])    <| $(0x0)
; jump to page 0x600 (executing stage)
.raw 0x090100000
