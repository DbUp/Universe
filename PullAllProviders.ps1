$allProviders = Get-Content -Path AllProviders.txt

pushd

cd ../DbUp
git pull

foreach ($provider in $allProviders) {
     Write-Host "Pulling $provider"
     cd ../$provider
     git pull
}

popd