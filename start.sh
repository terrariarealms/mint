#!/bin/bash

# Compiles Mint.Core from src/ and runs.

dotnet build src/ -v m -c Release --os linux -o bin/
bin/Mint.Core
