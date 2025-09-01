# 🐉 Dungeon Coding Agent

A classic rogue-like dungeon crawler built with .NET, Arch ECS, and Terminal.Gui v2. **This project is designed specifically to test GitHub Copilot Coding Agent capabilities** with parallel AI-driven development.

## 🤖 For GitHub Copilot Coding Agents

**👋 Welcome, Coding Agent!** This project is set up for parallel RFC implementation. Here's how to get started:

### **Quick Start**
1. 📋 **Check available work**: Browse [docs/RFC/README.md](docs/RFC/README.md) for ready RFCs
2. 🎯 **Pick an RFC**: Choose one marked `📝 Draft` with dependencies met  
3. 🚨 **Claim your work**: Create GitHub issue `"Implement RFC00X: [Title]"` and assign to yourself
4. 📖 **Read instructions**: See [AGENTS.md](AGENTS.md) for coding guidelines and patterns
5. ✅ **Complete the RFC**: Implement according to acceptance criteria

### **Available RFCs (Ready for Implementation)**

#### **🔥 High Priority - Start Here**
- **[RFC001: Core Game Loop](docs/RFC/RFC001-Core-Game-Loop.md)** - Game engine and state management
- **[RFC002: Terminal.Gui Shell](docs/RFC/RFC002-Terminal-Application-Shell.md)** - UI framework foundation

#### **⚡ Can Work in Parallel** 
- **[RFC007: Message Log UI](docs/RFC/RFC007-Simple-UI-Messages.md)** - Scrolling game messages (1 day)
- **[RFC010: Health Status Bar](docs/RFC/RFC010-Health-Status-Bar.md)** - Visual health/mana bars (1 day)  
- **[RFC008: Save Game System](docs/RFC/RFC008-Save-Game-Data.md)** - JSON save/load (2 days)

#### **🎮 Core Gameplay**
- **[RFC003: Map Generation](docs/RFC/RFC003-Map-Generation-System.md)** - Dungeon layout and rendering
- **[RFC004: Player Movement](docs/RFC/RFC004-Player-Movement-System.md)** - Input and collision system
- **[RFC006: Basic Inventory](docs/RFC/RFC006-Basic-Inventory.md)** - Item pickup/drop/use (2 days)
- **[RFC009: Simple Enemy AI](docs/RFC/RFC009-Simple-Enemy-AI.md)** - Chase/attack enemies (2-3 days)

**📝 Total: 9 RFCs ready for implementation - estimated 1-3 days each**

## 🎯 Game Vision

Classic terminal-based rogue-like with:
- **Procedural dungeons** with rooms and corridors
- **Turn-based combat** with tactical positioning  
- **ASCII/Unicode graphics** in full color
- **Classic mechanics**: health/mana, inventory, experience/levels
- **Save/load** functionality
- **Multiple enemy types** with different AI behaviors

## 🛠️ Technical Stack

- **.NET 8** - Modern C# development platform
- **Arch ECS** - High-performance Entity Component System  
- **Terminal.Gui v2** - Cross-platform terminal UI framework
- **xUnit** - Unit testing framework
- **System.Text.Json** - Save/load serialization

## 🏗️ Development Setup

```bash
# Clone repository
git clone [repository-url]
cd dungeon-coding-agent

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests  
dotnet test

# Run game
dotnet run --project src/DungeonCodingAgent.Game
```

## 📋 RFC-Driven Development

This project uses **Request for Comments (RFCs)** to define small, focused work packages perfect for AI agents:

- **Small Scope**: Each RFC completable in 1-3 days
- **Clear Requirements**: Detailed acceptance criteria and definition of done
- **Minimal Dependencies**: Designed for parallel implementation
- **Complete Specifications**: Everything needed to implement independently

See [docs/RFC/README.md](docs/RFC/README.md) for the complete RFC index and assignment guidelines.

## 🎮 Expected Gameplay

```
┌─────────────────────────────────────────────────────────────┐
│ File  Game  Options  Help                          [Turn 15] │
├─────────────────────────────────────────────────────────────┤
│  ########################                                    │
│  #......#..............#    g = goblin                      │  
│  #......#..............#    o = orc                         │
│  #..@...+..............#    ! = potion                      │
│  #......#..............#    @ = you                         │
│  #......################                                    │
│  ########################                                    │
├─────────────────────────────────────────────────────────────┤
│ Health: [████████████░░░░░] 60/80 | Mana: [██████░░░░░] 25/40 │
│ Level: 2 | Exp: [█████░░░░░░░] 150/300 | Turn: 15           │
├─────────────────────────────────────────────────────────────┤
│ > You move west                                             │
│ > The goblin hits you for 4 damage                         │  
│ > You attack the goblin for 6 damage                       │
│ > The goblin dies!                                          │
└─────────────────────────────────────────────────────────────┘
```

## 📚 Documentation

- **[CLAUDE.md](CLAUDE.md)** - Complete project context and architecture overview
- **[AGENTS.md](AGENTS.md)** - Detailed coding guidelines and patterns for agents
- **[docs/RFC/](docs/RFC/)** - All RFC specifications and implementation guides

## 🚀 Project Goals

1. **Test AI collaboration** - Multiple agents working on parallel RFCs
2. **Create playable game** - Full dungeon crawler experience  
3. **Demonstrate architecture** - Clean ECS + Terminal.Gui patterns
4. **Validate process** - RFC-driven development for AI teams

---

## 🚀 How to Use with GitHub Copilot Coding Agent

### **Quick Start for Fully Automated Agent Development:**
1. **⚡ Run Setup Workflow** - Enable complete automation (no human intervention)
2. **📋 Create RFC Issues** - Use workflow to create all 9 implementation tasks  
3. **🤖 Assign to @copilot** - Implementation agents work autonomously
4. **🔄 Watch Magic Happen** - Coordination agent handles review, merge, integration
5. **🎮 Play Game** - Working dungeon crawler emerges from parallel agent collaboration

**👉 See [GETTING-STARTED-WITH-COPILOT.md](GETTING-STARTED-WITH-COPILOT.md) for step-by-step instructions**

### **For Human Developers:**
Pick an RFC from [docs/RFC/README.md](docs/RFC/README.md) and start implementing! Each RFC includes everything you need to build a complete game system independently.

## 🏛️ Public Repository Policies & Setup

- License: See [LICENSE](./LICENSE)
- Code of Conduct: See [CODE_OF_CONDUCT.md](./CODE_OF_CONDUCT.md)
- Contributing Guide: See [CONTRIBUTING.md](./CONTRIBUTING.md)
- Security Policy: See [SECURITY.md](./SECURITY.md)
- GitHub App and Secrets Setup for automation: [docs/GITHUB-APP-SETUP.md](./docs/GITHUB-APP-SETUP.md)