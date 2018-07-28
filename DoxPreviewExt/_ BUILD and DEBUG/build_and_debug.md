# Build

When building the project, it has to be ensured that the current verison of the manifest is greater than the installed version of this VSIX extension.
The version can beset in the "source.extensions.vsixmanifest"


# DEBUG

Visual Studio Extensions can be debugged like any other application. You just need to setup the debug experience to launch devenv with the loaded extension. 

For debuging go to the "Debuggen" property pag.

- Choose "Externes Program starten" in the e and set the target to the visual studio executable.
   
	 e.g. "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\devenv.exe"

- Add to edite filed for "Befehlszeilenargumente": `/rootsuffix Exp`

- Start the debugging by "F5" or respectively the debug button in the menu.


See also:

- [Stackoverflow: How to debug Visual Studio extensions](http://stackoverflow.com/questions/9281662/how-to-debug-visual-studio-extensions)
- [BI+: Debugging the VSIX extension](https://bideveloperextensions.github.io/features/VSIXextensionmodel/)
- [WesHackett: Configure VSIX project to enable debugging](http://weshackett.com/2009/11/configure-vsix-project-to-enable-debugging/)
- [Stackoverflow: How to debug a Vsix project](https://stackoverflow.com/questions/24653486/how-to-debug-a-vsix-project)


It may be necessary to clear the data and cache from a previous debug session, which are located in the directory
"C:\Users\<USERNAME>\AppData\Local\Microsoft\VisualStudio", in anfolder names somehow like "Exp" and "15.0_f7c1d9f6Exp".<br/>
Possibly that was caused by a bug in earlier versions and thus be obsolete now.
