#!/bin/sh

rm -f out.zip
find . | egrep -v "node_modules/|obj/|bin/|.git/|dist/|UploadedImages/|Publish/" | egrep "\.(html|htm|css|js|ts|cs|csproj|md|sh|sln|json|png|jpg|pdf|gif)$" | zip -@ out.zip 
