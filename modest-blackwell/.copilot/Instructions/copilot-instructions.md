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

Modest Blackwell is an API, providing access to the automation equipments in a house. It will answer GraphQL queries and the data is stored in a RocksDB database.

## Architecture Overview

### Technology Stack
- **Backend**: .Net Core 9.0 using C#
- **Data Storage**: RocksDB using C# wrapper RocksDbSharp. 
- Use Swagger for manual API testing

## Project Structure Overview

See `mcp-browser-architecture.md` for detailed component organization. Key locations:
- `src/` - React frontend with components, hooks, types
- `src-tauri/src/` - Rust backend with database, MCP process management, and Tauri commands
- Documentation files in root directory

## 

This application is specifically designed for MCP (Model Context Protocol) server testing and exploration. The app provides a Postman-like interface for MCP server development and testing with dynamic server connection management, tool execution, and request/response history.

# Project general coding standards

## Naming Conventions
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
- Follow the standard code structure for .NET C# and Blazor pages.

## Development Considerations
- When using the Terminal to test anything, always redirect stdout and stderr to a file and then read the file. Delete the file after reading it. This ensures that you can see the output of your commands, as there is currently a bug in VSCode. Please cat the file so I can see the output as well.
- Use environment variables for configuration (e.g., database credentials, API keys)
- Ensure that sensitive information is not hardcoded in the codebase
- When creating new files, consider whether there should be any changes to the `.gitignore` file, especially for logs, temporary files, or build artifacts
- As you write code, make sure that you update the [README.md](../../README.md), updating any application server commands, description, dependencies, and installation instructions. This file serves as the main documentation for the project and should always reflect the current state of the application.
- As you write code, make sure that you update the [tools/install.sh](../../tools/install.sh) script, if applicable. This script considers the installation and setup of the application, including environment variables and dependencies.
- As you write code, make sure that you update the [tools/cleanup.sh](../../tools/cleanup.sh) script, if applicable. This script considers the cleanup of the installation, putting you in a clean state for the next setup of the application.

## Development validation
- When testing features, if you need to run the development server, check if it's already running before starting it again
- Use full paths when running commands instead of relative paths to avoid working in the wrong directory
- For UI testing, please do not test yourself, but instead ask me to test it. Provide details on what you'd like me to test, including specific actions to take and expected outcomes. I will then run the tests and provide feedback.
- All files that you generate for testing should go into the system temp directory, which is `/tmp` on Linux. This ensures that temporary files do not clutter the project directory.

## Error Handling
- Use try/catch blocks for async operations
- Always log errors with contextual information