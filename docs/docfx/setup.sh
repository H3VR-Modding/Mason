#!/bin/bash
TMP=$(mktemp)
DOCFX_URL="https://github.com/dotnet/docfx/releases/download/v2.58/docfx.zip"

rm -rf bin
wget -O "$TMP" "$DOCFX_URL"
unzip "$TMP" -d bin
rm "$TMP"
