# Instrukcja dla Prowadzącego — WiRR (Lab 1–7)

## Szybki Przegląd

**Kurs:** Wirtualna i Rozszerzona Rzeczywistość (studia II stopnia)  
**Czas:** ~15–18 godzin (2–3 tygodnie, ~6h/tydzień)  
**Format:** 7 laboratoriów, każde z systemem checkpointów (3.0–5.0)  
**Zaliczenie:** Checkpoint 3.0 na każdym laboratorium  
**Ocena:** 3.0–5.0 w zależności od poziomu zaawansowania  

---

## Timeline Kursu

```
Tydzień 1:
  - Lab 1 (100 min) + Lab 2 (90 min) = 190 min ≈ 3.5h

Tydzień 2:
  - Lab 3 (100 min) + Lab 4 (80 min) = 180 min ≈ 3h

Tydzień 3:
  - Lab 5 (80 min) + Lab 6 (80 min) = 160 min ≈ 2.7h

Tydzień 4:
  - Lab 7 (120 min) = 2h

Razem: ~11 godzin czystego czasu + 4–6h na debug/prace domowe
```

---

## Jak oceniać checkpointy

### Checkpoint 3.0 (Zaliczenie)

**Czas oceny:** max 90 sekund (demonstracja)

**Kryteria:**
1. ✓ Smoke test — czy system startuje bez błędów krytycznych?
2. ✓ Checkpoint 3.0 procedury — czy wszystkie wymagane kroki są ukończone?
3. ✓ Pomiary — czy dane są przynajmniej przybliżone (nie musimy być perfekcjonistami)?
4. ✓ Diagnoza — czy para znalazła błąd i wdrożyła poprawkę?
5. ✓ Evidence/lab0X.md — czy raport jest wypełniony?

**Werdykt:**
- >= 4/5 kryteriów → **PASS (3.0)**
- < 4/5 → **FAIL** (student musi poprawić do następnych zajęć)

---

### Checkpointy 3.5–5.0 (Wyższa ocena)

Te oceny są **opcjonalne** — student może wziąć ocenę 3.0 i przejść dalej.

Jeśli chce wyższą:
- **3.5:** Dodatkowe komponenty działają, efekty vizualne
- **4.0:** Pomiary + analiza, porównania metod
- **4.5:** Eksperymenty, optymalizacja, porównanie wielometodowe
- **5.0:** Automatyzacja, eksploracja granic, raport produkcyjny

**Szacunkowy czas:** +30–60 min pracy domowej na checkpoint

---

## 🔍 Ocena — rzeczy do sprawdzenia na szybko

### Lab 1
```
Console: FPS zarejestrowany (każde 300 klatek log musi być)
Profiler: CPU i GPU time zmierzone
Audyt: liczby poligonów, materiałów
```

### Lab 2
```
Chwyt: Module_A reaguje na Direct Grab
Panel: Ray Interactor nie wybiera środowiska
Lokomocja: Snap turn 30° (nie 45°!)
Czasy: 3 próby pomiarowe z medianą
```

### Lab 3
```
AR: ARPlaneManager wykrywa płaszczyzny
Rejestracja: Punkt O i X zaznaczone
Błąd: 5 pomiarów z różnych dystansów
Trend: czy błąd rośnie czy zostaje stały?
```

### Lab 4
```
Depth API: dostępna na telefonie (log się pojawia?)
Okluzja: rzeczywisty obiekt zasłania model
Oświetlenie: zmienia się razem ze sceną
Wiarygodność: 5 testów, średnia ocena >= 2/5
```

### Lab 5
```
Import CAD: skalowanie (1,1,1)?
LOD Group: przełącza się widocznie?
Benchmark: tabela z poligonami, FPS
Przyrost: FPS zmienia się między LOD?
```

### Lab 6
```
ROS 2: ros2 node list pokazuje unity_listener
Topiki: wszystkie dostępne (terminal: ros2 topic echo)
Synchronizacja: model śledzi robota (demo)
Latency: zmierzony (3 próby)
```

### Lab 7
```
Smoke: Start bez RED errors
Matryca: >= 70% Lab 1–6 PASS
Wydajność: FPS, CPU, GPU, Memory zmierzone
Scenariusz: 3 wykonane, czasy zarejestrowane
```

---

## Typowe Problemy & Rozwiązania

### Lab 1
```
Problem: "Profiler nie pokazuje FPS"
→ Sprawdzić: Window → Analysis → Profiler, tab CPU Performance

Problem: "XR Origin ma skalę (2,2,2)"
→ Rozwiązanie: Scale Factor w Import Settings, nie transform
```

### Lab 2
```
Problem: "Chwyt nie działa"
→ Sprawdzić: czy XR Direct Interactor ma Collider na dłoni?

Problem: "Ray wybiera blat zamiast panelu"
→ Rozwiązanie: Raycast Mask musi exclude Environment layer
```

### Lab 3
```
Problem: "ARPlaneManager nie skanuje"
→ Sprawdzić: czy ARCore jest dostępna? (ros2 system requirements)

Problem: "Błąd rejestracji rośnie drastycznie"
→ Możliwe: słabe oświetlenie, brak tekstury, zła kalibracja IMU
```

### Lab 4
```
Problem: "Depth API zwraca null na Android"
→ Sprawdzić: czy urządzenie na liście obsługiwanych (https://developers.google.com/ar/devices)

Problem: "Model nigdy nie jest okluzowany"
→ Rozwiązanie: depth texture musi być przypisana do shadera
```

### Lab 5
```
Problem: "LOD nie przełącza się"
→ Sprawdzić: czy LOD Group.enabled = true? Czy kamera się oddala?

Problem: "Import FBX ma błąd skalowania"
→ Rozwiązanie: Edit → Project Settings → FBX → Scale Factor
```

### Lab 6
```
Problem: "Model nie zmienia pozycji"
→ Sprawdzić: czy topik /robot/state się publikuje? (ros2 topic echo)

Problem: "Latency > 500ms"
→ Możliwe: sieć, ROS 2 CPU spowolniony, brak buforowania
```

### Lab 7
```
Problem: "100% testów FAIL"
→ Sprawdzić: czy wszystkie Lab 1–6 były completed?

Problem: "FPS pada do 5 FPS w stress teście"
→ Możliwe: wyciek pamięci, brak LOD, zbyt złożony shader
```

---

## Macierz walidacji

Każde laboratorium ma swoją macierz. Przygotuj je przed zajęciami:

### Lab 1
```
[ ] XR Origin (0,0,0) scale (1,1,1)
[ ] Benchmark baseline zmierzony
[ ] Audyt urządzeń
[ ] FPS >= 60 (lub uzasadnienie)
[ ] Diagnoza + commit
```

### (... i tak dla Lab 2–7)

---

## Jak prowadzić zajęcia

### 5 minut przed Lab
1. Sprawdź, czy kod kompiluje się bez RED errors
2. Upewnij się, że repozytorium jest dostępne
3. Przygotuj checklist oceny (print albo na ekranie)

### Na początku Lab (5 min)
1. Wyjaśnij problem inżynierski
2. Pokaż, co będą robić (2–3 min demo)
3. Odpowiedź na pytania

### Podczas Lab (80–90 min)
1. Studenci pracują w parach
2. Krąż między stanowiskami, sprawdzaj postęp co 10–15 minut
3. Jeśli ktoś utknął: zadaj pytania, nie mów rozwiązania
4. Zanotuj czasy — kto robi szybko, kto ma problemy

### Na końcu Lab (15 min)
1. Każda para pokazuje ostatni commit
2. Krótka ocena Checkpoint 3.0 (< 2 min na parę)
3. Zebranie evidence/lab0X.md
4. Krótkie podsumowanie: co się nauczyliśmy?

---

## Jak sprawdzić wkład pary — ?

```bash
# Pokaż Stat dla każdej osoby
git log --author="Imię" lab0X...lab0X-start --oneline

# Jeśli:
- 3+ merytoryczne commity → wkład ok (50/50)
- 1 commit, reszta od drugiej osoby → może być problem

# Akcja: porozmawiaj z parą, czy można naprawić na następnych Lab
```

---

## Gotowość do produkcji (Lab 7)

Checkpoint 3.0 na Lab 7 oznacza, że system jest **gotowy do testów** (nie do pełnego wdrożenia).

Oczekuj:
- FPS >= 60 (VR) / >= 30 (AR)
- Latency < 200ms (Lab 6)
- Brak drastycznych wycieków pamięci
- >= 70% Lab 1–6 w pełni funkcjonalnych

---

## Dodatkowe zasoby

- **Dokumentacja:** https://docs.unity3d.com/
- **GitHub repozytorium:** (link do kursu)
- **Discord:** (link do kanału społeczności, jeśli jest)

---

**Wersja:** 2.0 • 13 września 2026 r.  
**Autora:** Dr inż. Mateusz Pomianek  
**Aktualizacja:** Na bieżąco z XR technology

