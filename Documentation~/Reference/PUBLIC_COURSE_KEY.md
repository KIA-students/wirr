# Publiczny klucz kursu WiRR

Klucz identyfikacyjny dostarczany w paczce Unity:

```text
WIRR-2026-2027-PUBLIC-SUBMISSION-V1
```

Jest **publiczny** i jednakowy dla wszystkich studentów. Nie jest tokenem GitHub, hasłem, tajnym kluczem Supabase ani dowodem tożsamości studenta. Nie zapewnia prawa zapisu do repozytorium.

**Zapis raportu:** w Unity wybierz **WiRR → Raporty → Formularz raportu laboratoryjnego → Zapisz raport lokalnie (bez logowania)**. Działa offline.

**Przekazanie raportu:** docelowo osobna usługa przyjmująca plik JSON powinna otrzymać ten klucz w nagłówku `X-WiRR-Course-Key`; dopiero prywatny proces serwera publikuje zatwierdzone wyniki. Endpoint nie jest jeszcze skonfigurowany w paczce, więc brak serwerowego potwierdzenia oznacza, że raport **nie został dostarczony**. Obecna opcja „Wyślij raport do KIA” używa osobnego istniejącego procesu logowania, a nie publicznego klucza.

Backend powinien ograniczyć rozmiar i częstotliwość żądań, walidować strukturę, nie ufać polom tożsamości, deduplikować zgłoszenia i przechowywać token GitHub wyłącznie po stronie serwera. Z uwagi na publiczny charakter repozytorium nie publikować danych identyfikujących studentów ani indywidualnych wyników.
