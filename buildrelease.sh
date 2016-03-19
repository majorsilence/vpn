#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

nuget restore ./WebSite/VpnSite.sln -NonInteractive


if [ "$(expr substr $(uname -s) 1 10)" == "MINGW32_NT" ]; then
	"C:\Program Files (x86)\Mono\lib\mono\xbuild\12.0\bin\xbuild.exe" ./WebSite/VpnSite.sln /toolsversion:4.0 /t:Clean /t:rebuild /p:Configuration="Release";Platform="Any CPU"       
else
    xbuild ./WebSite/VpnSite.sln /toolsversion:4.0 /t:Clean /t:rebuild /p:Configuration="Release";Platform="Any CPU"
fi


# visual studio 2012 deployment style build
# ;DeployOnBuild=true;PublishProfile=\"Zip File\" /t:publish;clean;rebuild


# Web deployments relative paths do not work and publish profiles are not implemented.  Hack around this.
mkdir -p published-site
cd ./published-site
rm -rf *
cd ..
# See http://askubuntu.com/questions/333640/cp-command-to-exclude-certain-files-from-being-copied

if [ "$(expr substr $(uname -s) 1 10)" == "MINGW32_NT" ]; then
	XCOPY ".\WebSite\VpnSite\*.*" ".\published-site" /E /I /Y /EXCLUDE:c:\releaseexcludelist.txt    
else
	rsync -av --exclude="*.csproj" --exclude="*.mdb" --exclude="*.cs" --exclude="*VpnSite/Properties" --exclude="*VpnSite/obj" "./WebSite/VpnSite" "./published-site"
fi

cd ./published-site

# Set live settings for web.config
cd ./VpnSite
sed -i "s/    <add key=\"MySqlInstance\".*/    <add key=\"MySqlInstance\" value=\"localhost\" \/>/g" web.config 
sed -i "s/    <add name=\"LocalMySqlServer\".*/    <add name=\"LocalMySqlServer\" connectionString=\"server=localhost;user=root;pwd=Password123;database=VpnSessions;\" providerName=\"MySql.Data.MySqlClient\" \/>/g" web.config 
cd ..

# finally package the zip for transport to the live server or jenkins artifact
zip -r "VpnSite.zip" "VpnSite"
cd ..