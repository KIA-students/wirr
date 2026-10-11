# Misje WiRR: cztery bloki i lokalna karta sesji (wersja robocza)

Nowe okno **WiRR → Misje i odkrycia (4 bloki)** to uproszczony pulpit pracy z czterema blokami (Lab 01–02, 03–04, 05–06, 07–08). Jest dodatkiem do istniejącego **WiRR Reports**, a nie nowym automatycznym systemem oceniania.

### Co jest automatyczne
- odczyt wersji Unity, systemu operacyjnego i informacji o GPU;
- wykrywanie zapisanych scen w Assets/WiRR/LabNN;
- wykrywanie istniejących lokalnych eksportów formularza WiRR Reports dla laboratoriów 01–07;
- przeniesienie do **lokalnego Markdown** wybranych wartości z wcześniej zapisanych pól tabel;
- zapis daty i liczby zdarzeń oraz czasu Play Mode z eksportu, o ile te dane istnieją.

### Czego pakiet nie udaje
- nie stwierdza samodzielnie, że rzeczywisty Quest, telefon AR czy ROS działały poprawnie;
- nie mierzy w trakcie eksportu FPS, dryfu, RTT ani komfortu;
- nie określa ostatecznej oceny, nie przyznaje bonusu za czas pracy;
- nie wysyła nic do GitHub, nie wykonuje GitHub Actions ani nie gromadzi danych osobowych w karcie sesji.

Student wybiera jawnie tryb REAL/SIM/REPLAY i dopisuje jedno zdanie odkrycia, diagnozy oraz nowego zastosowania. Raport zapisuje się tam, gdzie wskaże użytkownik w standardowym oknie wyboru pliku, także na Ubuntu.

**Lab 08:** nowy blok nie wymaga w wersji roboczej modyfikowania istniejących siedmiu rubryk; okno wykrywa scenę, lecz uczciwie informuje, że obecny JSON formularza nie obsługuje Lab 08.

## Przed wydaniem produkcyjnym
- uruchomić okno w czystym Unity 6000.6.x na Windows i Ubuntu;
- przetestować zapis Markdown i odczyt przykładowych JSON;
- sprawdzić, że pobieranie danych nie wymaga Internetu ani tokenów;
- potwierdzić, że nie ma błędów kompilacji C# w Editor;
- opublikować razem z nowymi instrukcjami WiRR z repozytorium MatPomGit/prz.

Do czasu tego testu jest to funkcja **gałęzi rozwojowej**, nie potwierdzona funkcja wydania produkcyjnego.
