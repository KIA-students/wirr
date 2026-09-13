# Laboratorium 5 — Optymalizacja CAD

## 📦 Co Powinieneś Pobrać

- ✓ Projekt z Lab 1 (VR/Desktop)
- ✓ Model CAD (FBX lub 3DS Max/Blender)
- Blender albo 3DS Max (dla redukcji geometrii)

### Pliki
```
Assets/Models/
├── model_original.fbx
├── model_LOD0.fbx
└── model_LOD1.fbx
Assets/Scripts/PerformanceBenchmark.cs (z Lab 1, adapt.)
```

---

## ✅ Checklist

- [ ] Model CAD pobierz od prowadzącego
- [ ] Blender/3DS Max zainstalowany
- [ ] Profiler Unity otwarty
- [ ] Gałąź: `git switch -c team-<nr>/lab05-<nazwiska>`

---

## 🚀 Start Lab 5

1. Kopiuj model: `Assets/Models/model_original.fbx`
2. Sprawdź Import Settings: Scale Factor = 0.01 (jeśli cm)
3. Otwórz w Blenderze: Export → Reduce polygons (50%, 30%, 15%)
4. Zaexportuj jako model_LOD0/1/2.fbx
5. Przypisz w Inspectorze: LOD Group

---

## 📋 Skrypty do pobrania

Benchmark z Lab 1 — tylko zmień nazwę:
```
LAB05_BENCHMARK fps=... ms=... seed=...
```

---

**Razem: ~80 minut**
