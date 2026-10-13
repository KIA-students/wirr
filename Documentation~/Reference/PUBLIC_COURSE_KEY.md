# Publiczny klucz kursu WiRR

Klucz identyfikacyjny dostarczany w paczce Unity:

```text
WIRR-2026-2027-PUBLIC-SUBMISSION-V1
```

Jest **publiczny** i jednakowy dla wszystkich studentów. Nie jest tokenem GitHub, hasłem, tajnym kluczem Supabase ani dowodem tożsamości studenta. Nie zapewnia prawa zapisu do repozytorium.

**Zapis raportu:** w Unity wybierz **WiRR → Raporty → Formularz raportu laboratoryjnego → Zapisz raport lokalnie (bez logowania)**. Działa offline.

**Przekazanie raportu:** docelowo osobna usługa przyjmująca plik JSON powinna otrzymać ten klucz w nagłówku `X-WiRR-Course-Key`; dopiero prywatny proces serwera publikuje zatwierdzone wyniki. Endpoint KIA jest skonfigurowany w paczce. W Unity wybierz **WiRR → Student → Wyślij raport bez logowania** i wskaż wyeksportowany JSON. Zgłoszenie jest przyjęte dopiero po pozytywnym potwierdzeniu serwera; pozostaje w prywatnej kolejce do przeglądu. GitHub jest oddzielnym procesem. Starszy moduł logowania KIA nadal istnieje, ale nie jest potrzebny do tej ścieżki.

Backend powinien ograniczyć rozmiar i częstotliwość żądań, walidować strukturę, nie ufać polom tożsamości, deduplikować zgłoszenia i przechowywać token GitHub wyłącznie po stronie serwera. Z uwagi na publiczny charakter repozytorium nie publikować danych identyfikujących studentów ani indywidualnych wyników.
