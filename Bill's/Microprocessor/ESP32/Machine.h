#pragma once



#ifndef MACHINE_FILENAME


#    include "Machines/Chessato.h"

#else

#    define MACHINE_PATHNAME_QUOTED(name) <src/Machines/name>

#    include MACHINE_PATHNAME_QUOTED(MACHINE_FILENAME)

#endif  
