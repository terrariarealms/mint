#!/bin/bash

# Compiles Mint.Core from src/ and runs.

dotnet build src/ -v m -c Release --os linux -o bin/
bin/net7.0/linux-x64/Mint.Core
