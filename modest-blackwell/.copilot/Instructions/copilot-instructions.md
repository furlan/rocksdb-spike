---
applyTo: "**"
---
# Copilot Agent 

This file provides guidance to Copilot Agent when working with code in this repository.

## Overview

Modest Blackwell is an API, providing access to the automation equipments in a house. The API will use GraphQL language so the user can explore what data is available and only retrieve specific data. At this point, all the data will be available to be queried.

## Architecture Overview

### Technology Stack

- **Backend**: .Net Core 9.0 using C#
- **Data Storage**: 
    - YAML file with assets meta information
    - RocksDB using C# wrapper RocksDbSharp. 
- Use Swagger for manual API testing

## Project Structure Overview

- Documentation files in root directory

## Data Storage for Utilization Data

This application uses two different data sources. YAML file is used to store assets meta information, and RocksDB is used to store the utilization, which is the series of data from the sensors and equipments. More details will be provided below.

# Requirements

Create the application in two phases. At this point only work on the phase 1.

## Phase 1 - Web API for Assets Meta Information

✅ **Phase 1 Complete - Web API for Assets Meta Information**

Create a Web App API in Core .NET using C# that provide GET operation for the YAML file. That YAML file contains the assets meta information. Consider the follow data description:

Consider the class diagram in markdown mermaid file: [assets-class-diagram.md](./assets-class-diagram.md)

You need to write the proper service in the .NET Core. The service will retrieve the data from the YAML files. Also need to write the API to retrieve the data using GET verb only.

### Asset

Description: Defines the attributes of an asset.
Type: YAML 
File location: <project folder>/data/yaml/assets.yaml

### Stream 

Description: Defines the attributes of stream. An stream is a measure of utilization of an asset. So, every stream must belongs to an asset. This is just the definition, the values of the measure is in the RocksDB. 
Type: YAML 
File location: <project folder>/data/yaml/streams.yaml

## Phase 1a - Add operational type to the Asset

✅ **Phase 1a Complete - Add operational type to the Asset**

In order to organize RocksDB data, add the field type to the model Asset. It's a string value. Change the web API propertly.

## Phase 2 - GraphQL API for the Operational data

✅ **Phase 2 - GraphQL API for the Operational data**

In the phase 2, implement the GraphQL API to access the operational data in RocksDB. It will work like a wrapper around the RocksDbSharp implementation. There are three types of operational data and each one has their own Column Families at RocksDB: notification, utilization, and alarm.

RocksDB data file location: ./data/rocksdb/
RocksDB database name: operational

### GraphQL Definition

The GraphQL should be able to answer the following operation and response:

**Operation:**

```operation
{
    asset
    {
        id
        name
        location
        type
        class
        type {
            name
            streams {
                id
                name
                asset_id
                uom
                values {
                    key
                    value
                }
            }
        }
    }
}
```

**Example of Response:**

```json
{
    "data": {
        "asset": {
            "id": "NT01",
            "name": "Nest living room",
            "location": "Living room",
            "type": "thermostat",
            "class": "automation"
        },
        "type": {
            "name": "utilization",
            "streams": [
                {
                    "id": "NT01.T02",
                    "name": "Nest environment temperature",
                    "assetId": "NT01",
                    "uom": "F",
                    "values": [ 
                        {
                            "key": "NT01T0220250725T103258Z", 
                            "value": 75
                        },
                        {
                            "key": "NT01T0220250725T112658Z",
                            "value": 74
                        },
                        {
                            "key": "NT01T0220250725T103258Z",
                            "value": 73
                        },
                        {
                            "key": "NT01T0220250725T112658Z",
                            "value": 74
                        },
                    ]
                },
                ...
            ]
        } 
    }
}
```

### Response data source

The type "asset" came from Asset YAML file, filtered by the fields. For one Asset, there are zero or more Streams, that also came from the YAML file and filtered by the fields. The operational data is stored in the RocksDB. Every operational type has your onw columan family at the RocksDB. Include the column family when it's getting the value using RocksDBSharp.

Retrieving the information from RocksDB, use the prefix iterator. The prefix for an specific stream is the concatenation of asset ID and stream ID. For example, the prefix for asset ID "NT01" and stream ID "T02" is "NT01T02".

## Phase 2a - Add filter by location to operational data GraphQL API

✅ **Phase 2a - Add filter by location to operational data GraphQL API

Now it is required to add filters by asset location and operational data type to the GraphQL query.

```
{
    asset(location: "Living room")  {
       type(name: "notification") {
        name
        streams {
            id
            name
            uom
            assetId
            values {
                key
                value
            }
        }
       }
    }
}
```

Update README.md file with new filters documentation.

# Project general coding standards

## Naming Conventions
- Project name should be ModestBlackwell
- Use camelCase for variables, functions, and methods
- Prefix private class members with underscore (_)
- Use ALL_CAPS for constants

## Design Principles and Best Practices

- **SOLID Principles**
  1. Single Responsibility Principle
  2. Open/Closed Principle  
  3. Liskov Substitution Principle  
  4. Interface Segregation Principle  
  5. Dependency Inversion Principle

- **DRY (Don't Repeat Yourself):**
Avoid code duplication by abstracting common functionality into reusable functions, classes, interfaces, and factories.

## Testing and Quality Assurance

- **Testability:**  
  Structure code to facilitate unit, integration, and end-to-end testing.

## Code Structure
- Follow the standard code structure for .NET C#.

## Development Considerations
- When using the Terminal to test anything, always redirect stdout and stderr to a file and then read the file. Delete the file after reading it. This ensures that you can see the output of your commands, as there is currently a bug in VSCode. Please cat the file so I can see the output as well.
- Use environment variables for configuration (e.g., database credentials, API keys)
- Ensure that sensitive information is not hardcoded in the codebase
- When creating new files, consider whether there should be any changes to the `.gitignore` file, especially for logs, temporary files, or build artifacts
- As you write code, make sure that you update the [README.md](../../README.md), updating any application server commands, description, dependencies, and installation instructions. This file serves as the main documentation for the project and should always reflect the current state of the application.

## Development validation
- When testing features, if you need to run the development server, check if it's already running before starting it again
- Use full paths when running commands instead of relative paths to avoid working in the wrong directory
- For UI testing, please do not test yourself, but instead ask me to test it. Provide details on what you'd like me to test, including specific actions to take and expected outcomes. I will then run the tests and provide feedback.
- All files that you generate for testing should go into the system temp directory, which is `/tmp` on Linux. This ensures that temporary files do not clutter the project directory.

## Error Handling
- Use try/catch blocks for async operations
- Always log errors with contextual information