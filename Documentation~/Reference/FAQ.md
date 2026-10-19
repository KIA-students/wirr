# FAQ — Najczęstsze Pytania i Odpowiedzi

## Ogólne

### P1: Czy mogę zaliczyć kurs bez wyższych ocen?
**Odpowiedź:** Tak. punkt kontrolny 3.0 na każdym Lab = zaliczenie. Oceny 3.5–5.0 są opcjonalne. Możesz:
- Wziąć ocenę 3.0 i przejść do Lab X+1
- Wrócić na Lab X po zajęciach i spróbować 3.5+

### P2: Ile czasu zajmie cały kurs?
**Odpowiedź:** Zakres podstawowy odpowiada punktowi kontrolnemu 3.0 na każdym laboratorium; wyższe oceny wymagają wykonania dodatkowych zadań i pomiarów.

### P3: Czy mogę pracować sam, bez pary?
**Odpowiedź:** Możesz współpracować przy stanowisku, ale raport i ocenę przygotowujesz indywidualnie, według własnego numeru indeksu.

### P4: Co jeśli urządzenie (Quest/Android) mi nie działa?
**Odpowiedź:** 
- Lab 1: Możesz pracować w simulatorze
- Lab 2: Możesz pracować w simulatorze
- Lab 3–4: Wymaga Android/iOS ARCore/ARKit. Jeśli nie masz, skontaktuj się z prowadzącym
- Lab 5–6: Możesz pracować na desktop'ie
- Lab 7: Możesz testować te Lab, które masz

---

## Technika & Unity

### P5: Jaka wersja Unity?
**Odpowiedź:** Kursowa wersja to Unity 6000.6.5f1 lub nowsza zgodna wersja z linii 6000.6.5f1. Zapisz rzeczywistą wersję w raporcie.

### P6: Czy mogę używać Unity 2023/2024?
**Odpowiedź:** Użyj wersji kursowej z P5. Przed migracją istniejącego projektu zachowaj jego kopię i sprawdź zgodność pakietów.

### P7: Pakiety — które zainstalować?
**Odpowiedź:** Użyj [konfiguracji środowiska](ENVIRONMENT_SETUP.md) i manifestu `Project~/Packages/manifest.json`. Nie dobieraj starszych wersji z archiwalnych poradników.

### P8: Co jeśli pakiet nie zainstaluje się?
**Odpowiedź:** Zachowaj kopię projektu i pliki `Packages/manifest.json` oraz `Packages/packages-lock.json`. Sprawdź dostęp do repozytorium, adres UPM i pierwszy błąd w Console. Nie usuwaj katalogu Packages. Ewentualną odbudowę pamięci podręcznej Library wykonaj po zamknięciu Unity.

### P9: Konsola wyświetla RED error
**Odpowiedź:** Przeczytaj error — zwykle mówi dokładnie, co nie tak:
- `Missing reference` → brakuje referencji w Inspectorze
- `NullReferenceException` → skrypt próbuje użyć null obiektu
- `Assembly definition version...` → sprawdź wersje zależności i pierwszy błąd kompilacji; zachowaj kopię manifestu i lockfile

### P10: Profiler nie pokazuje danych
**Odpowiedź:** 
```
1. Window → Analysis → Profiler (otwórz Profiler)
2. Tab: CPU Performance
3. Play Mode: uruchom grę
4. Profiler powinien zbierać dane live
```

---

## Git & Repozytorium

### P11: Jak klonować repo?
**Odpowiedź:** Przeczytaj `REPOSITORY_SETUP.md` — tam jest krok po kroku.

### P12: Nie mogę pushować na origin
**Odpowiedź:** Sprawdź:
```bash
# 1. Czy jesteś na gałęzi, nie na main?
git branch  # → powinieneś być na lab01-work

# 2. Czy masz dostęp do repozytorium?
git remote -v  # → pokaż URL

# 3. Spróbuj:
git push --set-upstream origin lab01-work
```

### P13: Wgrałem Library/ — co teraz?
**Odpowiedź:** Usuń pamięć podręczną z indeksu Git, zachowując pliki lokalnie: `git rm -r --cached -- Library`. Dodaj `/Library/` do `.gitignore`, zatwierdź poprawkę i wykonaj zwykły push. Nie przepisuj historii współdzielonej gałęzi.

### P14: Merge conflict — co robić?
**Odpowiedź:** Nie wchodź w panikę!
```bash
git status  # Pokaż pliki w konflikcie
# Edytuj każdy plik, usuń <<<<<<, ======, >>>>>>
# Zachowaj to, co potrzebujesz
git add <plik>
git commit -m "resolve: merge conflict"
git push
```

### P15: Nie pamiętam swojego numeru indeksu
**Odpowiedź:** Przeczytaj mail potwierdzający rejestrację albo zapytaj sekretariat.

---

## Ocenianie i punkty kontrolne

### P16: Co to jest punkt kontrolny 3.0?
**Odpowiedź:** Minimalny kompletny zakres danego laboratorium. Szczegółowe kryteria zawierają aktualna instrukcja i [matryca](GRADING_MATRIX.md); wymagania nie są identyczne we wszystkich laboratoriach.

### P17: Jeśli nie przejdę punkt kontrolny 3.0?
**Odpowiedź:** Ustal termin i zakres poprawy z prowadzącym. Nie zakładaj, że inne laboratorium zastępuje brakujące zaliczenie.

### P18: Czy mogę wziąć ocenę 3.0 i pójść dalej?
**Odpowiedź:** Tak! Oceny 3.5+ są opcjonalne.

### P19: Ile czasu mam na poprawę do wyższej oceny?
**Odpowiedź:** Terminy popraw ustala prowadzący. Kampania obejmuje osiem laboratoriów w czterech blokach.

### P20: Czy mogę udzielić sobie samemu ocenę?
**Odpowiedź:** Nie. Narzędzie sprawdza kompletność raportu, a ocenę przyznaje prowadzący na podstawie dowodów i demonstracji.

---

## Lab-Specyficzne Problemy

### Lab 1

**P21: "XR Origin ma skalę (2, 2, 2), nie (1, 1, 1)"**
→ Sprawdź skalę Transform obiektu XR Origin oraz jego rodziców. Dla sceny bazowej ustaw (1, 1, 1) i zweryfikuj wymiar Cube_1m.

**P22: "FPS wynosi 20, to OK?"**
→ Na Quest powinno być >= 60 (90), na Android >= 30. Jeśli mniej, sprawdź Draw Calls i shader cost.

### Lab 2

**P23: "Chwyt nie działa"**
→ Sprawdzenie checklist:
```
✓ XR Direct Interactor ma się znajdować na dłoni (w XR Hands)
✓ Module_A ma Collider (nie Trigger)
✓ Module_A ma Rigidbody (Is Kinematic = false)
✓ Layer nie jest ignorowany w Ray Mask
```

**P24: "Ray wybiera blat zamiast UI"**
→ Dodaj Layer "Environment" dla blacie, exclude w Ray Mask dla panelu.

### Lab 3

**P25: "ARPlaneManager nie skanuje"**
→ Sprawdzenia:
```
✓ Czy telefon ma ARCore (wpisz model w DEVICE_SUPPORT_URL)
✓ Czy masz uprawnienie do kamery
✓ Czy oświetlenie jest wystarczające (ARCore wymaga >= 50 lux)
```

**P26: "Błąd rejestracji wynosi 10cm, to OK?"**
→ Zależy od Lab 3 wymagań. Sprawdź instrukcję — zwykle < 5cm jest PASS.

### Lab 4

**P27: „API głębi (Depth API) zwraca `null`”**
→ Sprawdzenia:
```
✓ Urządzenie obsługuje Depth API
✓ ARCore >= 1.40
✓ Edit → Project Settings → ARCore → Enable Depth API = true
```

**P28: "Model nie jest okluzowany"**
→ Sprawdzenia:
```
✓ Depth texture musi być przypisana do shadera
✓ Shader musi korzystać z texture (depth gradient na Z)
✓ Model musi mieć Mesh Renderer
```

### Lab 5

**P29: "LOD nie przełącza się"**
→ Sprawdzenia:
```
✓ LOD Group.enabled = true
✓ Kamera się oddala (aby wyzwolić LOD zmianę)
✓ Inspect → LOD Group → sprawdź threshold (zazwyczaj 0.5)
```

**P30: "Poligony nie zmniejszają się między LOD0/1"**
→ Sprawdź Import Settings → Model → LOD Group. Może nie zostały wygenerowane.

### Lab 6

**P31: "ROS 2 topik nie widoczny"**
→ Sprawdzenia:
```bash
# Terminal:
ros2 node list           # Czy uruchomiono źródło i ROS-TCP Endpoint?
ros2 topic list          # Czy /joint_states jest publikowany?
ros2 topic echo /joint_states  # Czy dane płyną?
```

**P32: "Latency > 500ms"**
→ Przyczyny:
- Słaba sieć (sprawdź ping)
- ROS 2 sub jest spowolniony (check: ros2 doctor)
- Brak buforowania (sprawdź ring buffer w kodzie)

### Lab 7

**P33: "Matryca testów — wiele FAIL**
→ Porównaj wyniki z kryteriami akceptacji z Lab 07. Każdy FAIL wymaga wyjaśnienia; nie ma ogólnego progu 70% zastępującego kryteria instrukcji.

**P34: "FPS pada w stress teście"**
→ Przyczyny (w kolejności):
1. Wyciek pamięci (Memory Profiler)
2. Zbyt złożony shader (Frame Debugger)
3. Zbyt dużo Draw Calls (Profiler → CPU)

---

## Nauka & wnioski

### P35: Jak napisać dobrą diagnozę?
**Odpowiedź:** Schemat 5 kroków:
1. **Objaw:** Co dokładnie obserwujesz? (np. "FPS pada z 90 do 20")
2. **Hipoteza:** Dlaczego tak się dzieje? (np. "Shader cost")
3. **Test:** Jak to sprawdzić? (np. "Zmienić shader na prostszy")
4. **Przyczyna:** Co odkryłeś?
5. **Wynik:** Czy naprawiłeś?

### P36: Co wpisać w raporcie `evidence/lab0X.md`?
**Odpowiedź:** Użyj `Samples~/LabXX/report-template.md` lub formularza WiRR. Wpisz własny indeks, warianty, dane surowe, obliczenia i wnioski wymagane dla wybranego punktu kontrolnego. Wyślij raport zgodnie z [procedurą](STUDENT_SUBMISSIONS.md).

### P37: "Moja diagnoza mówi: 'No idea'"
**Odpowiedź:** To nie wystarczy. Spróbuj:
1. Czytać error message
2. Zalogować wartości (`Debug.Log`)
3. Wyłączyć komponenty jeden po drugim (binary search)
4. Porównać z działającą wersją kogoś innego

---

## Kontakt & Wsparcie

### P38: Gdzie mogę znaleźć prowadzącego?
**Odpowiedź:** 
- Zajęcia: osobiście
- Mail: (email prowadzącego — w sylabusie)
- Dyżury: (sprawdź plan dyżurów na stronie KIA)

### P39: Czy są godziny dodatkowe?
**Odpowiedź:** Sprawdź aktualne dyżury i ustal termin z prowadzącym.

### P40: Nie rozumiem materiału
**Odpowiedź:**
1. Przeczytaj ponownie instrukcję Lab
2. Obejrzyj demo prowadzącego
3. Zapytaj kolegę z pary
4. Zapytaj prowadzącego na dyżurach

---

## Ostatnie Słowo

**Pamiętaj:**
- Kurs nie karze Ciebie, ale uczy Cię
- punkt kontrolny 3.0 jest całkowicie osiągalny
- Jeśli utkniesz, pytaj — prowadzący powinien pomagać, nie karać
- Git i debugging będą Ci potrzebne w pracy — to jest cenne doświadczenie

**Powodzenia!**

