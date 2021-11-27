#!/bin/sh

mkdir -p Publish
dotnet publish KachnaOnline.App -c Release -o Publish/ --self-contained false -r linux-x64
