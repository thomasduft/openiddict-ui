#!/bin/bash

if [ -z "$1" ]
then
  echo No version specified! Please specify a valid version like 1.2.3!
  exit 1
else
  echo version $1
fi

echo Cleaning up
rm -r ./dist

echo Restore solution
dotnet restore openiddict-ui.sln

version = $1

if [ -n "$2" && $2 = "r" ]
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

if [ -z "$3" ]
then
  echo Done
  exit 0
fi

for package in $(find ./dist/nupkgs/ -name *.nupkg); do
  echo Pushing $package
  dotnet nuget push $package -k $3 -s https://api.nuget.org/v3/index.json
done

echo Done
