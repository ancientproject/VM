New-Item ./build -ItemType Directory
New-Item ./build/ui -ItemType Directory
cd .\vm\csharp
dotnet publish -r win10-x64 /p:PublishSingleFile=true
cd ..
cd ..
Get-ChildItem .\vm\csharp\bin\Debug\netcoreapp3.0\win10-x64\publish | Copy -Destination .\build -Recurse
cd .\acc
dotnet publish -r win10-x64 /p:PublishSingleFile=true
cd ..
Get-ChildItem .\acc\bin\Debug\netcoreapp3.0\win10-x64\publish | Copy -Destination .\build -Recurse
cd .\ui
dotnet publish -r win10-x64
cd ..
Get-ChildItem .\ui\bin\Debug\netcoreapp3.0\win10-x64\publish | Copy -Destination .\build\ui -Recurse