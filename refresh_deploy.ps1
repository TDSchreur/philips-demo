./helm_uninstall.ps1
docker build -t devtds.azurecr.io/philips-api .
docker push devtds.azurecr.io/philips-api
./helm_install.ps1