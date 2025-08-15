# EpochsOfWar Performance Analysis Report

## Executive Summary

This report documents performance inefficiencies identified in the EpochsOfWar Unity RTS codebase. The analysis focused on common Unity performance bottlenecks including memory allocations, expensive operations in Update loops, and inefficient resource management.

## Critical Performance Issues Found

### 1. Material Creation in Update Loop (CRITICAL)
**File:** `Assets/Scripts/System/BuildingPlacer.cs:91-94`
**Severity:** High
**Impact:** Frame rate drops during building placement mode

**Problem:**
```csharp
var material = new Material(previewMaterial);
material.color = previewColor;
renderer.material = material;
```

**Issue:** Creates new Material objects every frame during building preview, causing garbage collection pressure and memory allocations.

**Solution:** Cache valid and invalid preview materials as class fields and reuse them.

**Status:** ✅ FIXED in this PR

### 2. Input Device Polling Every Frame (HIGH)
**Files:** 
- `Assets/Scripts/System/CameraController.cs:47`
- `Assets/Scripts/System/GameManager.cs:158`

**Problem:** Accessing `Keyboard.current` and `Mouse.current` every frame in Update methods.

**Impact:** Unnecessary overhead from input system queries.

**Recommendation:** Cache input device references or use event-based input handling.

### 3. Expensive Scene Searches During Initialization (MEDIUM)
**File:** `Assets/Scripts/System/GameManager.cs:50,53`

**Problem:**
```csharp
if (mapManager == null)
    mapManager = FindFirstObjectByType<MapManager>();
if (terrainManager == null)
    terrainManager = FindFirstObjectByType<TerrainManager>();
```

**Impact:** `FindFirstObjectByType` is expensive and should be avoided in frequently called methods.

**Recommendation:** Use direct references or singleton patterns for manager classes.

### 4. String Concatenation with + Operator (MEDIUM)
**File:** `Assets/Scripts/UI/SkirmishSetup.cs:127`

**Problem:**
```csharp
string mapInfo = $"Map: {gameSettings.mapName}\\nMax Players: {gameSettings.maxPlayers}\\nSpawn Points: Random";
```

**Impact:** String concatenation creates temporary objects and garbage.

**Recommendation:** Use StringBuilder for complex string building or optimize string interpolation.

### 5. Synchronous Destruction Calls (MEDIUM)
**Files:**
- `Assets/Scripts/UI/SkirmishSetup.cs:91`
- `Assets/Scripts/System/TerrainManager.cs:33`

**Problem:** Using `DestroyImmediate()` instead of `Destroy()`.

**Impact:** Synchronous destruction can cause frame hitches.

**Recommendation:** Use `Destroy()` for deferred destruction unless immediate destruction is required.

### 6. Excessive GameObject Creation for Grid (LOW-MEDIUM)
**File:** `Assets/Scripts/System/TerrainManager.cs:43-56`

**Problem:** Creates 101 GameObjects with LineRenderer components for a 50x50 grid (51 vertical + 51 horizontal lines).

**Impact:** High GameObject count and draw calls for visual grid.

**Recommendation:** Use a single mesh or GPU instancing for grid rendering.

## Performance Optimization Recommendations

### Immediate Actions (High Priority)
1. ✅ **Fix material creation in BuildingPlacer** - Implemented in this PR
2. Cache input device references in CameraController and GameManager
3. Replace FindFirstObjectByType calls with direct references

### Short-term Actions (Medium Priority)
4. Replace DestroyImmediate with Destroy where appropriate
5. Optimize string operations in UI updates
6. Implement object pooling for frequently created/destroyed objects

### Long-term Actions (Low Priority)
7. Optimize grid rendering system with mesh-based approach
8. Implement LOD system for buildings and units as game scales
9. Add performance profiling hooks for monitoring

## Testing Recommendations

### Performance Testing
- Use Unity Profiler to measure frame time improvements
- Monitor garbage collection frequency during building placement
- Test with multiple buildings being placed rapidly

### Functional Testing
- Verify building placement visual feedback works correctly
- Test all input methods (keyboard, mouse, touch)
- Ensure no regression in existing gameplay features

## Implementation Notes

The material caching fix in BuildingPlacer follows Unity best practices:
- Materials are created once during initialization
- Cached materials are reused instead of creating new ones each frame
- Proper cleanup prevents memory leaks
- Visual behavior remains identical to original implementation

## Metrics

**Before Fix:**
- New Material objects created every frame during building preview
- Potential for 60+ allocations per second at 60 FPS

**After Fix:**
- Zero material allocations during building preview
- Cached materials reused for entire session
- Proper memory cleanup on component destruction

## Conclusion

The identified performance issues range from critical frame-rate impacting problems to minor optimizations. The material creation fix addresses the most severe issue and should provide immediate performance benefits during building placement gameplay.

Future optimization work should focus on the input polling and scene search issues as they affect the core game loop performance.

---
*Report generated: August 15, 2025*
*Analyzed codebase: EpochsOfWar Unity RTS*
*Focus: Unity-specific performance patterns and RTS scalability*
