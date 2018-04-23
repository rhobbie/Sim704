# Sim704

Sim704 is a simulator for the IBM 704 Computer
 
It can run the original Fortran II compiler from 1958 and also the SHARE assembler program (UASAP).

It is written in C# and runs on Windows (and with Mono also under Linux). 

Some Parts are based on the SimH IBM 704 implementation.

See the Mkf2 repository for prebuild binaries and a set of windows batchfiles to create the UASAP and Fortran tapes and to compile and run Fortran programs on the simulator.

The configuration of Sim704 is done with an xml file that has to be given as command line parameter.
The tapes, drums, printer, card reader, card punch, sense lights and sense switches are simulated.
Tape and card files are in SimH p7b format and can also be used on Simh when using this option. 

The tool704 repository contains a set of tools for handling tape and cardfiles.

For documentation links see below.


Configuration options in the xml file. (For examples see the Mkf2 repository) 

```xml
 <MT>
    <string>Mt1.tap</string>
    <string>Mt2.tap</string>
    ...
 </MT>   
```

Sets the path for the tape-files. The first entry is for tape 1 and so on. Up to 10 tapes are possible. Non-existent files are created when the 704 SW accesses the tape.

```xml
  <DR>
    <string>Drm1.drm</string>
    <string>Drm2.drm</string>
    ...
  </DR>
```

Sets the path for the drum-files. Each logical drum is stored into one file. The first entry is for drum 1 and so on. Up to 8 logical drums are possible. Non-existent files are created when the 704 SW accesses the drum.
The content of the drums are cached in memory and written out when the simulator exits.

```xml
<CRD>SourceDeck.cbn</CRD>  
```

Sets the path for input to the card reader. 

```xml
<CPU>Punched.cbn</CPU>
```

Sets the path for output of the card punch. 

The cards are stored in a tape file with one binary record per card in column binary format. 

```xml
<LP>LP.txt</LP>
```

Sets the path for output of the printer. The file has ASCII format. 
When no LP config is given, the printer output is redirected to the console.

```xml
  <Switch>
    <boolean>false</boolean>
    <boolean>true</boolean>
    <boolean>true</boolean>
    <boolean>false</boolean>
    <boolean>true</boolean>
    <boolean>false</boolean>
  </Switch>
```

Configures the switches. First entry is for switch 1 and so on. false means "switch up" and true means "switch down". When no Switch config is given all switches are set to false.

```xml 
 <MemSize>4</MemSize>
```

Configures the Memory size of the simulated IBM 704 in kWords. Only 4 8 and 32 is a valid option.

```xml
<boot>MT</boot>. 
```

MT means the Simulator boots from tape 1, CRD means boot from card reader and DR means boot from drum 1. When no boot option is given the simulator boots from the card reader.

```xml
<LogCPU>logfile.txt</LogCPU>
```

 Whenn adding this option a logfile of all executed instructions is created. The file is similar to SimH CPU History format.

```xml 
 <LogIO>logIO.txt</LogIO>
```

When adding this option a logfile of every IO-operation is created.
  
When using the same filename for both entries then both logs are merged into one file. 

```xml 
 <ExitAtHalt>1</ExitAtHalt>
```

When adding this option the simulator automatically exits at the first halt. With value 2 it automatically continues at the first halt and exits at the second halt. 
 
When no ExitAtHalt option is given then the simulator halts to a (very simple) console.
When hitting return at the console the simulator continues. 
A x and return exits the simulator. A go followed by an octal value and return continues the simulator at the given address.
 
When the simulator exits then "finished." is printed to the console. This means that all cached data is written and all open files are closed.


Documentation links for the 704, UASAP and Fortran:

IBM 704 Manual of Operation 
       http://bitsavers.org/pdf/ibm/704/24-6661-2_704_Manual_1955.pdf
       
CODING for the MIT-IBM 704 COMPUTER      
      http://bitsavers.org/pdf/mit/computer_center/Coding_for_the_MIT-IBM_704_Computer_Oct57.pdf

SHARE Reference Manual for the IBM 704 
           http://www.piercefuller.com/library/share59.html

IBM MODEL-704 GUIDEBOOK 
         https://pubarchive.lbl.gov/islandora/object/ir:148891

Fortran Reference Manual 
          http://bitsavers.org/pdf/ibm/704/704_FortranProgRefMan_Oct56.pdf
          
Fortran Programmer's Primer 
           http://bitsavers.org/pdf/ibm/704/F28-6019_704_FORTRAN_primer.pdf

Fortran II Reference Manual 
           http://bitsavers.org/pdf/ibm/704/C28-6000-2_704_FORTRANII.pdf

Fortran II Operations Manual 
           http://bitsavers.org/pdf/ibm/704/704_FORTRAN_II_OperMan.pdf

History of FORTRAN and FORTRAN II 
           http://www.softwarepreservation.org/projects/FORTRAN/

IBM 704 SW archive 
           http://sky-visions.com/ibm/704/

IBM SHARE tape Library 
           http://www.piercefuller.com/library/share.html
