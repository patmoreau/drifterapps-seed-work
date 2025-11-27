# Local NuGet Package Testing Guide

Your project already has a `build.sh` script set up for creating local NuGet packages! Here's how to use it:

## Quick Start

### 1. Build and Pack Locally

Run the existing build script:

```bash
./build.sh
```

This script will:

- Increment a local version number (stored in `version.txt`)
- Build the solution in Debug configuration
- Run tests
- Create NuGet packages with version `1.0.{counter}-alpha`
- Output packages to `./nuget-packages/`
- Push packages to your local NuGet source at `/Users/patrickmoreau/.nuget/local-packages/`

### 2. Reference in Another Project

In your **test project**, add a `nuget.config` file (if it doesn't already exist) to reference the local package source:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="/Users/patrickmoreau/.nuget/local-packages/" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
```

### 3. Install the Local Package

In your test project, install the package:

```bash
dotnet add package DrifterApps.Seeds.Application --version 1.0.X-alpha
# Replace X with the current version number from version.txt
```

Or add directly to your `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="DrifterApps.Seeds.Application" Version="1.0.X-alpha" />
</ItemGroup>
```

## Alternative: Manual Process

If you prefer more control or want to pack without running tests:

### Option 1: Pack to specific output folder

```bash
# Pack all projects
dotnet pack --configuration Debug --output ./local-packages

# Pack specific project
dotnet pack src/Application/Application.csproj --configuration Debug --output ./local-packages
```

### Option 2: Use a specific version

```bash
dotnet pack -p:Version=1.0.999-local --configuration Debug --output ./local-packages
```

### Option 3: Reference without pushing to local source

In your test project's `nuget.config`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local-dev" value="/path/to/drifterapps-seed-work/nuget-packages" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
```

## Tips

1. **Clear NuGet cache** if you're updating packages with the same version:
   ```bash
   dotnet nuget locals all --clear
   ```

2. **Check current version**: Look at the `version.txt` file to see the current counter

3. **View packages in local source**:
   ```bash
   ls -la /Users/patrickmoreau/.nuget/local-packages/
   ```

4. **Restore packages** in your test project:
   ```bash
   dotnet restore --force
   ```

## Package Structure

Your solution creates these NuGet packages:

- `DrifterApps.Seeds.Application`
- `DrifterApps.Seeds.Application.Mediatr`
- `DrifterApps.Seeds.Domain`
- `DrifterApps.Seeds.Infrastructure`
- `DrifterApps.Seeds.Testing`

Each gets the same version number from the build script.

