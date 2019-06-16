New-Item ./build -ItemType Directory
New-Item ./build/ui -ItemType Directory
cd .\vm
dotnet publish -r win10-x64 /p:PublishSingleFile=true
cd ..
Get-ChildItem .\vm\bin\Debug\netcoreapp3.0\win10-x64\publish | Copy -Destination .\build -Recurse
cd .\acc
dotnet publish -r win10-x64 /p:PublishSingleFile=true
cd ..
Get-ChildItem .\acc\bin\Debug\netcoreapp3.0\win10-x64\publish | Copy -Destination .\build -Recurse
cd .\ui\CPU_Host
dotnet publish -r win10-x64
cd ..
cd ..
Get-ChildItem .\ui\CPU_Host\bin\Debug\netcoreapp3.0\win10-x64\publish | Copy -Destination .\build\ui -Recurse