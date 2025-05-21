#!/bin/bash
echo "Building Kart Server..."
dotnet build
if [ $? -eq 0 ]; then
    echo "Build successful!"
    echo "To run the server use: dotnet run"
    echo "To run with custom port: dotnet run -- <port>"
else
    echo "Build failed! See errors above."
fi