# Liga XR: nazwa drużyny i misje poza zajęciami

**Ligę tworzą dwie grupy laboratoryjne.** Wspólne doświadczenia są najważniejsze; nie trzeba uczestniczyć w dodatkowej rywalizacji, aby zaliczyć laboratoria.

## Nazwij drużynę

Każda grupa może uzgodnić swoją nazwę podczas pierwszych zajęć. [Generator propozycji](https://kia-students.github.io/wirr/#liga) tworzy losowe, zabawne zestawienia technologiczne, np. **Kwantowe Kapibary**, **Rekurencyjne Pierogi**, **Neonowe Dziobaki** lub **Zrobotyzowane Wombaty**. Można użyć dowolnej innej kulturalnej nazwy.

Przycisk generatora działa **lokalnie w przeglądarce** i niczego nie publikuje. Grupowa propozycja trafia do prowadzącego (np. pokazana na zajęciach), który weryfikuje i zatwierdza nazwę. Dopiero zatwierdzenie w \`competition/scoreboard.json\` zmienia publiczny ranking. Studenci nie podają nazwisk ani numerów indeksów.

Prowadzący może wykonać lokalnie:

    python competition/set_team_name.py --group G1 --name "Kwantowe Kapibary"

## Cztery misje dodatkowe poza zajęciami

Każda misja jest **dobrowolna**, można ją zrobić zdalnie na zwykłym komputerze, na Ubuntu lub Windows. Nie wymaga zakupów ani zakładania konta Unity Student.

| Kod | Przygoda | Dowód w projekcie | Maksimum |
|---|---|---|---:|
| \`xr-optics\` | **Optyczny detektyw:** rysunek lub demonstrator wyjaśniający wergencję i akomodację bez wywoływania bólu | 1 grafika, krótki opis, rozróżnienie symulacji i prawdziwego pomiaru | 2 |
| \`robot-failsafe\` | **Robot ma prawo milczeć:** wskaźnik LIVE/STALE z nagranego logu ROS | minimalny projekt lub test i wyjaśnienie niezawodności | 2 |
| \`accessible-hud\` | **HUD dla każdego:** dostępny interfejs z dużą czcionką i alternatywą dla koloru | dwa warianty porównawcze i jedno uzasadnienie | 2 |
| \`ar-measurement\` | **Mierz, nie zgaduj:** porównanie linijki z AR albo z wynikami referencyjnymi | kilka pomiarów, analiza błędu i ograniczenia | 2 |

**Maksymalnie 8 dodatkowych punktów na grupę przez cały kurs.** Punkty może otrzymać cała grupa dzięki jednemu zgłoszeniu, ale każdą misję można zaliczyć tylko raz. Wynik nie wpływa na indywidualne oceny.

**Jak zgłosić?** Przygotuj niewielki publiczny projekt, zrzut ekranu lub krótkie nagranie bez osób trzecich. Nie nagrywaj reakcji fizjologicznych ani objawów choroby symulatorowej. Wyślij prowadzącemu opis lub zaprezentuj wynik na konsultacjach. Prowadzący może po zatwierdzeniu dodać punkty:

    python competition/add_sidequest.py --group G2 --task accessible-hud --points 2 --reason "Przedstawiono dwa przetestowane warianty dostępnego HUD"

Ta komenda jest **przykładem składni, nie przyznaniem punktów**. Aktualizacja odbywa się ręcznie, najwyżej raz na zaakceptowaną misję, bez osobnego workflow.

## Zasada fair play

Punkty oznaczają jakość odkrycia, nie odporność na nudności, liczbę godzin w goglach, liczbę commitów ani posiadanie drogich urządzeń. Wyniki są grupowe, a indywidualne zaliczenie od ligi niezależne.
