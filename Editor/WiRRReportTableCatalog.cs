using System;
using System.Collections.Generic;

namespace KIA.WiRR.Editor
{
    internal readonly struct WiRRTableAxis
    {
        public string Id { get; }
        public string Label { get; }
        public WiRRTableAxis(string id, string label) { Id = id; Label = label; }
    }

    internal sealed class WiRRReportTable
    {
        public string Id { get; }
        public string Label { get; }
        public string Help { get; }
        public IReadOnlyList<WiRRTableAxis> Rows { get; }
        public IReadOnlyList<WiRRTableAxis> Columns { get; }

        public WiRRReportTable(string id, string label, WiRRTableAxis[] rows, WiRRTableAxis[] columns, string help = "")
        {
            Id = id; Label = label; Rows = rows; Columns = columns; Help = help;
        }
    }

    internal static class WiRRReportTableCatalog
    {
        public static IReadOnlyList<WiRRReportTable> Get(int lab, string checkpoint)
        {
            return lab switch
            {
                1 => Lab01(checkpoint), 2 => Lab02(checkpoint), 3 => Lab03(checkpoint), 4 => Lab04(checkpoint),
                5 => Lab05(checkpoint), 6 => Lab06(checkpoint), 7 => Lab07(checkpoint),
                _ => Array.Empty<WiRRReportTable>()
            };
        }

        public static string CellKey(WiRRReportTable table, WiRRTableAxis row, WiRRTableAxis column)
            => $"table.{table.Id}.{row.Id}.{column.Id}";

        private static WiRRTableAxis A(string id, string label) => new(id, label);
        private static WiRRTableAxis[] Rows(params string[] labels)
        {
            var result = new WiRRTableAxis[labels.Length];
            for (var i = 0; i < labels.Length; i++) result[i] = A(Slug(labels[i], i), labels[i]);
            return result;
        }
        private static WiRRTableAxis[] Cols(params string[] labels) => Rows(labels);
        private static string Slug(string value, int index)
        {
            var chars = new List<char>();
            foreach (var c in value.ToLowerInvariant())
                if (char.IsLetterOrDigit(c)) chars.Add(c); else if (chars.Count > 0 && chars[^1] != '_') chars.Add('_');
            var text = new string(chars.ToArray()).Trim('_');
            return string.IsNullOrEmpty(text) ? $"item{index + 1}" : text;
        }
        private static WiRRReportTable T(string id, string label, string[] rows, string[] columns, string help = "")
            => new(id, label, Rows(rows), Cols(columns), help);

        private static IReadOnlyList<WiRRReportTable> Lab01(string cp) => cp switch
        {
            "3.0" => new[] {
                T("lab01_cp30_baseline", "Baseline PC — trzy próby", new[]{"Próba 1","Próba 2","Próba 3"}, new[]{"FPS","ms/klatka"}),
                T("lab01_cp30_ab", "Eksperyment kontrolny 3.0", new[]{"A","B"}, new[]{"FPS 1","FPS 2","FPS 3","Mediana FPS","ms 1","ms 2","ms 3","Mediana ms"}) },
            "3.5" => new[] {
                T("lab01_cpu_a", "CPU-A — iterationsPerFrame", new[]{"0","10000","25000","50000","100000","200000","400000"}, new[]{"FPS 1","FPS 2","FPS 3","Mediana FPS","Mediana ms","Main Thread ms","Delta FPS %"}),
                T("lab01_cpu_b", "CPU-B — blocksPerFrame", new[]{"1","2","4","8","16","32"}, new[]{"FPS 1","FPS 2","FPS 3","Mediana FPS","Mediana ms","Delta FPS %"}),
                T("lab01_cpu_c", "CPU-C — allocationBytesPerFrame", new[]{"0","1024","4096","16384","65536","262144"}, new[]{"FPS 1","FPS 2","FPS 3","Mediana FPS","Mediana ms","GC Alloc","GC zaobserwowane"}) },
            "4.0" => new[] {
                T("lab01_gpu_a", "GPU-A — liczba widocznych obiektów", new[]{"0","100","250","500","1000","2000","4000"}, new[]{"FPS 1","FPS 2","FPS 3","Mediana FPS","Mediana ms","Batches","SetPass","Triangles","Delta FPS %"}),
                T("lab01_gpu_b", "GPU-B — liczba materiałów", new[]{"1","2","4","8","16","32"}, new[]{"FPS 1","FPS 2","FPS 3","Mediana FPS","Mediana ms","Batches","SetPass"}),
                T("lab01_gpu_d", "GPU-D — Render Scale", new[]{"0.60","0.70","0.80","0.90","1.00","1.20"}, new[]{"FPS 1","FPS 2","FPS 3","Mediana FPS","Mediana ms","Delta FPS %"}) },
            "4.5" => new[] {
                T("lab01_phy_a", "PHY-A — dynamiczne Rigidbody", new[]{"0","25","50","100","200","400","800"}, new[]{"FPS 1","FPS 2","FPS 3","Mediana FPS","Mediana ms","Physics ms","Delta FPS %"}),
                T("lab01_phy_c", "PHY-C — Fixed Timestep", new[]{"0.0333","0.0250","0.0200","0.0167","0.0133","0.0111"}, new[]{"kroki/s","FPS 1","FPS 2","FPS 3","Mediana FPS","Mediana ms","Physics ms"}) },
            "5.0" => new[] { T("lab01_quest_tests", "Quest 3 — testy funkcjonalne", new[]{"obrót głowy","translacja głowy","lewy kontroler","prawy kontroler","Select","standalone bez Link"}, new[]{"Wynik","Uwagi"}) },
            _ => Array.Empty<WiRRReportTable>()
        };

        private static IReadOnlyList<WiRRReportTable> Lab02(string cp) => cp switch
        {
            "3.0" => new[] { T("lab02_cp30_conditions", "Chwyt / socket / ray — pomiary", new[]{"A","B","C"}, new[]{"Próba 1 s","Próba 2 s","Próba 3 s","Próba 4 s","Próba 5 s","Mediana s","Błędy"}) },
            "3.5" => new[] { T("lab02_cp35_conditions", "UI i feedback — pomiary", new[]{"A","B","C"}, new[]{"Próba 1 s","Próba 2 s","Próba 3 s","Próba 4 s","Próba 5 s","Mediana s","Błędne aktywacje"}) },
            "4.0" => new[] { T("lab02_cp40_conditions", "Lokomocja — pomiary", new[]{"A","B","C"}, new[]{"Próba 1 s","Próba 2 s","Próba 3 s","Próba 4 s","Próba 5 s","Mediana s","Błędy/korekty"}) },
            "4.5" => new[] { T("lab02_scenario", "Scenariusz bazowy", new[]{"1","2","3","4","5"}, new[]{"Czas s","Upuszczenia","Błędne aktywacje","Nieudane teleportacje","Błędy osadzenia/reset","Uwagi"}) },
            "5.0" => new[] {
                T("lab02_ssq", "SSQ — wyniki liczbowe", new[]{"Osoba A PRE","Osoba A POST","Osoba B PRE","Osoba B POST"}, new[]{"N","O","D","TS"}),
                T("lab02_quest_experiment", "Eksperyment Quest 3", new[]{"A","B","C"}, new[]{"Próba 1 s","Próba 2 s","Próba 3 s","Mediana s","Błędy/korekty","Komfort 0–10"}) },
            _ => Array.Empty<WiRRReportTable>()
        };

        private static IReadOnlyList<WiRRReportTable> Lab03(string cp) => cp switch
        {
            "3.0" => new[] { T("lab03_tracking_conditions", "Tracking — warunki eksperymentalne", new[]{"A","B","C"}, new[]{"Powt. 1 s","Powt. 2 s","Powt. 3 s","Mediana s","notTrackingReason/uwagi"}) },
            "3.5" => new[] { T("lab03_placement", "Raycast i placement", new[]{"A","B","C"}, new[]{"Poprawne trafienia /10","Mediana błędu mm","Uwagi"}) },
            "4.0" => new[] {
                T("lab03_drift", "Kotwica — pomiar po powrocie", new[]{"1","2","3"}, new[]{"Błąd końcowy mm","Tracking","notTrackingReason","Uwagi"}),
                T("lab03_drift_conditions", "Eksperyment dryfu", new[]{"A","B","C"}, new[]{"Powt. 1 mm","Powt. 2 mm","Powt. 3 mm","Mediana mm","Uwagi"}) },
            "4.5" => new[] { T("lab03_registration", "Bazowe rejestracje dwupunktowe", new[]{"1","2","3"}, new[]{"Zmierzony OX m","e1 mm","e2 mm","e3 mm","e_med mm","e_max mm"}) },
            "5.0" => new[] { T("lab03_fault", "Baseline → błąd → naprawa", new[]{"baseline 1","baseline 2","baseline 3","błąd 1","błąd 2","błąd 3","naprawa 1","naprawa 2","naprawa 3"}, new[]{"e1 mm","e2 mm","e3 mm","e_med mm","e_max mm"}) },
            _ => Array.Empty<WiRRReportTable>()
        };

        private static IReadOnlyList<WiRRReportTable> Lab04(string cp) => cp switch
        {
            "3.0" => new[] { T("lab04_depth_v1", "Depth API — eksperyment v1", new[]{"A","B","C"}, new[]{"successRate 1 %","successRate 2 %","successRate 3 %","Mediana %","Rozdzielczość","Uwagi"}) },
            "3.5" => new[] {
                T("lab04_occlusion_base", "Okluzja — test bazowy", new[]{"Off","Raw","Smoothed"}, new[]{"Model zasłaniany?","Błędy /10","Dominujący błąd"}),
                T("lab04_occlusion_v2", "Okluzja — eksperyment v2", new[]{"A","B","C"}, new[]{"Tryb/jakość","Błędy /10","Requested depth","Current depth","Uwagi"}) },
            "4.0" => new[] {
                T("lab04_depth_raycast", "Depth-raycast — seria bazowa", new[]{"0.50 m / 1","0.50 m / 2","0.50 m / 3","0.50 m / 4","0.50 m / 5","1.00 m / 1","1.00 m / 2","1.00 m / 3","1.00 m / 4","1.00 m / 5","1.50 m / 1","1.50 m / 2","1.50 m / 3","1.50 m / 4","1.50 m / 5"}, new[]{"HIT/MISS","d_AR m","e_d m"}),
                T("lab04_depth_v3", "Depth-raycast — eksperyment v3", new[]{"A","B","C"}, new[]{"HIT /5","MISS /5","Mediana błędu HIT m","Uwagi"}) },
            "4.5" => new[] { T("lab04_lighting", "Estymacja oświetlenia — pięć warunków", new[]{"Jasne rozproszone","Słabsze rozproszone","Źródło boczne","Światło od przodu","Oświetlenie mieszane"}, new[]{"Mediana brightness","Color temp K/NA","Main direction/NA","Current mode","Ocena 1–5","Uwagi"}) },
            "5.0" => new[] { T("lab04_fault", "Baseline → błąd → naprawa", new[]{"baseline","kontrolowany błąd","po naprawie"}, new[]{"Powt. 1","Powt. 2","Powt. 3","Mediana/wynik","Uwagi"}) },
            _ => Array.Empty<WiRRReportTable>()
        };

        private static IReadOnlyList<WiRRReportTable> Lab05(string cp) => cp switch
        {
            "3.0" => new[] {
                T("lab05_topology", "Baseline topology audit", new[]{"Baseline"}, new[]{"MeshFilter","Vertices","Triangles","Submeshes","Material slots","Boundary edges","Non-manifold","Degenerate","q05","qMedian","World size XYZ"}),
                T("lab05_formats", "Porównanie formatów / eksportu", new[]{"A","B","C"}, new[]{"Format","Rozmiar","Obiekty","Vertices","Triangles","Submesh","Materiały","UV","Hierarchia","Wymiary OK"}) },
            "3.5" => new[] { T("lab05_triangulation", "Triangulacja i jakość siatki", new[]{"A","B","C"}, new[]{"Metoda/parametr","Vertices","Triangles","Boundary","Non-manifold","Degenerate","q05","qMedian","Cechy zachowane"}) },
            "4.0" => new[] {
                T("lab05_lods", "Finalne geometrie LOD", new[]{"Baseline","LOD0","LOD1","LOD2"}, new[]{"Rozmiar pliku","Vertices","Triangles","Submesh","Materiały","q05","qMedian","Uwagi"}),
                T("lab05_benchmark", "Benchmark LOD — trzy próby", new[]{"A1","A2","A3","B1","B2","B3","C1","C2","C3"}, new[]{"mean ms","median ms","p95 ms","FPS","Triangles","Renderers","Material slots","CPU ms","GPU ms","Batches","Pamięć"}) },
            "4.5" => new[] { T("lab05_materials", "Materiały, draw calls i kolizje", new[]{"A","B","C"}, new[]{"Triangles","Materiały","Draw calls/batches","CPU ms","GPU ms","Pamięć","Błędy /20","Jakość 1–5"}) },
            "5.0" => new[] {
                T("lab05_fault", "Baseline → fault → repaired", new[]{"baseline","fault","repaired"}, new[]{"Wymiar m","Vertices","Triangles","Boundary","q05","Materiały","Hierarchia OK","median ms","p95 ms","Błąd funkcjonalny","Uwagi"}),
                T("lab05_final", "Tabela końcowa", new[]{"Baseline","LOD0","LOD1","LOD2","Final XR"}, new[]{"Vertices","Triangles","Submesh","Materiały","median ms","p95 ms","Batches","Pamięć","Kompromis"}) },
            _ => Array.Empty<WiRRReportTable>()
        };

        private static IReadOnlyList<WiRRReportTable> Lab06(string cp) => cp switch
        {
            "3.0" => new[] {
                T("lab06_environment", "Konfiguracja środowiska", new[]{"ROS 2","Gazebo Sim","ros_gz","gz_ros2_control","Robot","ROS-TCP Connector","Endpoint","IPv4 hosta","Port","Transport"}, new[]{"Wartość"}),
                T("lab06_robot_motion", "Test ruchu robota", new[]{"Próba 1","Próba 2","Próba 3"}, new[]{"Komenda joint1","Komenda joint2","JointState joint1","JointState joint2","Gazebo/Unity zgodne?"}) },
            "3.5" => new[] { T("lab06_mapping", "Mapowanie JointState → Unity", new[]{"joint1","joint2","joint3"}, new[]{"Obiekt Unity","Typ","Oś lokalna","sign","offset","Jednostka"}) },
            "4.0" => new[] {
                T("lab06_interarrival", "Inter-arrival — pierwsze 30 próbek", new[]{"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21","22","23","24","25","26","27","28","29","30"}, new[]{"inter-arrival ms"}),
                T("lab06_buffer", "Buforowanie i interpolacja — wariant v3", new[]{"A","B","C"}, new[]{"Źródło","Hz","Delay ms","Buffer","T średnie ms","sT ms","Płynność"}),
                T("lab06_rtt", "RTT — próbki", new[]{"1","2","3","4","5","6","7","8","9","10"}, new[]{"RTT ms"}) },
            "4.5" => new[] {
                T("lab06_rtt_compare", "RTT — wariant v4", new[]{"A","B"}, new[]{"N","mean ms","min ms","max ms","Warunki"}),
                T("lab06_stale", "LIVE / STALE", new[]{"Pomiar"}, new[]{"próg STALE ms","joint_states Hz","pauza Gazebo s","detekcja STALE ms","odzyskanie LIVE ms"}) },
            "5.0" => new[] { T("lab06_fault", "Eksperyment diagnostyczny", new[]{"baseline","kontrolowany błąd","po naprawie"}, new[]{"Stan / wynik","joint1","joint2","Hz","RTT ms","Uwagi"}) },
            _ => Array.Empty<WiRRReportTable>()
        };

        private static IReadOnlyList<WiRRReportTable> Lab07(string cp) => cp switch
        {
            "3.0" => new[] {
                T("lab07_acceptance", "Kryteria akceptacji", new[]{"AC-01 uruchomienie","AC-02 funkcjonalność","AC-03 wydajność","AC-04 stabilność","AC-05 użyteczność","AC-06 bezpieczeństwo"}, new[]{"Kryterium","Źródło/uzasadnienie","Blokujące?"}),
                T("lab07_smoke", "Test dymny", new[]{"SM-01","SM-02","SM-03","SM-04","SM-05"}, new[]{"Warunek","Oczekiwany","Rzeczywisty","Status PASS/FAIL/NV","Dowód"}),
                T("lab07_functional", "Macierz funkcjonalna Lab 1–6", new[]{"Lab 1","Lab 2","Lab 3","Lab 4","Lab 5","Lab 6"}, new[]{"Funkcja/kontrakt","Procedura","Oczekiwany","Rzeczywisty","Status","Dowód"}) },
            "3.5" => new[] { T("lab07_performance", "Wydajność i stabilność", new[]{"bezczynność","nominalny","obciążeniowy"}, new[]{"FPS mediana","ms/kl mediana","CPU ms","GPU ms","Pamięć MB","Uwagi"}) },
            "4.0" => new[] { T("lab07_usability", "Użyteczność i dostępność", new[]{"U1","U2","U3"}, new[]{"Cel użytkownika","Sukces?","Czas/próby","Błędne aktywacje","Pomoc","Problem dostępności"}) },
            "4.5" => new[] { T("lab07_regression", "Test regresji", new[]{"REG-01","REG-02"}, new[]{"Test","Przed poprawką","Po poprawce","Status końcowy","Dowód"}) },
            "5.0" => new[] { T("lab07_risk", "Macierz ryzyka", new[]{"R-01","R-02","R-03"}, new[]{"Ryzyko","P","S","R","Dowód","Działanie ograniczające","Ryzyko resztkowe"}) },
            _ => Array.Empty<WiRRReportTable>()
        };
    }
}
