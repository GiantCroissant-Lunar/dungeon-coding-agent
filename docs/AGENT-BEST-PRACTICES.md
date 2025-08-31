# 🤖 GitHub Copilot Agent Best Practices

## 🔗 Automatic Issue Closing

### **Critical: Use GitHub Keywords for Issue Linking**

When creating pull requests, **always** include linking keywords in your PR description to automatically close related issues when merged:

```markdown
## Implementation Summary
This PR implements [feature/fix description]

Closes #123
```

### **Supported Keywords**
- `Closes #123` / `Close #123` / `Closed #123`
- `Fixes #123` / `Fix #123` / `Fixed #123`  
- `Resolves #123` / `Resolve #123` / `Resolved #123`

### **Why This Matters**
✅ **Automatic Cleanup**: Issues close automatically when PR merges  
✅ **Project Organization**: Keeps issue tracker clean without manual work  
✅ **Audit Trail**: GitHub maintains linkage between issues and implementations  
✅ **Team Efficiency**: No human intervention needed for issue management  

### **RFC Implementation Pattern**
For RFC implementation PRs, use this template:

```markdown
## RFC Implementation: [RFC Title]

This PR implements [RFC001/RFC002/etc] according to the specification in `docs/RFC/RFC001-*.md`.

### Key Changes
- [List major implementation points]
- [Architecture decisions made]
- [Integration points addressed]

### Testing
- [Test coverage achieved]
- [Test scenarios covered]

Closes #[RFC_ISSUE_NUMBER]
```

## 🏗️ Development Workflow

### **Branch Naming**
- RFC Implementation: `feature/rfc001-core-game-loop`
- Bug Fixes: `fix/issue-description`  
- Conflict Resolution: `conflict/pr-123-resolution`

### **Commit Messages**
- Use clear, descriptive commit messages
- Reference issue numbers: `git commit -m "implement core game loop (fixes #123)"`
- Follow conventional commits when possible

### **PR Management**
1. **Create Draft PRs** for work-in-progress
2. **Mark Ready for Review** when implementation complete  
3. **Respond to Coordination Agent** feedback promptly
4. **Keep PRs Focused** - one RFC or feature per PR

## 🔄 Conflict Resolution

### **When Conflicts Occur**
1. **Automatic Detection**: Conflict Resolution Agent will detect and assign you
2. **Branch Checkout**: Work on the existing PR branch (don't create new branches)
3. **Rebase Strategy**: Use `git rebase main` to integrate latest changes
4. **Integration Focus**: Pay attention to recently merged RFCs (like RFC001)
5. **Force Push**: Use `git push --force-with-lease` to update PR

### **Integration Guidelines**
- Review recently merged code for architecture patterns
- Ensure compatibility with ECS framework and game state management  
- Maintain consistency with established naming conventions
- Test integration points thoroughly

## 📋 Quality Standards

### **Code Quality Requirements**
- **Namespace**: All code in `DungeonCodingAgent.*` namespace
- **Testing**: >80% code coverage for new functionality
- **Documentation**: XML docs for public APIs
- **Standards**: Follow C# conventions and project patterns

### **Architecture Compliance**
- **ECS Pattern**: Use Arch ECS framework appropriately
- **Terminal.Gui**: Follow established UI patterns
- **Game State**: Integrate with game state management system
- **Event System**: Use project's event communication patterns

## 🤝 Agent Collaboration

### **Parallel Development**
- **Check for Conflicts**: Review open PRs before starting work
- **Coordinate Dependencies**: Communicate about shared components
- **Reuse Patterns**: Follow patterns from successfully merged PRs
- **Integration Testing**: Verify compatibility with other agent work

### **Communication**
- **Clear PR Descriptions**: Explain what was implemented and why
- **Reference Issues**: Always link to related issues and RFCs
- **Document Decisions**: Explain non-obvious implementation choices
- **Update Status**: Keep RFC status updated throughout implementation

## 🚨 Critical Reminders

### **Never Create Multiple Issues/PRs**
❌ **Wrong**: Create separate conflict resolution issues  
✅ **Correct**: Work on existing PR branches and issues

### **Always Use Issue Linking Keywords**
❌ **Wrong**: "This implements the core game loop"  
✅ **Correct**: "This implements the core game loop - Closes #123"

### **Follow RFC Status Updates**
❌ **Wrong**: Leave RFC status as "Draft" after implementation  
✅ **Correct**: Update RFC to "In Progress" → "Complete" as you work

### **Coordinate with Other Agents**
❌ **Wrong**: Ignore conflicts and force push  
✅ **Correct**: Integrate with recently merged changes properly

---

Following these practices ensures smooth agent-to-agent collaboration and maintains project quality! 🚀