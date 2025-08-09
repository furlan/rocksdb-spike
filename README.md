# RocksDB Spike

This repository contains experiments and examples of working with RocksDB in C#.

## Examples

There are some ad-hoc examples, exploring different aspects of the RocksDB usage.

- FamilyTree is a simple program showing Put/Get/Iterator methods.
    - For the same example in Python, check Jupyter [notebook](./simple_family_tree.ipynb).
- ColumnFamily is to explore how to use column families functionality.
- PrefixIterator is to show how to use prefix iterators.
- SKChat is a very simple chat using Semantic Kernel with plugins.

## Projects

Projects as more extensive example of usage of RocksDB simulating real application. Here is the list of projects and the respective details:

- Modest Blackwell API
- Wise Blackwell Console
- Edifice Blackwell Console

### Modest Blackwell API

A .NET Core 9.0 Web API that provides access to automation equipment data in a house. The API uses YAML files for metadata and uses RocksDB for time-series data storage. The project was generated using Copilot Agent, with some manual corrections. You can check the [Copilot instructions here](./modest-blackwell/.copilot/Instructions/copilot-instructions.md).

#### Features

- **Assets Management**: GET operation for automation assets (thermostats, lights, etc.)
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
   - GraphQL Console: `http://localhost:8080/graphql`

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

[Mermaid diagram showing the relationship between Assets and Streams](./modest-blackwell/.copilot/Instructions/assets-class-diagram.md)

#### Example Usage

```bash
# Get all assets
curl -s http://localhost:8080/api/assets | jq .

# Get a specific asset
curl -s http://localhost:8080/api/assets/NT01 | jq .

# Get all data streams
curl -s http://localhost:8080/api/streams | jq .
```

#### GraphQL queries

The API supports GraphQL queries with filtering capabilities for both asset location and operational data type.

**Basic Query:**

```GraphQL
{
    asset(id: "NT01")  {
       type {
        name
        streams {
            id
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

**Filter by Asset Location:**

```GraphQL
{
    asset(location: "Living room")  {
       type {
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

**Filter by Operational Data Type:**

```GraphQL
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

**Available Operational Data Types:**
- `utilization` - Sensor readings and equipment measurements
- `notification` - System notifications and operational notes  
- `alarm` - Equipment alarms and error conditions

**Query Multiple Assets by Location:**

```GraphQL
{
    assets(location: "Living room")  {
       asset {
         id
         name
         location
       }
       type {
        name
        streams {
            id
            name
            values {
                key
                value
            }
        }
       }
    }
}
```

![GraphQL console example](./graphql-console-example.png)

#### Operational Data

The intent of the program `OperationalData` is to seed the RocksDB with operational data. Use the [Excel spreadsheet](./modest-blackwell/OperationalData/load/Operational_poc_data.xlsx) to help generating the lines and paste on [load file](./modest-blackwell/OperationalData/load/alarm-data-load.txt).

```bash
dotnet run [--seed]
```

Use 'seed', '--seed', or '-s' argument to seed sample data.

### Wise Blackwell Console

It is a .NET 9.0 Console program that uses Semantic Kernel to orchestrate LLM calls to answer questions in natural language about Assets, Streams, and operational data. It makes calls (REST and GraphQL) to the Modest Blackwell API to retrieve the data, and LLM to format to human reading.

#### Usage Guide

> Note: make sure Modest Blackwell API program is running and responding to port 8080.

```bash
dotnet run 
```

```
User > Hi!
Wise-Blackwell > Hello! How can I assist you today?
User > What's the name of the asset NT01?
Wise-Blackwell > The name of the asset NT01 is "Nest living room."
User > List all the operational data for Nest living room
Wise-Blackwell > Here is the operational data for the "Nest living room" asset:

### Utilization Streams

1. **NT01.NT1** (Operation Notes):
    - `NT01NT120250722T112322Z`: Rush hour scheduled for July 24th, 2025.
    - `NT01NT120250725T102523Z`: Change filter.

2. **NT01.T01** (Current Temperatures):
    - `NT01T0120250725T103258Z`: 73°F.
    - `NT01T0120250725T112658Z`: 74°F.

3. **NT01.T02** (Target Temperatures):
    - `NT01T0220250725T103258Z`: 75°F.
    - `NT01T0220250725T112658Z`: 74°F.
    - `NT01T0220250725T123258Z`: 73°F.
    - `NT01T0220250725T132658Z`: 74°F.

4. **NT01.O01** (Operating Status - Offline Duration):
    - Timestamps:
        - `NT01O0120250725T103258Z`: 0 hours.
        - `NT01O0120250725T112658Z`: 0 hours.
        - `NT01O0120250725T123258Z`: 0 hours.
        - `NT01O0120250725T132658Z`: 0 hours.

5. **NT01.O02** (Device Loop/Activity Status):
    - Timestamps:
        - `NT01O0220250725T103258Z`: Active.
        - `NT01O0220250725T112658Z`: Active.
        - `NT01O0220250725T123258Z`: Active.
        - `NT01O0220250725T132658Z`: Inactive.

6. **NT01.O03** (Historical Error Check/Time Further Aggregation if just iterations):
    - Timestamps:
      OR-log:
    any Error responsible being caused beyond aggregation Error followup context).

User > bye
Wise-Blackwell > Goodbye! If you need assistance again, feel free to reach out. Have a great day! 😊
```

### Edifice Blackwell Console

A .NET Core 9.0 Console application that serves as an AI assistant for accessing home automation data using natural language. This application understands user intent and generates appropriate GraphQL queries to retrieve operational data from assets.

#### Overview

Edifice Blackwell is currently in **Phase 1** development, focusing on extracting location and operational data type filters from natural language queries and generating corresponding GraphQL queries.

#### Features

- **Natural Language Processing**: Converts user queries into structured intent
- **GraphQL Query Generation**: Automatically generates GraphQL queries based on extracted intent
- **Smart Filtering**: Supports filtering by asset location and operational data type
- **Demo Mode**: Demonstrates functionality without requiring Azure AI credentials
- **Azure AI Integration**: Uses Azure OpenAI for intent extraction with structured outputs

#### Technology Stack

- **.NET Core 9.0** - Console application framework
- **Microsoft Semantic Kernel** - AI orchestration and Azure AI integration
- **Handlebars Templates** - Prompt management in YAML format
- **Azure OpenAI** - LLM for intent extraction with structured outputs
- **JSON Structured Outputs** - Deterministic response parsing

#### Supported Operational Data Types

- **`alarm`** - Equipment alarms, alerts, and warning messages
- **`notification`** - System notifications and informational alerts
- **`utilization`** - Sensor readings, temperature data, usage information, measurements

#### Supported Locations

Any typical house location such as: living room, kitchen, family room, bedroom, bathroom, garage, etc.

#### Getting Started

**Prerequisites:**
- .NET Core 9.0 SDK
- Azure OpenAI API access (for production mode)

**Demo Mode (No API Key Required):**
```bash
cd EdificeBlackwell
dotnet build
dotnet run demo
```

**Test Mode (Verify Functionality):**
```bash
cd EdificeBlackwell
dotnet run test
```

**Production Mode (Requires Azure OpenAI API Key):**
```bash
cd EdificeBlackwell
export AZURE_OPENAI_API_KEY="your-api-key-here"
dotnet run
```

#### Configuration

The application uses environment variables for configuration:

- `AZURE_OPENAI_ENDPOINT` (optional) - Azure OpenAI endpoint URL
  - Default: `https://ff-openai-gpt-4.openai.azure.com/`
- `AZURE_OPENAI_API_KEY` (required) - Azure OpenAI API key
- `AZURE_OPENAI_MODEL_ID` (optional) - Azure OpenAI model deployment ID
  - Default: `gpt-4-0125-Preview`

#### Usage Examples

**Demo Mode Output:**
```
=== EdificeBlackwell Demo Mode ===
Demonstrating GraphQL query generation with sample intents

🔤 User Query: 'List the alarms for living room'

📋 Extracted Intent:
  Location: living room
  Operational Data Type: alarm
  Relevant: True
  Context: User wants to see alarm data for living room

✅ Generated GraphQL Query:
----------------------------------------
query {
  asset(location: "Living Room") {
    type(name: "alarm") {
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
----------------------------------------
```

**Interactive Mode (with Azure AI):**
```
Query: Show me temperature sensors in the kitchen

🔤 Processing: 'Show me temperature sensors in the kitchen'

🤖 Analyzing query intent...
📋 Extracted Intent:
  Location: kitchen
  Operational Data Type: utilization
  Relevant: True

🔍 Generating GraphQL query...
✅ Generated GraphQL Query:
----------------------------------------
query {
  asset(location: "Kitchen") {
    type(name: "utilization") {
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
----------------------------------------
```

#### Architecture

The application follows SOLID principles and uses a clean architecture:

```
EdificeBlackwell/
├── Models/              # Data models and structured outputs
│   └── QueryIntent.cs   # Intent extraction model
├── Services/            # Business logic services
│   ├── AiService.cs            # Azure AI integration
│   ├── GraphQLQueryService.cs  # GraphQL query generation
│   ├── ConfigurationService.cs # Environment configuration
│   └── DemoService.cs          # Demo mode functionality
├── Prompts/             # AI prompt templates
│   └── ExtractGraphQLIntent.yaml
└── Program.cs           # Application entry point
```

#### Prompt Engineering

The application uses a structured YAML prompt template that:
- Provides clear instructions for intent extraction
- Includes examples for different query types
- Enforces JSON structured output format
- Handles edge cases and irrelevant queries

#### Current Limitations (Phase 1)

- Only supports location and operational data type filtering
- Ignores other query components not related to these filters
- Requires Azure OpenAI API key for production use
- GraphQL queries are generated but not executed (next phase)

#### Future Development

Phase 2 will include:
- Integration with GraphQL server API
- Execution of generated queries
- Real-time data retrieval and display
- Enhanced query capabilities

## Development Environment

This project uses a dev container with:
- .NET Core 9.0 SDK
- Python support
- libsnappy-dev (for RocksDB compression)
- C# dev tools and extensions
