# Indeks plików WiRR Lab 1–7

## Struktura Pakietu

```
/mnt/user-data/outputs/
│
├── 📄 DOWNLOAD_SUMMARY.txt              ← CZYTAJ PIERWSZY
├── 📄 Laboratorium_1_XR_Pełna_Instrukcja.docx
├── 📄 Laboratorium_2_XR_Pełna_Instrukcja.docx
├── 📄 Laboratorium_3_XR_Pełna_Instrukcja.docx
├── 📄 Laboratorium_4_XR_Pełna_Instrukcja.docx
├── 📄 Laboratorium_5_XR_Pełna_Instrukcja.docx
├── 📄 Laboratorium_6_XR_Pełna_Instrukcja.docx
├── 📄 Laboratorium_7_XR_Pełna_Instrukcja.docx
│
└── starter-files/
    ├── 📄 QUICK_START.md                ← CZYTAJ DRUGIE
    ├── 📄 REPOSITORY_SETUP.md
    ├── 📄 INSTRUCTOR_GUIDE.md
    ├── 📄 FAQ.md
    ├── 📄 GRADING_MATRIX.md
    ├── 📄 GLOSSARY.md
    ├── 📄 INDEX.md                      ← JESTEŚ TUTAJ
    │
    ├── 📄 LAB1_STARTER_README.md
    ├── 📄 LAB2_STARTER_README.md
    ├── 📄 LAB3_STARTER_README.md
    ├── 📄 LAB4_STARTER_README.md
    ├── 📄 LAB5_STARTER_README.md
    ├── 📄 LAB6_STARTER_README.md
    ├── 📄 LAB7_STARTER_README.md
    │
    ├── 📄 evidence_template_lab01.md
    ├── 📄 evidence_template_lab02.md
    ├── 📄 evidence_template_lab03.md
    ├── 📄 evidence_template_lab04.md
    ├── 📄 evidence_template_lab05.md
    ├── 📄 evidence_template_lab06.md
    ├── 📄 evidence_template_lab07.md
    │
    └── cs-snippets/
        ├── 📄 Lab01_PerformanceBenchmark.cs
        ├── 📄 Lab02_Trial.cs
        ├── 📄 Lab03_RegistrationError.cs
        ├── 📄 Lab04_LightingController.cs
        ├── 📄 Lab05_PerformanceBenchmark.cs
        ├── 📄 Lab06_ROS2Manager.cs
        ├── 📄 Lab06_RobotStateListener.cs
        └── 📄 Lab07_TestRunner.cs
```

---

## Jak nawigować po materiałach

### Jeśli jesteś **STUDENTEM**:

1. **Przeczytaj najpierw:**
   ```
   QUICK_START.md                 (orientacja)
   REPOSITORY_SETUP.md            (git workflow)
   GLOSSARY.md                    (słownictwo)
   ```

2. **Przed każdym Lab X:**
   ```
   LAB<X>_STARTER_README.md       (co pobrać, checklist)
   Laboratorium_<X>_..docx        (pełna instrukcja — przeczytaj całą!)
   evidence_template_lab0X.md     (do wklejenia w repo)
   ```

3. **Podczas Lab X:**
   - Otwórz instrukcję .docx (główne źródło)
   - Kopia template raportu: `cp evidence_template_lab0X.md evidence/lab0X.md`
   - Jeśli utkniesz: przeczytaj FAQ.md lub zapytaj prowadzącego

4. **Po Lab X:**
   - Wypeł evidence/lab0X.md
   - Git push
   - Czekaj na recenzję prowadzącego

### Jeśli jesteś **PROWADZĄCYM**:

1. **Przeczytaj najpierw:**
   ```
   INSTRUCTOR_GUIDE.md            (ogólna strategia)
   GRADING_MATRIX.md              (jak oceniać szybko)
   FAQ.md                         (typowe problemy)
   GLOSSARY.md                    (by rozumieć studentów)
   ```

2. **Przed każdym Lab X:**
   ```
   Laboratorium_<X>_..docx        (przeczytaj całą instrukcję)
   GRADING_MATRIX.md → sekcja Lab X (przygotuj checklist)
   ```

3. **Podczas Lab X:**
   - Prowadź zajęcia zgodnie z .docx
   - Oceniaj wg GRADING_MATRIX.md (max 90 sekund/parę)
   - Znotuj oceny
   - Archiwizuj evidence/lab0X.md od każdej pary

4. **Po Lab X:**
   - Merge'uj gałęzie do main (Quality Gate)
   - Ustaw nową gałąź startową dla Lab X+1
   - Wyślij feedb feedback

### Szybkie Odnośniki:

**Dla studentów:**
- Pytanie techniczne? → FAQ.md
- Nie wiem co to znaczy? → GLOSSARY.md
- Jak Git? → REPOSITORY_SETUP.md
- Jak oceniam? → GRADING_MATRIX.md

**Dla prowadzącego:**
- Jak prowadzić Lab X? → Laboratorium_X..docx
- Jak ocenić szybko? → GRADING_MATRIX.md + max 90 s
- Student ma problem? → INSTRUCTOR_GUIDE.md (troubleshooting)
- Jak zaplanować kurs? → INSTRUCTOR_GUIDE.md (timeline)

---

## Wersje plików

Wszystkie pliki: **Wersja 2.0, gotowa do wdrożenia, 13 września 2026 r.**

| Plik | Typ | Rozmiar | Przeznaczenie |
|------|-----|---------|---------------|
| .docx | Instrukcje | ~45–58 KB każdy | Studenci + Prowadzący (czytaj na zajęciach) |
| README.md | Szybki start | ~2–3 KB | Studenci (czytaj PRZED zajęciami) |
| evidence_template.md | Raport | ~2 KB | Studenci (kopuj do repo) |
| cs-snippets/*.cs | Kod | ~1–2 KB | Studenci (wklej do Unity) |
| GUIDE.md / FAQ.md / MATRIX.md | Dokumentacja | ~5–10 KB | Prowadzący (przygotowanie) |
| GLOSSARY.md | Słownik | ~8 KB | Wszyscy (gdy nie rozumiesz terminu) |

---

## Ścieżka uczenia (zależności między labami)

```
Lab 1 (Fundament: XR Origin, Profiler)
    ↓ (zależy od)
Lab 2 (Interakcja: Chwyt, UI)
    ↓ (niezależy od 1–2, ale):
Lab 3 (AR: Rejestracja)
    ↓
Lab 4 (AR: Mieszanie realności)
    ↓
Lab 5 (Optymalizacja CAD)  ← Niezależy od Lab 3–4, ale razem z Lab 6
    ↓
Lab 6 (ROS 2: Bliźniak cyfrowy)
    ↓ (integruje wszystkie)
Lab 7 (Walidacja: Smoke + Functional + Performance)
    ↓
Zaliczenie kursu
```

**Legenda:**
- Lab 1–2: VR (Quest 3)
- Lab 3–4: AR (Android)
- Lab 5–6: Mixed
- Lab 7: Wszystko razem

---

## Checklist pobierania

### Dla Studenta

- [ ] Pobierz QUICK_START.md
- [ ] Pobierz REPOSITORY_SETUP.md
- [ ] Pobierz GLOSSARY.md
- [ ] Pobierz LAB1_STARTER_README.md
- [ ] Pobierz Laboratorium_1_..docx
- [ ] Pobierz evidence_template_lab01.md
- [ ] (Powielaj dla Lab 2–7)

### Dla Prowadzącego

- [ ] Pobierz INSTRUCTOR_GUIDE.md
- [ ] Pobierz GRADING_MATRIX.md
- [ ] Pobierz FAQ.md (dla wiedzy)
- [ ] Pobierz wszystkie Laboratorium_*.docx
- [ ] Przygotuj checklist oceny (print!)
- [ ] (Opcjonalnie) GLOSSARY.md dla zrozumienia studentów

---

## Wsparcie

- **Błąd w pliku?** → Kontakt z prowadzącym
- **Nie wiem gdzie znaleźć plik?** → Ten INDEX.md
- **Pytanie techniczne?** → FAQ.md
- **Nie rozumiem terminu?** → GLOSSARY.md

---

**Wersja:** 2.0 • 13 września 2026 r.  
**Data generacji:** 8 września 2026 r.

