# Przygotowanie środowiska WiRR

Instrukcja referencyjna dla laboratoriów WiRR. Zalecanym systemem jest **Windows 11 64-bit**. Obsługiwanym wariantem alternatywnym jest **Ubuntu 22.04 lub 24.04 LTS 64-bit**.

## Wymagane składniki

- Unity Hub;
- najnowsza dostępna wersja **Unity 6 (6000.x)**;
- Android Build Support;
- Android SDK & NDK Tools;
- OpenJDK;
- Git;
- IDE C# z integracją Unity;
- GitHub CLI opcjonalnie, ale zalecane;
- Android Studio opcjonalnie, zalecane do ADB, Logcat i diagnostyki Androida;
- Docker przed Laboratorium 06.

Do Androida i Meta Quest 3 używaj SDK, NDK i OpenJDK zainstalowanych przez Unity Hub. Nie zastępuj ich przypadkowymi wersjami systemowymi, jeśli nie ma ku temu konkretnej potrzeby.

## Windows 11 64-bit

1. Zainstaluj Unity Hub.
2. W Unity Hub zainstaluj Unity 6 (6000.x).
3. Podczas instalacji albo później przez **Installs → ustawienia wersji → Add modules** zaznacz:
   - Android Build Support;
   - Android SDK & NDK Tools;
   - OpenJDK.
4. Zainstaluj Git.
5. Zalecane IDE: Visual Studio 2022 Community z workloadem **Game development with Unity**.
6. Opcjonalnie zainstaluj GitHub CLI i Android Studio.
7. Przed Lab 06 zainstaluj Docker Desktop.
8. Zweryfikuj:

```powershell
git --version
gh --version
docker --version
docker compose version
```

Brak `gh` nie blokuje laboratoriów. Brak Dockera blokuje tylko wariant WebSim w Lab 06.

## Ubuntu 22.04 / 24.04 LTS 64-bit

1. Zainstaluj Unity Hub i Unity 6 (6000.x).
2. W **Add modules** doinstaluj:
   - Android Build Support;
   - Android SDK & NDK Tools;
   - OpenJDK.
3. Zainstaluj podstawowe narzędzia:

```bash
sudo apt update
sudo apt install git curl ca-certificates
```

4. Użyj Ridera albo VS Code z rozszerzeniem Unity jako IDE.
5. Opcjonalnie zainstaluj GitHub CLI.
6. Przed Lab 06 zainstaluj Docker Engine z oficjalnego repozytorium Dockera, wraz z:
   - `docker-ce`;
   - `docker-ce-cli`;
   - `containerd.io`;
   - `docker-buildx-plugin`;
   - `docker-compose-plugin`.
7. Zweryfikuj:

```bash
git --version
docker --version
docker compose version
```

## Ważne: C# 9 a komunikat o C# 10

Unity 6 (6000.x) w typowej konfiguracji kompiluje skrypty projektu jako C# 9. Komunikat:

```text
Feature 'global using directive' is not available in C# 9.0.
Please use language version 10.0 or greater.
```

nie oznacza, że student powinien „zaktualizować C#”. Najczęstsza przyczyna to dodatkowy projekt .NET utworzony wewnątrz katalogu `Assets`. Narzędzia .NET generują wtedy pliki w rodzaju:

```text
Assets/.../obj/Debug/net10.0/...GlobalUsings.g.cs
```

Unity traktuje pliki `.cs` znajdujące się pod `Assets` jako część projektu i próbuje skompilować wygenerowane `global using`, które wymaga C# 10.

### Jak naprawić

1. Otwórz pierwszy błąd CS8773 w Console i sprawdź pełną ścieżkę pliku.
2. Jeśli prowadzi do `Assets/.../obj/...` albo `*.GlobalUsings.g.cs`, zamknij Unity.
3. Usuń dodatkowy projekt .NET utworzony w `Assets` albo co najmniej jego katalogi `bin` i `obj`.
4. Nie twórz `dotnet new` w `Assets`.
5. Nie dodawaj własnego `csc.rsp` z `-langversion:10`.
6. Usuń katalogi `Library`, `Temp` i główny `obj` projektu Unity, jeśli błąd pozostał po usunięciu źródła.
7. Otwórz projekt ponownie i pozwól Unity odtworzyć pliki projektu dla IDE.

Pakiet WiRR nie używa obecnie konstrukcji wymagających C# 10, takich jak `global using`, file-scoped namespaces ani `record struct`.

## Kontrola przed laboratorium

Po instalacji pakietu otwórz **WiRR → Narzędzia kursu**. Sekcja **Stan środowiska** sprawdza zależności dla aktualnie wybranego laboratorium. Przycisk **Napraw automatycznie** instaluje brakujące zależności obsługiwane przez Unity Package Manager. Moduły Unity Hub są tylko wykrywane i muszą zostać doinstalowane przez Hub.
