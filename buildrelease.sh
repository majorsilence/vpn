#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

CURRENTPATH=$(pwd)

dotnet restore ./WebSite/VpnSite.sln

rm -rf build/
dotnet publish "$CURRENTPATH/Website/Majorsilence.Vpn.Site" -p:Configuration="Release" -o "$CURRENTPATH/build/publish/VpnSite"
cd build/publish

# finally package the zip for transport to the live server or jenkins artifact
7za a -t7z "$CURRENTPATH/build/VpnSite.7z" VpnSite -bd
cd "$CURRENTPATH"