# CommandLineTool

Commands
```
   help
```
a command to see what the tool does
USAGE:
```
 CmdTool.exe help <flag>
```
<flag> is optional. flag -h will show how to use the 'help' command

EXAMPLE:
CmdTool.exe help    will show the help as to how to use all the commands
CmdTool.exe help -h    will show the help of the 'help' command
     
```
uninstall
```
 
uninstall is a command used for uninstalling softwares

USAGE:
```
CmdTool.exe uninstall <flags> [software names]
```
[software names]: the full names of the softwares to be uninstalled.
                Multiword names should be in quotes (see the example below)
                 Only single wildcard (*) is supported in the names. Wildcard can be at start, end or in the middle
<flags>: -s (single) -m  (multiple) -silent (for silently uninstalling)

EXAMPLE:
CmdTool.exe uninstall -s Zoom    This will uninstall Zoom
CmdTool.exe uninstall -s -silent "Python Laucher"    This will uninstall 'Python Laucher' silently
CmdTool.exe uninstall -m "Notepad++*" Zoom    This will uninstall Notepad++ and Zoom


In order to extend the application (add another command)

1. Implement the abstract class `Command` Say `MyNewCommand`
2. Implement `ICommandHandler<>` Command Say `MyNewCommandHandler<MyNewCommand>

That is all needed.
