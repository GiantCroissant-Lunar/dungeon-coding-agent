# 📋 RFC Index - Dungeon Coding Agent

## About RFCs

Request for Comments (RFCs) define the technical specifications for each major system in the dungeon crawler. Each RFC is designed to be implemented independently by GitHub Copilot Coding Agents working in parallel.

## 📊 RFC Status Legend

- **📝 Draft**: Specification complete, ready for implementation
- **🔄 In Progress**: Currently being implemented
- **✅ Complete**: Implementation finished and tested
- **⏸️ Blocked**: Waiting on dependencies
- **❌ Rejected**: Specification rejected or replaced

## 🎯 RFC Directory

### **Foundation Systems** (Core Architecture)

| RFC | Title | Status | Dependencies | Assignable |
|-----|-------|--------|--------------|------------|
| [RFC001](RFC001-Core-Game-Loop.md) | Core Game Loop & State Management | 📝 Draft | None | ✅ Yes |
| [RFC002](RFC002-Terminal-Application-Shell.md) | Terminal.Gui Application Shell | 📝 Draft | RFC001 | ✅ Yes |

### **Game Systems** (Gameplay Logic)

| RFC | Title | Status | Dependencies | Assignable |
|-----|-------|--------|--------------|------------|
| [RFC003](RFC003-Map-Generation-System.md) | Map Generation & Rendering System | 📝 Draft | RFC001, RFC002 | ✅ Yes |
| [RFC004](RFC004-Player-Movement-System.md) | Player Entity & Movement System | 📝 Draft | RFC001, RFC003 | ✅ Yes |
| RFC005 | Combat Resolution Engine | 📝 Draft | RFC001, RFC004 | ✅ Yes |
| [RFC006](RFC006-Basic-Inventory.md) | Basic Inventory System | 📝 Draft | RFC001, RFC004 | ✅ Yes |
| [RFC009](RFC009-Simple-Enemy-AI.md) | Simple Enemy AI | 📝 Draft | RFC001, RFC003, RFC004 | ✅ Yes |

### **Supporting Systems** (Features & Polish)

| RFC | Title | Status | Dependencies | Assignable |
|-----|-------|--------|--------------|------------|
| [RFC007](RFC007-Simple-UI-Messages.md) | Simple Message Log UI | 📝 Draft | RFC002 | ✅ Yes |
| [RFC008](RFC008-Save-Game-Data.md) | Save Game Data System | 📝 Draft | RFC001 | ✅ Yes |
| [RFC010](RFC010-Health-Status-Bar.md) | Health and Status Bar UI | 📝 Draft | RFC002, RFC004 | ✅ Yes |

## 🤖 For GitHub Copilot Coding Agents

### **Ready for Implementation**
The following RFCs are **complete and ready** for agent assignment:

#### **RFC001: Core Game Loop** ⭐ **HIGH PRIORITY**
- **Status**: 📝 Draft - Complete specification
- **Dependencies**: None (foundational)
- **Effort**: 2-3 days
- **Risk**: Low
- **Description**: Core game engine, turn-based logic, state management

#### **RFC002: Terminal.Gui Shell** ⭐ **HIGH PRIORITY**  
- **Status**: 📝 Draft - Complete specification
- **Dependencies**: RFC001 
- **Effort**: 3-4 days  
- **Risk**: Medium
- **Description**: UI framework, terminal management, application lifecycle

### **Implementation Guidelines**

1. **Choose an Available RFC**: Select a RFC marked as 📝 Draft with dependencies met
2. **Read Complete Specification**: Understand objectives, technical details, and acceptance criteria
3. **Check Integration Points**: Review events consumed/published and system interfaces
4. **Implement with Tests**: Follow TDD practices, achieve >80% coverage
5. **Verify Acceptance Criteria**: Ensure all checkboxes are completed
6. **Update RFC Status**: Change status to ✅ Complete when finished

### **Parallel Development Strategy**

#### **Independent Workstreams**
- **RFC001** and **RFC002** can be developed simultaneously
- Future RFCs designed for minimal cross-dependencies
- Each RFC defines clear system boundaries and interfaces

#### **Integration Pattern**
```
RFC001 (Core) ←── RFC002 (UI Shell)
     ↓              ↓
   RFC003-008   RFC007 (UI Components)
```

## 📝 RFC Template

When creating new RFCs, follow the established template:

```markdown
# RFCxxx: System Name

## 📋 Metadata
- RFC Number, Title, Status, Author, Created, Dependencies

## 🎯 Objective  
Clear problem statement and goals

## 📖 Problem Statement
Detailed context and requirements

## 🏗️ Technical Specification
Complete technical design with code examples

## ✅ Acceptance Criteria
Testable completion requirements

## 🔗 Integration Points
Dependencies, dependents, events

## 📊 Performance Considerations
Optimization targets and constraints

## 🧪 Testing Strategy
Unit, integration, and performance tests
```

## 🔄 RFC Lifecycle

1. **Planning**: Architect creates RFC with complete specification
2. **Review**: RFC reviewed for completeness and feasibility  
3. **Ready**: RFC marked as 📝 Draft and available for assignment
4. **Implementation**: Agent implements according to specification
5. **Testing**: Agent completes acceptance criteria and tests
6. **Integration**: RFC integrated with other systems
7. **Complete**: RFC marked as ✅ Complete

## 📊 Progress Tracking

### **Overall Progress**
- **Foundation Systems**: 2/2 ready for implementation  
- **Game Systems**: 4/5 specifications complete
- **Supporting Systems**: 3/3 specifications complete
- **Total RFCs**: 9/10 ready for coding agents

### **Assignment Strategy**
1. **Start with RFC001 + RFC002** - Core foundation (can work in parallel)
2. **Then RFC003 + RFC007 + RFC010** - Map and UI systems  
3. **Next RFC004 + RFC006 + RFC008** - Player systems and save/load
4. **Finally RFC009** - Enemy AI (needs player movement complete)

### **Estimated Timeline**
- **Week 1**: Foundation systems (RFC001, RFC002)
- **Week 2**: Core gameplay (RFC003, RFC004, RFC007, RFC010) 
- **Week 3**: Game features (RFC006, RFC008, RFC009)
- **Week 4**: Integration, testing, polish

---

## 🎮 Happy Implementation!

Each RFC is designed to be a complete, standalone work package that contributes to the overall dungeon crawler experience. The modular design allows multiple agents to work efficiently in parallel while building a cohesive game.

**Questions or clarifications?** Check the main [CLAUDE.md](../CLAUDE.md) and [AGENTS.md](../AGENTS.md) for project context and implementation guidelines.