---
applyTo: "**"
---
# Copilot Agent 

This file provides guidance to Copilot Agent when working with code in this repository.

## Quick Start for New Sessions

**Before starting any work, read these files in order:**

1. **`pair_programming.md`** - Our workflow process for story-driven development
2. **`project-plan-{some-extension}.md`** - Current progress and next story to work on  
3. **`technical-considerations.md`** - Lessons learned and implementation decisions
4. **`mcp-browser-architecture.md`** - Overall architecture and design decisions

**Key workflow reminders:**
- Always use the TodoWrite tool to track story progress
- Follow the exact human verification format from pair_programming.md
- Update technical_considerations.md with lessons learned after each story

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

## Phase 2 - GraphQL API for the Notifications, Utilizations, and Alarms

In the phase 2, implement the GraphQL API to access the operational data in RocksDB. It will work like a wrapper around the RocksDbSharp implementation.

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