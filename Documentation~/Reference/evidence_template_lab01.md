# Laboratorium 1 — raport wykonania 

## Identyfikacja pary
- Numer indeksu student 1: ___________
- Numer indeksu student 2: ___________
- Suma numerów: ___________
- Przypisany wariant diagnostyczny: ___ (A/B/C/D)

---

## Pomiar wydajności — wariant bazowy (oryginalny CAD)

| Próba | FPS | ms/klatka | Draw Calls |
|-------|-----|----------|-----------|
| 1     | __  | __       | __ |
| 2     | __  | __       | __ |
| 3     | __  | __       | __ |
| Mediana | __ | __ | __ |

**Kontekst:** Urządzenie: Quest 3 / Android / Symulator
**Sieć:** Localhost / LAN

---

## Audyt urządzeń (jeśli Lab 1 na sprzęcie)

### Quest 3 (jeśli dostępny)
- CPU: ___
- GPU: ___
- RAM: ___
- Refreszowanie: 90/120 Hz
- **Verdict:** Capable / Limited / Not suitable

### Android (jeśli testowany)
- Model: ___
- API Level: ___
- RAM: ___
- GPU: ___

---

## Diagnoza (wariant ___)

### Objaw
Opisz dokładnie, co obserwujesz:
```
Przykład: "FPS pada z 90 do 20, gdy wszystkie obiekty są widoczne"
```

### Hipotezy
1. **Hipoteza 1:** ___ 
   - Przewidywanie: ___
   
2. **Hipoteza 2:** ___
   - Przewidywanie: ___

### Test rozstrzygający
Jaki test rozróżni między hipotezami?
```
Przykład: "Wyłączyć wszystkie Draw Calls poza głównym modelem — czy FPS wraca do 90?"
```

### Przyczyna (wynik testu)
Co rzeczywiście się dzieje?
```
Przykład: "Shader cost, nie geometria — zmieniłem shader na uproszczony i FPS wrócił do 90"
```

### Wynik przed / po
| Metryka | Przed | Po | Status |
|---------|-------|----|----|
| FPS | __ | __ | FIXED / NOT FIXED |
| Draw Calls | __ | __ | - |
| ms/frame | __ | __ | - |

---

## Wnioski

1. (Zawsze wpisz coś) ___
2. (Coś do zapamiętania na później) ___

---

## Git — Commit message
```bash
git commit -m "fix(lab01): <wpisz przyczynę błędu tutaj>"
# Przykład: git commit -m "fix(lab01): reduced draw calls by optimizing shader"
```

---

## Załączniki (opcjonalnie)
- [ ] Zrzut ekranu Profilera (CPU, GPU, Memory)
- [ ] Log z Console (Copy & Paste)
- [ ] Zdjęcie stanowiska testowego

---

**Data:** ___________  
**Czas pracy:** ____ min  
**Status:** ☐ PASS (3.0) | ☐ Candidate (3.5+)

