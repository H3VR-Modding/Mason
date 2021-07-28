#!/bin/bash
LATEST_UNITY_SUBSET_35=https://ci.appveyor.com/api/buildjobs/yaovugrbkveb7dry/artifacts/YamlDotNet%2Fbin%2FRelease%2FRelease-UnitySubset-v35.zip

final=$1
if [[ -z "$final" ]]; then
	echo Final destination required
	exit 1
fi

tmp=$(mktemp -d)
patchers=$tmp/patchers
dest=$tmp/dest.zip
yaml_zip=$(mktemp)

mkdir -p "$patchers"

curl -o "$yaml_zip" -L "$LATEST_UNITY_SUBSET_35"
unzip "$yaml_zip" "$patchers/YamlDotNet.dll"

rm -rf "$yaml_zip"

cp README.md Mason.Patcher/manifest.json "$tmp/"
cp media/icon/256.png "$tmp/icon.png"
cp Mason.Patcher/bin/Release/net35/{Mason.Core,Mason.Patcher,Newtonsoft.Json}.dll "$patchers/"

pushd "$tmp"
zip -9r "$dest" .
popd

mv "$dest" "$final"

rm -rf "$tmp"
