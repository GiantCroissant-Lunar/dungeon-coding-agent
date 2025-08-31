# RFC014: UX Polish & Accessibility

## 📋 Metadata
- Status: 📝 Draft
- Author: Architecture Team
- Created: 2025-08-31
- Dependencies: RFC002 (Terminal Shell), RFC001 (Core Loop - read)

## 🎯 Objective
Improve usability and accessibility: theme support, high-contrast palette, resize-safe layouts, help overlay, and input discoverability, all within Terminal.Gui v2.

## 📖 Problem Statement
The terminal UI should be readable and navigable across environments. Users need quick help and the ability to toggle themes, with layouts resilient to terminal resizes.

## 🏗️ Technical Specification
- Theming
  - Provide at least: Default, Dark, High-Contrast
  - Theme applied to views (map, sidebars, status bar, message log)
  - Persist selection via RFC012 config layer
- Layout & Resize
  - Use `Terminal.Gui` layouting to keep map centered and side panels sized within viewport
  - On window resize, recompute layout without overlap
- Help Overlay
  - `[F1]` toggles a modal listing key bindings and agent-friendly commands
- Key Bindings & Discoverability
  - Show a top status strip with hotkeys: `F1 Help | F2 Theme | I Inventory | S Save | Q Quit`
  - Input rebinding deferred; out-of-scope for now

## 🖼️ UI Mockups
Top strip and layout:
```
┌──────────────────────────────────────────────────────────────────┐
│ F1 Help | F2 Theme | I Inventory | S Save | Q Quit               │
├───────────────┬──────────────────────────────────────────────────┤
│ Stats/Status  │ Map (resizes with window)                        │
│ HP: 10/10     │                                                  │
│ Depth: 1      │                                                  │
├───────────────┴──────────────────────────────────────────────────┤
│ Message Log (latest first)                                       │
└──────────────────────────────────────────────────────────────────┘
```

## 🔗 Integration Points
- RFC012 persists theme choice and reads on startup
- RFC010 health/status bar adopts theme tokens
- RFC007 message log adopts theme tokens

## ✅ Acceptance Criteria
- [ ] Theme toggle works (Default, Dark, High-Contrast); persists between runs
- [ ] Layout remains correct after terminal resize (no clipped/overlapping views)
- [ ] F1 opens a help overlay with all active key bindings
- [ ] Status strip displays key hints; keys function
- [ ] Unit tests for theme selection persistence; manual test script for resize

## 🧪 Testing Strategy
- Snapshot screenshots via CI artifact (optional)
- Automated check on config persistence (read/write round-trip)

## 📊 Performance Considerations
- Minimal; theme swap should not trigger full redraw storms—batch updates where possible
