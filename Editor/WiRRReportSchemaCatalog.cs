using System;
using System.Collections.Generic;

namespace KIA.WiRR.Editor
{
    internal enum WiRRReportFieldKind { Text, Multiline, Integer, Number, Choice, Boolean }

    internal sealed class WiRRReportField
    {
        public string Id { get; }
        public string Label { get; }
        public WiRRReportFieldKind Kind { get; }
        public bool Required { get; }
        public string Unit { get; }
        public string Help { get; }
        public string[] Choices { get; }

        public WiRRReportField(string id, string label, WiRRReportFieldKind kind = WiRRReportFieldKind.Text,
            bool required = true, string unit = "", string help = "", params string[] choices)
        {
            Id = id; Label = label; Kind = kind; Required = required; Unit = unit; Help = help;
            Choices = choices ?? Array.Empty<string>();
        }
    }

    internal sealed class WiRRReportSection
    {
        public string Title { get; }
        public string Checkpoint { get; }
        public IReadOnlyList<WiRRReportField> Fields { get; }
        public WiRRReportSection(string title, string checkpoint, params WiRRReportField[] fields)
        { Title = title; Checkpoint = checkpoint; Fields = fields; }
    }

    internal static class WiRRReportSchemaCatalog
    {
        public static IReadOnlyList<WiRRReportSection> Get(int lab) => lab switch
        {
            1 => Lab01(), 2 => Lab02(), 3 => Lab03(), 4 => Lab04(), 5 => Lab05(), 6 => Lab06(), 7 => Lab07(),
            _ => throw new ArgumentOutOfRangeException(nameof(lab))
        };

        private static WiRRReportField T(string id, string label, bool req = true, string help = "") => new(id, label, WiRRReportFieldKind.Text, req, help: help);
        private static WiRRReportField M(string id, string label, bool req = true, string help = "") => new(id, label, WiRRReportFieldKind.Multiline, req, help: help);
        private static WiRRReportField N(string id, string label, string unit = "", bool req = true) => new(id, label, WiRRReportFieldKind.Number, req, unit);
        private static WiRRReportField I(string id, string label, string unit = "", bool req = true) => new(id, label, WiRRReportFieldKind.Integer, req, unit);
        private static WiRRReportField B(string id, string label, bool req = true) => new(id, label, WiRRReportFieldKind.Boolean, req);
        private static WiRRReportField C(string id, string label, params string[] values) => new(id, label, WiRRReportFieldKind.Choice, true, choices: values);

        private static WiRRReportSection CommonEnvironment() => new("Środowisko i identyfikacja", "COMMON",
            T("environment.station", "Stanowisko / oznaczenie PC", false),
            T("environment.device", "HMD / telefon / urządzenie", false),
            T("environment.notes", "Uwagi o środowisku", false));

        private static IReadOnlyList<WiRRReportSection> Lab01() => new[]
        {
            CommonEnvironment(),
            new WiRRReportSection("3.0 — baseline i eksperyment kontrolny", "3.0", N("cp30.baseline_fps","Mediana FPS","FPS"), N("cp30.baseline_frame_ms","Mediana czasu klatki","ms"), M("cp30.answer","Odpowiedź na pytanie badawcze")),
            new WiRRReportSection("3.5 — CPU", "3.5", N("cp35.cpu_median_fps","Mediana FPS w wybranym wariancie","FPS"), N("cp35.main_thread_ms","Main Thread","ms"), N("cp35.gc_alloc","GC Alloc","B",false), M("cp35.answer","Wniosek CPU")),
            new WiRRReportSection("4.0 — renderowanie", "4.0", N("cp40.gpu_median_fps","Mediana FPS","FPS"), I("cp40.batches","Batches"), I("cp40.triangles","Triangles"), N("cp40.render_scale","Render Scale","",false), M("cp40.answer","Wniosek GPU")),
            new WiRRReportSection("4.5 — fizyka", "4.5", N("cp45.physics_median_fps","Mediana FPS","FPS"), N("cp45.physics_ms","Physics.Processing","ms"), N("cp45.fixed_timestep","Fixed Timestep","s",false), M("cp45.answer","Wniosek fizyka")),
            new WiRRReportSection("5.0 — Quest 3", "5.0", C("cp50.quest_build","Build standalone","sukces","błąd","niezweryfikowano"), N("cp50.quest_fps","Mediana FPS standalone","FPS",false), B("cp50.tracking_6dof","Tracking 6DoF działa"), M("cp50.answer","Wniosek końcowy"))
        };

        private static IReadOnlyList<WiRRReportSection> Lab02() => new[]
        {
            CommonEnvironment(),
            new WiRRReportSection("3.0 — chwyt, socket, ray", "3.0", N("cp30.task_median_s","Mediana czasu zadania","s"), I("cp30.errors","Błędy łącznie"), M("cp30.answer","Odpowiedź na pytanie badawcze")),
            new WiRRReportSection("3.5 — UI i feedback", "3.5", N("cp35.ui_median_s","Mediana czasu","s"), I("cp35.false_activations","Błędne / powtórzone aktywacje"), B("cp35.haptics_requested","Żądanie haptyki generowane"), M("cp35.answer","Wniosek")),
            new WiRRReportSection("4.0 — lokomocja", "4.0", N("cp40.locomotion_median_s","Mediana czasu","s"), I("cp40.corrections","Błędy / korekty"), B("cp40.teleport_valid","Teleport odrzuca niedozwolone powierzchnie"), M("cp40.answer","Wniosek komfort / lokomocja")),
            new WiRRReportSection("4.5 — diagnostyka", "4.5", N("cp45.scenario_median_s","Mediana scenariusza","s"), M("cp45.h1","Hipoteza H1"), M("cp45.h2","Hipoteza H2"), M("cp45.fix","Minimalna poprawka"), M("cp45.post_result","Wynik po poprawce")),
            new WiRRReportSection("5.0 — Quest 3 i SSQ", "5.0", N("cp50.ssq_pre_ts","SSQ TS PRE"), N("cp50.ssq_post_ts","SSQ TS POST"), N("cp50.comfort_0_10","Komfort 0–10"), M("cp50.answer","Wniosek ograniczony do tej sesji"))
        };

        private static IReadOnlyList<WiRRReportSection> Lab03() => new[]
        {
            CommonEnvironment(),
            new WiRRReportSection("3.0 — tracking i płaszczyzny", "3.0", N("cp30.tracking_s","Czas do SessionTracking","s"), N("cp30.first_plane_s","Czas do pierwszej użytecznej płaszczyzny","s"), T("cp30.not_tracking_reason","Najczęstszy notTrackingReason"), M("cp30.answer","Odpowiedź i ograniczenia")),
            new WiRRReportSection("3.5 — raycast i skala 1:1", "3.5", I("cp35.hits_10","Poprawne trafienia / 10"), N("cp35.placement_error_mm","Mediana błędu placementu","mm"), B("cp35.scale_1_1","Skala 1:1"), M("cp35.answer","Wniosek")),
            new WiRRReportSection("4.0 — kotwica i dryf", "4.0", N("cp40.path_m","Długość ścieżki","m"), N("cp40.drift_median_mm","Mediana błędu po powrocie","mm"), T("cp40.tracking_state","Tracking podczas ruchu"), M("cp40.answer","Wniosek")),
            new WiRRReportSection("4.5 — rejestracja dwupunktowa", "4.5", N("cp45.ox_m","Zmierzony OX","m"), N("cp45.e_median_mm","Mediana e_med","mm"), N("cp45.e_max_mm","Maksymalny błąd","mm"), M("cp45.uncertainty","Najważniejsze źródło niepewności"), M("cp45.answer","Wniosek")),
            new WiRRReportSection("5.0 — kontrolowany błąd", "5.0", M("cp50.fault","Kontrolowany błąd"), M("cp50.h1","H1"), M("cp50.h2","H2"), N("cp50.error_after_fix_mm","e_med po naprawie","mm"), M("cp50.answer","Diagnoza końcowa"))
        };

        private static IReadOnlyList<WiRRReportSection> Lab04() => new[]
        {
            CommonEnvironment(),
            new WiRRReportSection("3.0 — Depth API i diagnostyka", "3.0", C("cp30.depth_available","Depth","dostępna","niedostępna","niezweryfikowano"), T("cp30.depth_mode","Current depth mode"), T("cp30.depth_resolution","Rozdzielczość ostatniej klatki depth", false), M("cp30.answer","Wniosek i najważniejsze ograniczenie pomiaru")),
            new WiRRReportSection("3.5 — okluzja środowiskowa", "3.5", T("cp35.occlusion_mode","Dominujący tryb / jakość"), I("cp35.occlusion_errors","Błędy okluzji / 10"), T("cp35.signature","Dominująca sygnatura błędu"), M("cp35.answer","Wniosek o smoothingu / jakości depth")),
            new WiRRReportSection("4.0 — depth-raycast i błąd odległości", "4.0", N("cp40.depth_error_m","Mediana błędu dla HIT","m"), I("cp40.hits","Liczba HIT / 5"), I("cp40.misses","Liczba MISS / 5"), M("cp40.answer","Wniosek i wyjaśnienie, dlaczego MISS nie jest 0 m")),
            new WiRRReportSection("4.5 — estymacja oświetlenia", "4.5", N("cp45.brightness","Mediana brightness","",false), N("cp45.color_temperature","Color temperature","K",false), T("cp45.current_mode","Current light estimation mode"), M("cp45.answer","Wniosek i opis niedostępnych pól API")),
            new WiRRReportSection("5.0 — kontrolowany błąd i diagnoza", "5.0", M("cp50.fault","Kontrolowany błąd"), M("cp50.h1","H1"), M("cp50.h2","H2"), M("cp50.test","Test rozstrzygający i wynik"), M("cp50.fix","Minimalna poprawka"), M("cp50.answer","Diagnoza końcowa"))
        };

        private static IReadOnlyList<WiRRReportSection> Lab05() => new[]
        {
            CommonEnvironment(),
            new WiRRReportSection("3.0 — audyt modelu CAD", "3.0", T("cp30.source_format","Format źródłowy"), I("cp30.triangles_before","Triangles przed"), I("cp30.triangles_after","Triangles po"), M("cp30.answer","Ocena topologii / importu")),
            new WiRRReportSection("3.5 — LOD", "3.5", I("cp35.lod0_triangles","LOD0 triangles"), I("cp35.lod1_triangles","LOD1 triangles"), I("cp35.lod2_triangles","LOD2 triangles"), M("cp35.answer","Kompromis jakość–koszt")),
            new WiRRReportSection("4.0 — wydajność zasobu", "4.0", N("cp40.import_s","Czas importu","s"), N("cp40.memory_mb","Pamięć zasobu","MB"), N("cp40.frame_ms","Mediana czasu klatki","ms"), M("cp40.answer","Wniosek")),
            new WiRRReportSection("4.5 — diagnostyka geometrii", "4.5", M("cp45.issue","Wykryty problem geometrii"), M("cp45.fix","Zastosowana naprawa"), N("cp45.error_before","Miara błędu przed", "", false), N("cp45.error_after","Miara błędu po", "", false), M("cp45.answer","Weryfikacja naprawy")),
            new WiRRReportSection("5.0 — pipeline", "5.0", C("cp50.pipeline","Wybrana ścieżka","STEP→DCC→FBX/glTF","USD","bezpośredni mesh","inna"), M("cp50.risk","Najważniejsze ryzyko pipeline"), M("cp50.answer","Uzasadnienie końcowe"))
        };

        private static IReadOnlyList<WiRRReportSection> Lab06() => new[]
        {
            CommonEnvironment(),
            new WiRRReportSection("3.0 — źródło symulacji", "3.0", C("cp30.mode","Tryb","LOCAL","LAN","WEBSIM"), T("cp30.ros_version","ROS 2"), T("cp30.gazebo_version","Gazebo"), N("cp30.joint_rate_hz","/joint_states","Hz"), M("cp30.answer","Weryfikacja Gazebo→ROS 2→Unity")),
            new WiRRReportSection("3.5 — zgodność stanu", "3.5", N("cp35.joint1_error_deg","Błąd joint1","deg"), N("cp35.joint2_error_deg","Błąd joint2","deg"), N("cp35.joint3_error_deg","Błąd joint3","deg",false), M("cp35.answer","Wniosek o zgodności")),
            new WiRRReportSection("4.0 — transport i bufor", "4.0", N("cp40.latency_ms","Mediana RTT / opóźnienia","ms"), N("cp40.jitter_ms","Jitter","ms"), N("cp40.buffer_ms","Opóźnienie interpolacji","ms"), M("cp40.answer","Wniosek transportowy")),
            new WiRRReportSection("4.5 — utrata danych i STALE", "4.5", N("cp45.stale_ms","Czas do STALE","ms"), N("cp45.recovery_ms","Czas powrotu LIVE","ms"), M("cp45.h1","H1"), M("cp45.h2","H2"), M("cp45.answer","Diagnoza")),
            new WiRRReportSection("5.0 — wynik końcowy", "5.0", N("cp50.sync_error_deg","Maks. błąd synchronizacji","deg"), B("cp50.recovery_pass","STALE→LIVE działa"), M("cp50.answer","Wniosek o bliźniaku cyfrowym"))
        };

        private static IReadOnlyList<WiRRReportSection> Lab07() => new[]
        {
            CommonEnvironment(),
            new WiRRReportSection("3.0 — smoke i kryteria akceptacji", "3.0", I("cp30.smoke_pass","Liczba PASS"), I("cp30.blocking_fail","Blokujące FAIL"), I("cp30.nv","NV",false), M("cp30.answer","Ocena kryteriów")),
            new WiRRReportSection("3.5 — wydajność", "3.5", N("cp35.frame_ms","Mediana czasu klatki","ms"), N("cp35.fps","Mediana FPS","FPS"), N("cp35.memory_mb","Pamięć","MB"), M("cp35.answer","Wniosek CPU/GPU/memory/transport")),
            new WiRRReportSection("4.0 — użyteczność", "4.0", N("cp40.success_rate","Skuteczność","%"), I("cp40.false_activations","Błędne aktywacje"), I("cp40.assistance","Liczba podpowiedzi"), M("cp40.answer","Wniosek użyteczność / dostępność")),
            new WiRRReportSection("4.5 — fault injection i regresja", "4.5", T("cp45.fault","Kontrolowany fault"), N("cp45.recovery_s","Czas recovery","s"), B("cp45.regression_pass","Test regresyjny PASS"), M("cp45.diagnosis","H1/H2, test rozstrzygający i poprawka")),
            new WiRRReportSection("5.0 — automatyzacja, ryzyko i decyzja", "5.0", B("cp50.automated_pass","Automatyczny test PASS"), B("cp50.controlled_fail","Kontrolowany FAIL wykazany"), I("cp50.max_risk","Maksymalne R=P×S"), C("cp50.acceptance","Decyzja","ACCEPT","ACCEPT WITH CONDITIONS","REJECT"), M("cp50.answer","Uzasadnienie decyzji"))
        };
    }
}