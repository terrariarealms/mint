# [README на русском](https://github.com/terrariarealms/mint/blob/main/README_russian.md)

Mint is a modern, powerful core for your Terraria server, providing many improvements.

## For developers
A package is required for development .NET 7.0 SDK, which can be found on [Microsoft's official website](https://dotnet.microsoft.com/en-us/download/dotnet/7.0 ).

It can also be installed via `sudo apt-get install dotnet-sdk-7.0` on many Linux distributions.
But don't forget to update the package repositories via `sudo apt-get update && sudo apt-get upgrade`.

In order to change the kernel for yourself, you must first install the server (instructions below).

### Core Development
The kernel source code is always located in the `src/` folder, and the compiled server is in the `bin/net7.0/` folder

### Development of modules for the kernel
Modules are loaded from the `modules/` folder (at least if your hands haven't gotten around to changing the modules folder in the kernel).
Modules can be loaded:
* From already compiled binaries, for example `modules/MyModule.dll`
* From projects, for example `modules/MyModule/src/MyModule.csproj`.

In order to create a module project, it is necessary:
1. Create a folder with the name of your module (the folder name must match the output name of the binary)
2. Create a project via `dotnet new classlib -f net7.0 -o MyModule/src -n MY_MODULE`, where `MY_MODULE` will be the name of your module.

In order for the server to load your module, it must inherit the class [`Mint.Modules.MintModule`](https://github.com/terrariarealms/mint/blob/main/src/Modules/MintModule.cs)
Be sure to specify dependencies on other modules in the `ModuleReferences` property, and also update the `ModuleArchitecture` property every time if you have made an update to your module that may affect collaboration with other modules. (changing/deleting/renaming public methods/classes/fields/properties and the like)

The `Setup` method should contain the code that initializes the module before starting work (it should not contain interaction with other modules)
The `Initialize` method should contain the code that starts all the work, and can interact with other modules.

## Installation
1. Create a directory where the server will be located.
2. Open the command prompt/terminal in this folder.
3. Clone the repository via `git clone https://github.com/terrariarealms/mint.git`

## Starting the server
1. Open the command prompt/terminal in your folder.
2. Run `./start.sh``

If you are on Linux, then you can start the server through an already created script: `./start.sh `.
* If you are denied access, enter `chmod +x start.sh ` and run the script again.
