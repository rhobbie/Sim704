# Sim704

Sim704 is a simulator for the IBM 704 Computer
 
It can run the original Fortran II compiler from 1958 and also the SHARE assembler program (UASAP).

It is written in C# and runs on Windows (and with Mono also under Linux). 

Some Parts are based on the SimH IBM 704 implementation.

See the Mkf2 repository for prebuild binaries and a set of windows batchfiles to create the UASAP and Fortran tapes and to compile and run Fortran programs on the simulator.

The configuration of Sim704 is done with an xml file that has to be given as command line parameter.
The tapes, drums, printer, card reader, card punch, sense lights and sense switches are simulated.
Tape and card files are in SimH p7b format and can also be used on Simh when using this option. 
(For xml-examples see the Mkf2 repository)

...
