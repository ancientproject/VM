New-Item ./build -ItemType Directory
New-Item ./build/ui -ItemType Directory
cd .\vm\csharp
dotnet publish -r win10-x64
cd ..
cd ..
cd .\acc
dotnet publish -r win10-x64
cd ..
cd .\Rune
dotnet publish -r win10-x64
cd ..
cd .\ui
dotnet publish -r win10-x64
cd ..
Get-ChildItem .\ui\bin\Debug\netcoreapp3.0\win10-x64\publish | Copy -Destination .\build\ui -Recurse