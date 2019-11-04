## REPL 
  
`0xB00B50000` - halt off [`Bootable sector not found`]  
    
`0x1110010A0`/`0x1110000A0` - enable/disable trace log      
        
`0x1190010A0`/`0x1190000A0` - enable/disable stack forward flag     
`0x1180010A0`/`0x1180000A0` - enable/disable float flag     
        
`0x1220010A0`/`0x1220000A0` - enable/disable southFlag      
`0x1210010A0`/`0x1210000A0` - enable/disable eastFlag     
`0x1200010A0`/`0x1200000A0` - enable/disable northFlag      
  
### Check status of bios-guard    
`0x1220010A0` - enable south flag   
(`.ldx 0x12 0x1`)   
`0xA4450F600` - reading from bios this flag and insert into stack     
(`.rfd 0x45 0xF6`)    
`0xA19000000` - pulling from stack and insert value into 0x9 memory cell        
(`.pull 0x9`)     
`0xF14E900F0` - reading from 0x9 memory cell and send to terminal device for print value on screen    
(`.mvx 0x1 0x4 0x9`)      
