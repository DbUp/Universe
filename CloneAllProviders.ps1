$allProviders = Get-Content -Path AllProviders.txt

pushd
cd ..

git clone "https://github.com/DbUp/DbUp"

foreach ($provider in $allProviders) {
   if(Test-Path $provider)
   {
        Write-Host "$provider is already cloned"
   }
   else
   {
        Write-Host "Cloning $provider"
        git clone "https://github.com/DbUp/$provider"
   }
}

popd