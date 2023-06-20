#!/bin/bash

# Set the path to the file that will store the local version value
file_path="version.txt"
output_nuget_path="./nuget-packages"
local_nuget_path="/Users/patrickmoreau/.nuget/local-packages/"

# Check if the file exists
if [ -f "$file_path" ]; then
  # Read the current value from the file
  counter=$(cat "$file_path")
else
  # If the file doesn't exist, initialize the counter to 0
  counter=0
fi

# Increment the counter
counter=$((counter + 1))

# Output the current version
echo "Current version: 1.0.$counter-alpha"

# Save the updated version back to the file
echo "$counter" > "$file_path"

# Use the rm command to delete all files in the folder
rm -rf "$output_nuget_path"/*

dotnet build --configuration Debug

dotnet test --configuration Debug

dotnet pack "/property:Version=1.0.$counter-alpha" --force --configuration Debug --include-source --include-symbols --output "$output_nuget_path"

dotnet nuget push "$output_nuget_path"/*.nupkg --skip-duplicate --source "$local_nuget_path"
