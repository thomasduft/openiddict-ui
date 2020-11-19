#!/bin/bash

if [ -z "$1" ];
then
  echo No version specified! Please specify a valid version like 1.2.3!
  exit 1
fi

if [ -z "$2" ];
then
  echo No release info specified! For a relase provide the r flag and a valid release verion!
  exit 1
fi

echo ----
echo Starting building version $1

echo ----
echo Cleaning up
rm -r ./dist

echo ----
echo Restore solution
dotnet restore openiddict-ui.sln

echo ----
if [ $2 = "r" ];
then
  echo Packaging solution with Version = $1
  dotnet pack src/OpenIddict.UI.Core -c Release -p:PackageVersion=$1 -p:Version=$1 -o ./dist/nupkgs/
  dotnet pack src/OpenIddict.UI.Infrastructure -c Release -p:PackageVersion=$1 -p:Version=$1 -o ./dist/nupkgs/
  dotnet pack src/OpenIddict.UI.Api -c Release -p:PackageVersion=$1 -p:Version=$1 -o ./dist/nupkgs/
else
  echo Packaging solution with PackageVersion = $1
  dotnet pack src/OpenIddict.UI.Core -c Release -p:PackageVersion=$1 -o ./dist/nupkgs/
  dotnet pack src/OpenIddict.UI.Infrastructure -c Release -p:PackageVersion=$1 -o ./dist/nupkgs/
  dotnet pack src/OpenIddict.UI.Api -c Release -p:PackageVersion=$1 -o ./dist/nupkgs/
fi

if [ -z "$3" ];
then
  echo ----
  echo Done
  exit 0
fi

echo ----
echo Pushing packages
for package in $(find ./dist/nupkgs/ -name *.nupkg); do
  dotnet nuget push $package -k $3 -s https://api.nuget.org/v3/index.json
done

echo ----
echo Creating and pushing tag 
git tag -a v$1 -m '"version '$1'"'
git push origin v$1

echo ----
echo Done
