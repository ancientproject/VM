
.warm_up    $0
.loadi      $0, $6
.dump_l
.loadi      $0, $C
.dump_l
.add_t      $0, $1
.dump_l
.dump_i     $1
.dump_i     $2
.swipe      $1, $2
.label      $C
.push_t     "H", 0
.push_t     "E", 0
.push_t     "L", 0
.push_t     "L", 0
.push_t     "O", 0
.push_t     " ", 0
.push_t     "W", 0
.push_t     "O", 0
.push_t     "R", 0
.push_t     "D", 0
.push_t     "!", 0
.push_t     "\n", 0
.pop_x      $0
.jump       $C
.halt