#!/bin/bash
final=$1
if [[ -z "$final" ]]; then
	echo Final destination required
	exit 1
fi

tmp=$(mktemp -d)
patchers=$tmp/patchers
dest=$tmp/dest.zip

mkdir -p "$patchers"

cp README.md Mason.Patcher/manifest.json "$tmp/"
cp media/icon/256.png "$tmp/icon.png"
cp Mason.Patcher/bin/Release/net35/{Mason.Core,Mason.Patcher,Newtonsoft.Json}.dll "$patchers/"
cp libs/YamlDotNet_UnitySubset35.dll "$patchers/YamlDotNet.dll"

pushd "$tmp"
zip -9r "$dest" .
popd

mv "$dest" "$final"

rm -rf "$tmp"
