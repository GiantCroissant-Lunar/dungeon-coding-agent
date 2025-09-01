## ⚠️ Limitations

- GitHub does not provide an API or Actions capability to programmatically start a Copilot Coding Agent session. Workflows and scripts here prepare issues (labels and comments) with all context required, but the agent session is started by a user via the issue UI (“Let Copilot work on this”) or via Copilot Chat, subject to org/repo policy.
- Comments mentioning `@copilot` do not auto-start the agent; they provide instructions/context only.
- Ensure Copilot Coding Agent is enabled in your organization/repository settings; otherwise the UI button will not appear.
# 🤖 GitHub Copilot RFC Auto-Implementation System

This document describes the automated system for triggering GitHub Copilot coding agents to implement RFCs in the dungeon-coding-agent project.

## 🎯 Overview

The system bridges the gap between RFC creation and actual implementation by automatically triggering GitHub Copilot coding agents when RFC issues are ready for implementation.

## 🏗️ Architecture

### **Workflow Chain**
1. **RFC Created** → `docs/RFC/RFC###-*.md` file added
2. **Issue Auto-Created** → `auto-create-rfc-issues-on-push.yml` creates implementation issue
3. **Agent Assignment** → `assign-rfc-issues-to-agent.yml` assigns and labels issue
4. **Copilot Auto-Spawned** → Multiple automated options trigger GitHub Copilot
5. **Implementation** → GitHub Copilot coding agent implements RFC
6. **Pull Request** → Copilot creates PR with implementation

### **Key Components**

#### **Primary Auto-Spawn System** (`.github/workflows/auto-spawn-copilot-agents.yml`)
- **Automatic Discovery**: Scans for ready RFC issues every 30 minutes
- **Event-Driven**: Triggers on issue labels, assignments, edits
- **Capacity Management**: Maintains max 3 concurrent Copilot agents
- **Enhanced Instructions**: Posts comprehensive RFC-specific implementation guides
- **Manual Override**: Workflow dispatch for immediate spawning

#### **Legacy Trigger System** (`.github/workflows/trigger-copilot-implementation.yml`)  
- Simpler event-based triggering
- Posts basic implementation instructions
- Still functional for fallback scenarios

#### **Script Integration System** (`.github/workflows/call-trigger-scripts.yml`)
- Calls standalone PowerShell/Bash scripts from GitHub Actions
- Useful for testing and manual control
- Provides cross-platform script execution in CI/CD

#### **Manual Trigger Scripts**
- **PowerShell**: `trigger-copilot-rfc.ps1` - Windows environments
- **Bash**: `trigger-copilot-rfc.sh` - Unix/Linux/WSL environments

#### **Configuration Files**
- **`.github/copilot-instructions.md`** - Comprehensive Copilot instructions
- **`AGENTS.md`** - Detailed coding standards and architecture patterns

## 🚀 Usage

### **Fully Automatic Preparation** ⭐ **Recommended**
The primary system (`auto-spawn-copilot-agents.yml`) automatically prepares issues for Copilot by:
1. **Scans every 30 minutes** for RFC issues with `rfc-implementation` + `agent-assigned` labels
2. **Triggers on events** when issues are labeled, assigned, or edited
3. **Spawns Copilot agents** with comprehensive, RFC-specific instructions
4. **Manages capacity** to prevent overload (max 3 concurrent agents)
5. Posting detailed guidance comments and labels to maximize success

> Note: Starting a Copilot Coding Agent session itself is not programmatically supported by GitHub Actions. See Limitations below.

### **Manual Control Options**
When you need direct control:

### **Manual Triggering**

#### **PowerShell (Windows)**
```powershell
# Basic usage
.\trigger-copilot-rfc.ps1 -IssueNumber 42

# With custom repository
.\trigger-copilot-rfc.ps1 -IssueNumber 42 -Repository "username/repo-name"

# Force trigger even if already working
.\trigger-copilot-rfc.ps1 -IssueNumber 42 -Force
```

#### **Bash (Unix/Linux/WSL)**
```bash
# Basic usage
./trigger-copilot-rfc.sh --issue 42

# With custom repository  
./trigger-copilot-rfc.sh --issue 42 --repo "username/repo-name"

# Force trigger even if already working
./trigger-copilot-rfc.sh --issue 42 --force

# Show help
./trigger-copilot-rfc.sh --help
```

#### **GitHub Actions (Manual Dispatch)**

**Primary Auto-Spawn System:**
```bash
# Spawn agents for all ready RFCs
gh workflow run "Auto-Spawn Copilot RFC Agents" \
  --field issue_numbers=all \
  --field force_spawn=false

# Spawn agent for specific issues
gh workflow run "Auto-Spawn Copilot RFC Agents" \
  --field issue_numbers="42,43,44" \
  --field force_spawn=true
```

**Script Integration System:**
```bash
# Call bash script via workflow
gh workflow run "Call Trigger Scripts for Copilot Spawning" \
  --field script_type=bash \
  --field issue_number=42 \
  --field force_trigger=false

# Call PowerShell script via workflow  
gh workflow run "Call Trigger Scripts for Copilot Spawning" \
  --field script_type=powershell \
  --field issue_number=42 \
  --field force_trigger=true
```

**Legacy System:**
```bash
# Via GitHub CLI
gh workflow run "Trigger Copilot RFC Implementation" \
  --field issue_number=42 \
  --field force_trigger=false
```

**Via GitHub Web Interface:**
Go to Actions → Select desired workflow → "Run workflow"

## 📋 Configuration

### **Required Secrets**
- `APP_ID`: GitHub App ID for authentication
- `APP_PRIVATE_KEY`: GitHub App private key

### **Required Labels**
The system automatically creates these labels:
- `rfc-implementation`: Marks RFC implementation issues
- `agent-work`: Indicates AI-authored work
- `agent-assigned`: Issue assigned to AI agent
- `ai-review-requested`: AI review requested
- `copilot-working`: GitHub Copilot is actively working
- `in-progress`: Work is actively being done

### **Capacity Management**
- **Default Limit**: 3 simultaneous RFC implementations
- **Configurable**: Modify `REMAINING=$((3 - ACTIVE))` in workflow
- **Prevents Overload**: Ensures quality by limiting concurrent work

## 🎯 Copilot Instructions

When triggered, the system posts comprehensive instructions to the issue including:

### **Implementation Guidelines**
- Read complete RFC specification
- Follow Arch ECS architecture patterns
- Use Terminal.Gui v2 for all UI
- Achieve >80% test coverage
- Complete all RFC acceptance criteria

### **Technical Standards**
- **Components**: Pure data structures (structs)
- **Systems**: Logic classes inheriting from `SystemBase<World, float>`
- **Events**: Use `GameEvents.RaiseXXX()` for inter-system communication
- **File Organization**: Follow established project structure

### **Definition of Done**
- [ ] All RFC acceptance criteria checkboxes completed
- [ ] Unit tests written and passing (>80% coverage)
- [ ] Integration tests verify system works with existing code
- [ ] Code follows project architectural patterns
- [ ] Feature works as demonstrated in RFC
- [ ] Pull request created with detailed description

## 🔍 Monitoring & Debugging

### **Check System Status**
```bash
# List RFC implementation issues
gh issue list --label "rfc-implementation" --state open

# List active Copilot work
gh issue list --label "copilot-working" --state open

# View specific issue
gh issue view 42
```

### **Common Issues**

#### **"Copilot not responding"**
- Check if issue has `copilot-working` label
- Verify issue has proper RFC number in title
- Ensure issue has both `rfc-implementation` and `agent-assigned` labels

#### **"Multiple Copilots working on same RFC"**
- System prevents this with `copilot-working` label
- Use `--force` flag only if certain previous work was abandoned

#### **"Capacity limit reached"**
- System limits to 3 simultaneous implementations
- Wait for current work to complete or increase limit in workflow

## 🛠️ Customization

### **Modify Capacity Limit**
Edit `trigger-copilot-implementation.yml`:
```yaml
REMAINING=$((5 - ACTIVE))  # Change from 3 to 5
```

### **Add Custom Instructions**
Edit `.github/copilot-instructions.md` to add project-specific guidance.

### **Change Trigger Conditions**
Edit the workflow `if` condition to modify when Copilot is triggered.

## 🔗 Integration Points

### **Existing Workflows**
- **Works with**: `auto-create-rfc-issues-on-push.yml`
- **Works with**: `assign-rfc-issues-to-agent.yml`
- **Extends**: GitHub's native Copilot coding agent functionality

### **External Dependencies**
- **GitHub Copilot**: Pro, Business, or Enterprise plan required
- **GitHub CLI**: For manual trigger scripts
- **Repository Permissions**: Issues and pull requests write access

## 🎮 Example Flow

1. **Developer adds RFC**: `docs/RFC/RFC001-Core-Game-Loop.md`
2. **System creates issue**: "RFC001: Core Game Loop - Implementation"  
3. **System assigns agent**: Adds `agent-assigned` and `in-progress` labels
4. **Copilot triggered**: Posts comprehensive implementation comment
5. **Copilot implements**: Creates feature branch, writes code, adds tests
6. **Copilot creates PR**: Submits implementation for review
7. **Review & merge**: Human review and merge to main branch

## 📚 References

- [GitHub Copilot Coding Agent Documentation](https://docs.github.com/en/copilot/concepts/coding-agent/coding-agent)
- [Project Architecture Guidelines](./AGENTS.md)
- [RFC Template and Examples](./docs/RFC/)

---

## 🎯 Ready to Auto-Implement RFCs!

This system transforms your detailed RFCs into working code through automated GitHub Copilot coding agent triggering. Each RFC becomes a well-guided implementation task that Copilot can complete following your project's standards and architecture.