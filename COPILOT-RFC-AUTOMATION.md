# 🤖 GitHub Copilot RFC Auto-Implementation System

This document describes the automated system for preparing GitHub Copilot coding agents to implement RFCs in the dungeon-coding-agent project.

## ⚠️ Important Note About GitHub Copilot Coding Agents

**GitHub does not provide an API to programmatically start Copilot coding agent sessions.** This automation system prepares issues with comprehensive context and labels, but the actual Copilot coding agent must be started manually via:

1. **GitHub Issue UI**: Click "Let Copilot work on this" button (if enabled in your org/repo)
2. **GitHub Copilot Chat**: Mention the prepared issue in a Copilot chat session
3. **Organization Settings**: Ensure Copilot coding agents are enabled in your GitHub organization

## 🎯 What This System Does

The automation system:
- ✅ **Monitors RFC issues** and detects when they're ready for implementation
- ✅ **Adds proper labels** (`copilot-working`, capacity management)
- ✅ **Posts comprehensive instructions** with RFC context and technical requirements
- ✅ **Manages capacity** to prevent overloading (max 3 concurrent implementations)
- ❌ **Cannot start Copilot agents** (manual step required via GitHub UI)

## 🏗️ Architecture

### **Workflow Chain**
1. **RFC Created** → `docs/RFC/RFC###-*.md` file added
2. **Issue Auto-Created** → `auto-create-rfc-issues-on-push.yml` creates implementation issue
3. **Agent Assignment** → `assign-rfc-issues-to-agent.yml` assigns and labels issue
4. **Issue Prepared** → `auto-spawn-copilot-agents.yml` adds comprehensive Copilot instructions
5. **Manual Step** → User starts Copilot coding agent via GitHub UI
6. **Implementation** → GitHub Copilot coding agent implements RFC
7. **Pull Request** → Copilot creates PR with implementation

### **Key Components**

#### **Primary Auto-Preparation System** (`.github/workflows/auto-spawn-copilot-agents.yml`)
- **Automatic Discovery**: Scans for ready RFC issues every 30 minutes
- **Event-Driven**: Triggers on issue labels, assignments, edits
- **Capacity Management**: Maintains max 3 concurrent preparation slots
- **Enhanced Instructions**: Posts comprehensive RFC-specific implementation guides
- **Manual Override**: Workflow dispatch for immediate preparation

## 🚀 Usage

### **Fully Automatic Preparation** ⭐ **Recommended**
The system automatically prepares issues for Copilot by:
1. **Scanning every 30 minutes** for RFC issues with `rfc-implementation` + `agent-assigned` labels
2. **Triggering on events** when issues are labeled, assigned, or edited
3. **Preparing comprehensive instructions** with RFC-specific context
4. **Managing capacity** to prevent overload (max 3 concurrent agents)
5. **Adding labels and context** to maximize implementation success

> **Manual Step Required**: After preparation, start the Copilot coding agent via GitHub UI

### **Manual Workflow Dispatch**

```bash
# Prepare all ready RFCs for Copilot
gh workflow run "Auto-Spawn Copilot RFC Agents" \
  --field issue_numbers=all \
  --field force_spawn=false

# Prepare specific issues
gh workflow run "Auto-Spawn Copilot RFC Agents" \
  --field issue_numbers="42,43,44" \
  --field force_spawn=true
```

**Via GitHub Web Interface:**
Go to Actions → "Auto-Spawn Copilot RFC Agents" → "Run workflow"

## 📋 Configuration

### **Required Secrets**
- `APP_ID`: GitHub App ID for authentication
- `APP_PRIVATE_KEY`: GitHub App private key

### **Environment Variables**
- `COPILOT_MAX_CAPACITY`: Maximum concurrent preparations (default: 3)

### **Required Labels**
The system automatically creates and uses these labels:
- `rfc-implementation`: Marks RFC implementation issues
- `agent-work`: Indicates AI-authored work
- `agent-assigned`: Issue assigned to AI agent
- `ai-review-requested`: AI review requested
- `copilot-prepared`: Issue prepared with detailed instructions for Copilot
- `copilot-working`: Issue actively being worked by Copilot (set after manual activation)
- `in-progress`: Work is actively being done

## 🎯 What Gets Prepared

When an RFC issue is prepared for Copilot, the system adds:

### **Comprehensive Instructions**
- Complete RFC file path and summary
- Architecture patterns (Arch ECS + Terminal.Gui)
- File organization requirements
- Technical implementation guidelines

### **Definition of Done**
- [ ] All RFC acceptance criteria checkboxes completed
- [ ] Unit tests written and passing (>80% coverage)
- [ ] Integration tests verify system compatibility
- [ ] Code follows architectural patterns
- [ ] Pull request created with detailed description

### **Resource Links**
- Architecture guidelines in `AGENTS.md`
- Copilot instructions in `.github/copilot-instructions.md`
- Complete RFC specification file
- Existing code patterns to follow

## 🔍 Monitoring

### **Check Preparation Status**
```bash
# List RFC implementation issues
gh issue list --label "rfc-implementation" --state open

# List issues prepared for Copilot
gh issue list --label "copilot-working" --state open

# View specific prepared issue
gh issue view 42
```

### **Common Scenarios**

#### "No issues getting prepared"
- Check if issues have both `rfc-implementation` and `agent-assigned` labels
- Verify capacity limit hasn't been reached (max configurable; default 3 for assignment workflow; auto-spawn uses COPILOT_MAX_CAPACITY)
- Ensure RFC files exist in `docs/RFC/` directory

#### "Capacity limit reached"
- Assignment workflow limits to 3 simultaneous `rfc-implementation + agent-assigned + in-progress`
- Auto-spawn capacity counts both `copilot-prepared` and `copilot-working`
- Wait for current work to complete, raise capacity, or use `force_spawn=true`

## 🎮 Complete Example Flow

1. **Developer creates RFC**: `docs/RFC/RFC001-Core-Game-Loop.md`
2. **System creates issue**: "RFC001: Core Game Loop - Implementation"  
3. **System assigns labels**: `rfc-implementation`, `agent-assigned`
4. **System prepares issue**: Adds comprehensive Copilot instructions and `copilot-working` label
5. **Developer starts Copilot**: Via GitHub UI "Let Copilot work on this" button
6. **Copilot implements**: Creates feature branch, writes code, adds tests
7. **Copilot creates PR**: Submits implementation for review
8. **Review & merge**: Human review and merge to main branch

## 📚 References

- [GitHub Copilot Coding Agent Documentation](https://docs.github.com/en/copilot/concepts/coding-agent/coding-agent)
- [Project Architecture Guidelines](./AGENTS.md)
- [RFC Template and Examples](./docs/RFC/)

---

## 🎯 Ready to Prepare RFCs for Copilot!

This system transforms your detailed RFCs into perfectly prepared implementation tasks with comprehensive context and instructions. While it cannot start Copilot coding agents directly, it maximizes their success rate by providing all necessary guidance and technical requirements.