#!/bin/bash

# Compiles Mint.Core from src/ and runs.

dotnet build src/ -v m -c "Release RUSSIAN" --os linux -o bin/net7.0/linux-x64
bin/net7.0/linux-x64/Mint.Core
