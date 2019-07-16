#{
    ~label 'z'    0x05 ~label 'a'    0x06 ~label 'b' 0x07
    ~label 'bios' 0x45 ~label 'tick' 0x05
}
{| this is fucking comment line |}
.page ![0x600]
.prune (0x0E1)<->(0x0D2)
.warm
.unlock &(0x000)
.rf.d   &(![~bios]) &(![~tick])
.pull   &(![~z])
.ld.x   &(0x009)    $(0x4CE3)
.mul    $(0x01F)    &(![~z])
.ixor   &(0x009)    &(![~z])
.dup    &(![~z]) |> &(0x012)
.stl.c  &(0x012)
.call.i !{sys->external->module(u32)}
.add    $(0xD00) &(![~z])
.mull   $(0xAD0) &(![~z])
.lp.str &[{"random value: "}]
.pull   &(0x008)
.mv.x   &(0x015) <| &(0x008)
.mv.x   &(0x015) <| &(![~z])
.halt

@float_t(12.2)