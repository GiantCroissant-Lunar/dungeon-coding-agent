---
name: RFC Implementation
about: Assign an RFC to GitHub Copilot Coding Agent for implementation
title: 'Implement RFC00X: [RFC Title]'
labels: ['rfc-implementation', 'copilot']
assignees: []
---

## 🤖 RFC Implementation Request

**RFC**: [RFC Number and Title]
**File**: `docs/RFC/RFC00X-[rfc-name].md`
**Estimated Effort**: [X days]
**Dependencies**: [List any required RFCs that must be completed first]

## 📋 Task Description

Implement the system specified in the linked RFC according to its acceptance criteria and definition of done.

## ✅ Definition of Done

Please complete ALL checkboxes in the RFC specification before marking this issue as complete:

**Review the complete RFC specification**: [Link to RFC file]

The RFC contains detailed:
- Technical specifications with code examples
- Complete acceptance criteria (checkboxes to complete)
- Integration requirements and event definitions
- Testing strategy and performance requirements

## 🎯 Success Criteria

- [ ] All RFC acceptance criteria checkboxes completed
- [ ] Unit tests written and passing (>80% coverage)
- [ ] Code follows project standards from `AGENTS.md`
- [ ] Integration events working with existing systems
- [ ] RFC status updated to `✅ Complete`

## 📚 Resources

- **Agent Guidelines**: `AGENTS.md` - Coding standards and architecture patterns
- **Project Context**: `CLAUDE.md` - Overall project architecture
- **RFC Directory**: `docs/RFC/README.md` - All RFC specifications

## 🔧 Development Notes

When implementing this RFC:

1. **Read the complete RFC specification** - Don't start coding until you understand all requirements
2. **Follow ECS architecture patterns** - Use Arch ECS for all game state and logic
3. **Write tests as you go** - Each system needs corresponding unit tests
4. **Fire integration events** - Systems communicate through the GameEvents class
5. **Update RFC status** - Change from `📝 Draft` to `🔄 In Progress` to `✅ Complete`

---

**@copilot This issue is ready for implementation. Please review the linked RFC specification and implement according to the acceptance criteria.**