# 🚨 Emergency Workflow Cleanup Guide

## Current Situation
The repository has accumulated failed workflow runs from new monitoring workflows that have configuration issues. This document provides immediate cleanup steps.

## 🔧 Immediate Actions Needed

### 1. Disable Failing Monitoring Workflows
The new monitoring workflows are failing due to configuration issues:

```bash
# Navigate to repository
cd .github/workflows/

# Disable problematic workflows temporarily
mv workflow-failure-monitor.yml workflow-failure-monitor.yml.disabled
mv issue-cleanup-monitor.yml issue-cleanup-monitor.yml.disabled  
mv issue-pr-linking-guide.yml issue-pr-linking-guide.yml.disabled
```

### 2. Clean Up Failed Workflow Runs
```bash
# Delete recent failed runs (last 20 failures)
gh run list --status=failure --limit=20 --json id --jq '.[].id' | xargs -I {} gh run delete {}

# Delete cancelled runs
gh run list --status=cancelled --limit=10 --json id --jq '.[].id' | xargs -I {} gh run delete {}
```

### 3. Keep Only Essential Workflows
Current working workflows that should remain active:
- ✅ `conflict-resolution-agent.yml` - Core functionality
- ✅ `auto-merge-coordinator.yml` - Core functionality  
- ✅ `create-rfc-issues.yml` - Manual trigger only
- ✅ `setup-automated-merge.yml` - Manual trigger only
- ✅ `test-workflow.yml` - Manual trigger only

## 🔍 Root Cause Analysis

### Why Monitoring Workflows Failed
1. **Permission Issues**: `workflow_run` triggers need elevated permissions
2. **Fork Limitations**: Schedule triggers don't work in forks  
3. **Complex Logic**: Too much complexity in first implementation
4. **API Rate Limits**: Too many GitHub API calls in monitoring workflows

### Lessons Learned
- Start with simple implementations
- Test workflows in draft mode first
- Use manual triggers during development
- Validate permissions before deploying

## 🚀 Recovery Plan

### Phase 1: Stabilize (Immediate)
- [x] Disable failing workflows
- [ ] Clean up failed runs  
- [ ] Commit cleanup changes
- [ ] Verify only working workflows remain

### Phase 2: Rebuild (Short-term)
- [ ] Redesign monitoring workflows with simpler logic
- [ ] Use `workflow_dispatch` instead of `schedule` triggers
- [ ] Test each workflow individually before deploying
- [ ] Add proper error handling and fallbacks

### Phase 3: Enhance (Long-term)  
- [ ] Add back monitoring features gradually
- [ ] Implement proper workflow health checks
- [ ] Create workflow testing framework
- [ ] Document workflow architecture

## 🤖 Automated Cleanup Command

Run this single command to execute emergency cleanup:

```bash
# Emergency cleanup - disable failing workflows and clean runs
cd .github/workflows/ && \
mv workflow-failure-monitor.yml workflow-failure-monitor.yml.disabled 2>/dev/null || true && \
mv issue-cleanup-monitor.yml issue-cleanup-monitor.yml.disabled 2>/dev/null || true && \
mv issue-pr-linking-guide.yml issue-pr-linking-guide.yml.disabled 2>/dev/null || true && \
echo "✅ Disabled problematic workflows" && \
cd ../.. && \
gh run list --status=failure --limit=20 --json id --jq '.[].id' | head -10 | xargs -I {} gh run delete {} 2>/dev/null && \
echo "✅ Cleaned up failed runs" && \
git add . && \
git commit -m "emergency: disable failing monitoring workflows for cleanup" && \
git push && \
echo "🎉 Emergency cleanup complete!"
```

## ✅ Success Criteria

After emergency cleanup:
- [ ] No workflows failing on every push
- [ ] Clean workflow run history  
- [ ] Only essential workflows enabled
- [ ] Repository ready for gradual re-enablement

## 📞 Next Steps

1. **Execute emergency cleanup** using automated command above
2. **Verify stability** - ensure no more workflow spam
3. **Plan monitoring redesign** - simpler, more reliable approach  
4. **Gradual re-enablement** - test each feature individually

---

*Emergency procedures for workflow cleanup and stabilization*