#!/bin/bash

# Compiles Mint.Core from src/ and runs.

dotnet build src/ -v m
bin/net6.0/Mint.Core
