# 🐉 Dungeon Coding Agent

A classic rogue-like dungeon crawler built with .NET, Arch ECS, and Terminal.Gui v2. This project serves as a GitHub Copilot Coding Agent experiment for parallel AI-driven development.

## 🎯 Project Overview

### **Core Concept**
Terminal-based rogue-like with classic mechanics:
- Procedurally generated dungeons
- Turn-based movement and combat
- Character progression and equipment
- ASCII/Unicode visual representation
- Save/load functionality

### **Technical Architecture**
- **.NET 8**: Modern C# development platform
- **Arch ECS**: High-performance Entity Component System for game logic
- **Terminal.Gui v2**: Cross-platform terminal UI framework
- **Modular Design**: Component-based architecture for parallel development

## 🤖 GitHub Copilot Coding Agent Integration

### **Agent-Driven Development Strategy**
This project is designed for parallel implementation by multiple GitHub Copilot Coding Agents:

1. **RFC-Based Task Decomposition**: Each major system defined in detailed RFCs
2. **Minimal Inter-Dependencies**: Components communicate through well-defined interfaces
3. **Clear Definition of Done**: Comprehensive acceptance criteria per RFC
4. **Parallel Workstreams**: Agents can work simultaneously on different systems

### **Agent Instructions**
See `AGENTS.md` for detailed GitHub Copilot Coding Agent instructions including:
- Architecture patterns and conventions
- Code style guidelines
- Testing expectations
- Integration requirements

## 📋 RFC System Architecture

### **Completed RFCs**
*(None yet - RFCs to be created)*

### **Planned RFC Structure**
- **RFC001** - Core Game Loop & State Management
- **RFC002** - Terminal.Gui Application Shell
- **RFC003** - Map Generation & Rendering System
- **RFC004** - Player Entity & Movement System  
- **RFC005** - Combat Resolution Engine
- **RFC006** - Inventory & Equipment System
- **RFC007** - UI Panel Components
- **RFC008** - Save/Load Persistence System

Each RFC defines:
- **System Boundaries**: Clear interfaces and responsibilities
- **Implementation Details**: Technical requirements and patterns
- **Acceptance Criteria**: Testable completion requirements
- **Integration Points**: How the system connects to others

## 🏗️ Project Structure

```
dungeon-coding-agent/
├── src/
│   └── DungeonCodingAgent.Game/          # Main game project
├── tests/
│   └── DungeonCodingAgent.Tests/         # Unit tests
├── docs/
│   └── RFC/                              # Request for Comments documents
├── .github/
│   └── workflows/                        # GitHub Actions (if needed)
├── CLAUDE.md                             # This file - project context
├── AGENTS.md                             # GitHub Copilot Coding Agent instructions
└── README.md                             # Public documentation
```

## 🔧 Development Setup

### **Prerequisites**
- .NET 8 SDK or later
- Terminal that supports Unicode/ANSI escape sequences

### **Build Commands**
```bash
# Restore dependencies
dotnet restore

# Build solution  
dotnet build

# Run game
dotnet run --project src/DungeonCodingAgent.Game

# Run tests
dotnet test
```

### **Dependencies**
- **Terminal.Gui 2.0.0**: Terminal UI framework
- **Arch 2.0.0**: Entity Component System
- **xUnit**: Testing framework

## 🎮 Game Design Vision

### **Core Mechanics**
- **Turn-Based**: Classic rogue-like timing
- **Grid-Based Movement**: Discrete tile-based positioning
- **Procedural Generation**: Randomized dungeon layouts
- **Resource Management**: Health, mana, inventory space
- **Character Progression**: Experience, levels, equipment

### **UI Layout Concept**
```
┌─────────────────────────────────────────────────────────────┐
│                        Dungeon Map                          │
│  ########################                                   │
│  #......#.............#                                     │
│  #......#.............#                                     │  
│  #..@...#.............#                                     │
│  #......###############                                     │
│  ########################                                   │
├─────────────────────────────────────────────────────────────┤
│ Health: ████████████████████ | Mana: ████████░░░░░░░░░░     │
├─────────────────────────────────────────────────────────────┤
│ Message Log:                                                │
│ > You entered the dungeon                                   │
│ > A goblin appears!                                         │
│ > You attack the goblin for 5 damage                       │
└─────────────────────────────────────────────────────────────┘
```

## 🧪 Testing Strategy

### **Unit Testing Approach**
- **System Tests**: Each ECS system tested in isolation
- **Component Tests**: Data structures and game state validation  
- **Integration Tests**: System interactions and game flow
- **UI Tests**: Terminal.Gui component behavior (where feasible)

### **Test Organization**
Follow RFC boundaries - each RFC implementation should include corresponding test coverage.

## 📚 Documentation Standards

### **Code Documentation**
- XML documentation for public APIs
- Inline comments for complex game logic
- README files for each major namespace

### **RFC Documentation**
- Clear problem statements and objectives
- Detailed technical specifications
- Acceptance criteria and test requirements
- Integration guidelines

## 🔗 Related Projects

This project follows patterns established in the lunar-snake workspace:
- **yokan-projects**: Game development framework projects
- **Arch ECS**: Modern entity-component-system architecture
- **Terminal UI**: Cross-platform console application development

## 🚀 Getting Started for Agents

1. **Read AGENTS.md**: Understand development guidelines and patterns
2. **Review Relevant RFC**: Understand system requirements and boundaries  
3. **Check Integration Points**: Ensure compatibility with existing systems
4. **Implement with Tests**: Follow TDD practices where applicable
5. **Update Documentation**: Keep RFC status and documentation current

## 📊 Current Status

- **Project Setup**: ✅ Complete
- **RFC Documentation**: 🔄 In Progress
- **Core Implementation**: ⏳ Awaiting Agent Assignment
- **Testing Infrastructure**: ⏳ Awaiting Implementation
- **GitHub Integration**: ⏳ Pending Public Repository

---
*This project is part of the lunar-snake workspace ecosystem and follows established patterns for AI-assisted development.*