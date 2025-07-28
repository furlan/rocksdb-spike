# RocksDB Spike

This repository contains experiments and examples for working with RocksDB in C#.

## Projects

### ModestBlackwell API

A .NET Core 9.0 Web API that provides access to automation equipment data in a house. The API uses YAML files for metadata and will eventually use RocksDB for time-series data storage.

#### Features

- **Assets Management**: CRUD operations for automation assets (thermostats, lights, etc.)
- **Data Streams Management**: Access to sensor and equipment measurement definitions
- **RESTful API**: Clean REST endpoints with comprehensive documentation
- **Swagger Integration**: Interactive API documentation available at the root URL

#### Technology Stack

- **.NET Core 9.0** - Web API framework
- **YamlDotNet** - YAML file parsing
- **RocksDbSharp** - RocksDB C# wrapper (for future time-series data)
- **Swagger/OpenAPI** - API documentation

#### Getting Started

1. **Prerequisites**:
   ```bash
   # Install libsnappy (required for RocksDB on Linux)
   sudo apt update && sudo apt install -y libsnappy-dev
   ```

2. **Build and Run**:
   ```bash
   cd modest-blackwell
   dotnet build
   dotnet run
   ```

3. **Access the API**:
   - API Base URL: `http://localhost:8080`
   - Swagger Documentation: `http://localhost:8080` (root URL)

#### API Endpoints

**Assets**:
- `GET /api/assets` - Get all assets
- `GET /api/assets/{id}` - Get asset by ID

**Data Streams**:
- `GET /api/streams` - Get all data streams
- `GET /api/streams/{id}` - Get data stream by ID
- `GET /api/streams/by-asset/{assetId}` - Get all streams for a specific asset

#### Data Structure

The API reads from YAML files located in `data/yaml/`:

- **assets.yaml**: Contains asset metadata (thermostats, lights, etc.)
- **streams.yaml**: Contains data stream definitions (temperature readings, status indicators, etc.)

#### Example Usage

```bash
# Get all assets
curl http://localhost:8080/api/assets

# Get a specific asset
curl http://localhost:8080/api/assets/NT01

# Get all data streams for an asset
curl http://localhost:8080/api/streams/by-asset/NT01
```

### Examples

- **ColumnFamily**: RocksDB column family example
- **FamilyTree**: Person/family relationship example using RocksDB

## Development Environment

This project uses a dev container with:
- .NET Core 9.0 SDK
- Python support
- libsnappy-dev (for RocksDB compression)
- C# dev tools and extensions

## Architecture

The ModestBlackwell API is designed in two phases:

1. **Phase 1** (Current): Web API for assets metadata using YAML files
2. **Phase 2** (Future): Integration with RocksDB for time-series utilization data storage

The application follows SOLID principles and clean architecture patterns with:
- Service layer for business logic
- Interface-based dependency injection
- Comprehensive error handling and logging
- Structured configuration management
