#prepare server for installation of packages. Necessary for new environments
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update

#install environment dependencies for dotnet 
sudo apt-get install -y dotnet-sdk-5.0
dotnet new tool-manifest
dotnet tool install dotnet-ef --version 7.0.0
 
#function to add migration 
nowInMs() {
  echo "$(($(date +'%s * 1000 + %-N / 1000000')))"
}
export "$(grep -vE "^(#.*|\s*)$" .env)"
#run and apply migration
cd src/services/Gamawabs247API
dotnet ef migrations add "gamasoft$(nowInMs)" --project "../../Libraries/Infrstructure.Data"
dotnet ef migrations bundle --self-contained --connection $DATABASE_URL --force
./efbundle 

