# RFC012: Configuration & Seed Management

## 📋 Metadata
- Status: 📝 Draft
- Author: Architecture Team
- Created: 2025-08-31
- Dependencies: RFC001 (Core Loop), RFC003 (Map - read)

## 🎯 Objective
Provide a unified configuration layer and deterministic run seeding so testers and agents can reproduce worlds and tune gameplay parameters without code changes.

## 📖 Problem Statement
We need consistent configuration for TPS, viewport size, colors, difficulty, and RNG seed, with CLI overrides for quick testing. Deterministic seeds must reproduce the same map and event sequence.

## 🏗️ Technical Specification
- Config Sources (priority order)
  1. CLI flags (e.g., `--seed`, `--tps`, `--viewport 80x25`, `--theme dark`)
  2. User config file in repo (e.g., `config.json` or `appsettings.json`)
  3. Defaults baked into app
- Implementation
  - Strongly-typed `GameConfig` record
  - Parsing for `WIDTHxHEIGHT` viewport; validation with helpful errors
  - RNG service seeded from config; expose `IRandom` for systems
  - Persistence: write current config on exit (optional)
- Sample Keys
  - `tps` (int), `fps` (int), `viewport` (WxH), `seed` (int/string), `theme` (enum), `difficulty` (enum)

## 🖥️ CLI Examples
```
./game --seed 12345 --viewport 100x30 --tps 30 --theme high-contrast
./game --difficulty easy
```

## 🔗 Integration Points
- RFC003 map generation uses `IRandom`
- RFC005/007 consume `IRandom` for rolls and behavior
- RFC002 reads UI theme and viewport

## ✅ Acceptance Criteria
- [ ] Launching with `--seed 12345` reproduces identical map twice
- [ ] CLI flags override file config; invalid flags show clear error/help
- [ ] Viewport parsed and applied to UI layout
- [ ] Theme applied at startup; persisted if changed in-app (optional)
- [ ] Unit tests cover parsing, precedence, and deterministic RNG

## 🧪 Testing Strategy
- Golden tests for config precedence
- Determinism tests across two runs with same seed

## 📊 Performance Considerations
- None significant; config parsing at startup only
