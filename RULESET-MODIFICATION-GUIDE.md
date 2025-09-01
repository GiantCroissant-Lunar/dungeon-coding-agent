# 🛠️ Repository Ruleset Modification Guide

## Current Situation
The repository has a ruleset "Main protection with App bypass" (ID: 7784048) that enforces Gemini AI review even when bypass labels are present. This is blocking experimental workflows.

## 🎯 **Web UI Method** (Recommended)

### **Step 1: Access Ruleset Settings**
1. Go to: https://github.com/GiantCroissant-Lunar/dungeon-coding-agent/settings/rules
2. Click "Edit" on "Main protection with App bypass" ruleset

### **Step 2: Add Bypass Actors**
In the "Bypass list" section, add:

#### **Option A: Add Yourself as Bypass Actor**
- **Actor type**: "User"
- **Actor**: Your GitHub username
- **Bypass mode**: "Pull request" 
- **Reason**: "Admin bypass for experimental workflows"

#### **Option B: Add Team/Organization Bypass**
- **Actor type**: "Organization admin" or "Team"
- **Bypass mode**: "Pull request"
- **Reason**: "Experimental workflow bypass"

### **Step 3: Modify Status Check Rule**
1. Find the "Require status checks to pass before merging" rule
2. Click "Edit" on that rule
3. **Add bypass conditions** (if available in UI)
4. Or **temporarily remove** the "Require Gemini AI Review" status check

### **Step 4: Save Changes**
Click "Save changes" to apply the ruleset modifications.

## 🚀 **API Method** (Advanced)

### **Current Ruleset Analysis**
```json
{
  "name": "Main protection with App bypass",
  "enforcement": "active",
  "bypass_actors": [
    {
      "actor_id": 1872207,
      "actor_type": "Integration", 
      "bypass_mode": "pull_request"
    }
  ],
  "rules": [
    {
      "type": "required_status_checks",
      "parameters": {
        "required_status_checks": [
          {"context": "Require Gemini AI Review / Check for 'ai-review' label"}
        ]
      }
    }
  ]
}
```

### **Modified Ruleset with User Bypass**
```bash
# Get your user ID
USER_ID=$(gh api user --jq .id)
echo "Your user ID: $USER_ID"

# Create modified ruleset JSON
cat > /tmp/modified_ruleset.json << EOF
{
  "name": "Main protection with App bypass",
  "enforcement": "active",
  "target": "branch",
  "conditions": {
    "ref_name": {
      "include": ["~DEFAULT_BRANCH"],
      "exclude": []
    }
  },
  "bypass_actors": [
    {
      "actor_id": 1872207,
      "actor_type": "Integration",
      "bypass_mode": "pull_request"
    },
    {
      "actor_id": $USER_ID,
      "actor_type": "User", 
      "bypass_mode": "pull_request"
    }
  ],
  "rules": [
    {"type": "deletion"},
    {"type": "non_fast_forward"},
    {
      "type": "pull_request",
      "parameters": {
        "allowed_merge_methods": ["merge", "squash", "rebase"],
        "automatic_copilot_code_review_enabled": false,
        "dismiss_stale_reviews_on_push": false,
        "require_code_owner_review": false,
        "require_last_push_approval": false,
        "required_approving_review_count": 1,
        "required_review_thread_resolution": false
      }
    },
    {
      "type": "required_status_checks",
      "parameters": {
        "do_not_enforce_on_create": false,
        "required_status_checks": [
          {"context": "Require Gemini AI Review / Check for 'ai-review' label"}
        ],
        "strict_required_status_checks_policy": false
      }
    }
  ]
}
EOF

# Apply the modified ruleset
gh api -X PUT repos/GiantCroissant-Lunar/dungeon-coding-agent/rulesets/7784048 \
  --input /tmp/modified_ruleset.json
```

## 🔧 **Alternative: Temporary Bypass**

### **Remove AI Review Requirement Temporarily**
```bash
# Create ruleset WITHOUT the AI review status check
cat > /tmp/temp_ruleset.json << EOF
{
  "name": "Main protection with App bypass",
  "enforcement": "active",
  "target": "branch", 
  "conditions": {
    "ref_name": {
      "include": ["~DEFAULT_BRANCH"],
      "exclude": []
    }
  },
  "bypass_actors": [
    {
      "actor_id": 1872207,
      "actor_type": "Integration",
      "bypass_mode": "pull_request"
    }
  ],
  "rules": [
    {"type": "deletion"},
    {"type": "non_fast_forward"},
    {
      "type": "pull_request",
      "parameters": {
        "allowed_merge_methods": ["merge", "squash", "rebase"],
        "automatic_copilot_code_review_enabled": false,
        "dismiss_stale_reviews_on_push": false,
        "require_code_owner_review": false,
        "require_last_push_approval": false,
        "required_approving_review_count": 1,
        "required_review_thread_resolution": false
      }
    }
  ]
}
EOF

# Apply temporarily (REMOVE AI review requirement)
gh api -X PUT repos/GiantCroissant-Lunar/dungeon-coding-agent/rulesets/7784048 \
  --input /tmp/temp_ruleset.json

# Merge your PR
gh pr merge 59 --squash

# Restore original ruleset with AI review
gh api -X PUT repos/GiantCroissant-Lunar/dungeon-coding-agent/rulesets/7784048 \
  --input /tmp/current_ruleset.json
```

## 🎯 **Recommended Approach**

### **For Immediate PR Merge**
1. **Use Web UI method** to add yourself as bypass actor
2. **Or use temporary bypass** to remove AI review requirement
3. **Merge PR #59**
4. **Restore ruleset** with improved bypass conditions

### **For Long-term Solution**  
1. **Add bypass actors** for admin users/teams
2. **Keep AI review requirement** for regular PRs
3. **Use bypass** for experimental/urgent work

## 🚀 **Quick Commands for Immediate Fix**

```bash
# Get your user ID
USER_ID=$(gh api user --jq .id)

# Add yourself as bypass actor (run this to modify ruleset)
# [See API method above for complete command]

# Then merge your PR
gh pr merge 59 --admin --squash
```

## ✅ **Verification**

After modification:
```bash
# Check if ruleset was updated
gh api repos/GiantCroissant-Lunar/dungeon-coding-agent/rulesets/7784048 --jq .bypass_actors

# Try merging with admin bypass
gh pr merge 59 --admin --squash
```

---

**The web UI method is safest - just add yourself as a bypass actor and you'll be able to merge experimental PRs without waiting for AI review!** 🚀