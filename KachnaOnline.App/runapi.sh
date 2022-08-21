#!/bin/sh
# Ensure that static files directory exists (otherwise dotnet throws an error)
if [ ! -d wwwroot ]; then
    mkdir -p ../ClientApp/KachnaOnline/dist/KachnaOnline
fi
dotnet run $@
