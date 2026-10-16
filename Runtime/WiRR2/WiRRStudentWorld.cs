using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace KIA.WiRR
{
    /// <summary>Small, local teaching geometry. No imported textures, CAD SDK or robot connection.</summary>
    public sealed class WiRRStudentWorld : IDisposable
    {
        public readonly GameObject root;
        public readonly Camera camera;
        public readonly List<GameObject> targets = new List<GameObject>();
        public Transform socket, anchorReference, robotJoint1, robotJoint2;
        public Mesh activeMesh;
        public LODGroup lod;
        private readonly List<UnityEngine.Object> owned = new List<UnityEngine.Object>();

        public WiRRStudentWorld(Transform parent, int lab, int count, int detail, bool shadows, bool physics, bool highlight, int view)
        {
            root = new GameObject("Materiały doświadczenia");
            root.transform.SetParent(parent, false);
            var cameraObject = new GameObject("Kamera doświadczenia");
            cameraObject.transform.SetParent(root.transform);
            camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.08f, 0.11f, 0.16f);
            camera.transform.position = new Vector3(view == 0 ? 0 : 2.8f, 2.2f, -6f);
            camera.transform.LookAt(new Vector3(0, 1, 1));
            var lamp = new GameObject("Światło");
            lamp.transform.SetParent(root.transform);
            var light = lamp.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.5f;
            light.shadows = shadows ? LightShadows.Soft : LightShadows.None;
            lamp.transform.rotation = Quaternion.Euler(50, -25, 0);
            Shape("Podłoże", PrimitiveType.Cube, new Vector3(0, -0.12f, 2), new Vector3(24, 0.2f, 24), Color.gray);
            if (lab == 1)
            {
                foreach (var scale in new[] { 0.1f, 1f, 10f })
                {
                    var reference = Shape("Skala " + scale + " m", PrimitiveType.Cube,
                        new Vector3(scale < 1 ? -2 : scale < 10 ? 0 : 9, scale / 2, scale < 10 ? 5 : 12), Vector3.one * scale, Color.cyan);
                    Label(reference.transform, scale + " m", new Vector3(0, 0.7f, 0));
                }
                for (var i = 0; i < count; i++)
                {
                    var cube = Shape("Obciążenie " + i, PrimitiveType.Cube,
                        new Vector3((i % 10 - 4.5f) * 0.45f, 0.3f + (i / 10) * 0.5f, 1), Vector3.one * 0.35f, Color.yellow);
                    if (physics) cube.AddComponent<Rigidbody>();
                }
            }
            else if (lab == 2)
            {
                targets.Add(Shape("Chwytany obiekt", PrimitiveType.Cube, new Vector3(-1, 1, 1), Vector3.one * 0.3f, Color.cyan));
                socket = Shape("Gniazdo", PrimitiveType.Cube, new Vector3(1, 1, 1),
                    Vector3.one * 0.5f, highlight ? Color.green : Color.gray).transform;
                Label(socket, "GNIAZDO", new Vector3(0, 0.8f, 0));
            }
            else if (lab == 3)
            {
                for (var i = 0; i < 3; i++)
                {
                    var reference = Shape("Referencja " + i, PrimitiveType.Cylinder,
                        new Vector3((i - 1) * 0.3f, 0.7f, 1), new Vector3(0.04f, 0.7f, 0.04f), Color.green);
                    if (i == 1) anchorReference = reference.transform;
                }
                targets.Add(Shape("Kotwica SIM_AR", PrimitiveType.Sphere, anchorReference.position,
                    Vector3.one * 0.12f, Color.yellow));
                Label(anchorReference, "Odcinki 0,30 m; SIM_AR", new Vector3(0, 1, 0));
            }
            else if (lab == 4)
            {
                Shape("Przeszkoda", PrimitiveType.Cube, new Vector3(0, 1, 1), new Vector3(1.3f, 2, 0.15f), Color.gray);
                targets.Add(Shape("Obiekt 1 za przeszkodą", PrimitiveType.Sphere, new Vector3(0, 1, 2), Vector3.one * 0.4f, Color.red));
                targets.Add(Shape("Obiekt 2 przed przeszkodą", PrimitiveType.Sphere, new Vector3(0.4f, 1.5f, 0), Vector3.one * 0.4f, Color.green));
                targets.Add(Shape("Obiekt 3 z boku", PrimitiveType.Sphere, new Vector3(2, 1, 2), Vector3.one * 0.4f, Color.cyan));
                if (detail == 0)
                {
                    var shader = Shader.Find("WiRR/TeachingThroughWall");
                    if (shader == null) throw new InvalidOperationException("Brak shadera demonstracji okluzji.");
                    var material = new Material(shader); owned.Add(material);
                    material.SetColor("_Color", Color.red);
                    targets[0].GetComponent<Renderer>().sharedMaterial = material;
                }
            }
            else if (lab == 5)
            {
                var group = new GameObject("Część demonstracyjna LOD"); group.transform.SetParent(root.transform);
                group.transform.position = new Vector3(0, 1.2f, 1);
                lod = group.AddComponent<LODGroup>();
                var levels = new LOD[3];
                for (var level = 0; level < 3; level++)
                {
                    var mesh = RingMesh(level == 0 ? 96 : level == 1 ? 24 : 6, level != 2);
                    owned.Add(mesh);
                    var part = new GameObject("Siatka " + level); part.transform.SetParent(group.transform, false);
                    part.AddComponent<MeshFilter>().sharedMesh = mesh;
                    var renderer = part.AddComponent<MeshRenderer>(); renderer.sharedMaterial = Material(Color.cyan);
                    levels[level] = new LOD(level == 0 ? 0.35f : level == 1 ? 0.12f : 0.01f, new Renderer[] { renderer });
                    if (level == detail) activeMesh = mesh;
                }
                lod.SetLODs(levels); lod.RecalculateBounds(); lod.ForceLOD(detail);
                Label(group.transform, "Część proceduralna; Low traci otwór", new Vector3(0, 1.2f, 0));
            }
            else if (lab == 6)
            {
                robotJoint1 = new GameObject("joint1").transform; robotJoint1.SetParent(root.transform);
                robotJoint1.position = new Vector3(0, 0.2f, 1);
                var arm = Shape("Ramię 1", PrimitiveType.Cube, Vector3.zero, new Vector3(0.15f, 1.2f, 0.15f), Color.cyan);
                arm.transform.SetParent(robotJoint1, false); arm.transform.localPosition = Vector3.up * 0.6f;
                robotJoint2 = new GameObject("joint2").transform; robotJoint2.SetParent(robotJoint1, false);
                robotJoint2.localPosition = Vector3.up * 1.2f;
                var forearm = Shape("Ramię 2", PrimitiveType.Cube, Vector3.zero, new Vector3(0.15f, 0.9f, 0.15f), Color.yellow);
                forearm.transform.SetParent(robotJoint2, false); forearm.transform.localPosition = Vector3.up * 0.45f;
                Label(root.transform, "Model dydaktyczny RR; ROS_REPLAY", new Vector3(0, 3, 1));
            }
            else
            {
                for (var i = 0; i < 3; i++)
                {
                    var target = Shape("Cel " + (i + 1), PrimitiveType.Cube,
                        new Vector3((i - 1) * 1.3f, 1, 1 + view), Vector3.one * 0.45f,
                        highlight ? Color.yellow : new Color(0.35f, 0.4f, 0.45f));
                    Label(target.transform, (i + 1).ToString(), Vector3.up * 0.8f); targets.Add(target);
                }
            }
        }

        public GameObject Shape(string name, PrimitiveType type, Vector3 position, Vector3 scale, Color color)
        {
            var shape = GameObject.CreatePrimitive(type); shape.name = name;
            shape.transform.SetParent(root.transform); shape.transform.position = position; shape.transform.localScale = scale;
            shape.GetComponent<Renderer>().sharedMaterial = Material(color);
            return shape;
        }

        private Material Material(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null) throw new InvalidOperationException("Brak shadera URP Lit/Standard.");
            var material = new Material(shader); owned.Add(material); material.color = color; return material;
        }

        private void Label(Transform parent, string text, Vector3 offset)
        {
            var label = new GameObject("Opis"); label.transform.SetParent(parent, false); label.transform.localPosition = offset;
            var mesh = label.AddComponent<TextMesh>(); mesh.text = text; mesh.characterSize = 0.06f; mesh.fontSize = 36;
            mesh.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.GetComponent<Renderer>().sharedMaterial = mesh.font.material;
            mesh.anchor = TextAnchor.MiddleCenter;
        }

        public bool Occluded(int target)
        {
            var position = targets[target].transform.position;
            var ray = new Ray(camera.transform.position, position - camera.transform.position);
            return Physics.Raycast(ray, out var hit, Vector3.Distance(position, ray.origin) + 0.3f)
                && hit.transform != targets[target].transform;
        }

        public static Mesh RingMesh(int segments, bool hole)
        {
            if (segments < 3) throw new ArgumentOutOfRangeException(nameof(segments));
            var vertices = new List<Vector3>(); var triangles = new List<int>();
            // Four rings: front outer/inner, back outer/inner; separate face vertices
            // preserve a real through-hole in High/Medium. Low intentionally fills it.
            for (var side = 0; side < 2; side++)
            for (var radius = 0; radius < 2; radius++)
            for (var i = 0; i < segments; i++)
            {
                var angle = 2 * Mathf.PI * i / segments;
                var r = radius == 0 ? 0.8f : hole ? 0.3f : 0;
                vertices.Add(new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, side == 0 ? -0.12f : 0.12f));
            }
            for (var i = 0; i < segments; i++)
            {
                var next = (i + 1) % segments;
                if (hole)
                {
                    Quad(triangles, i, next, segments + next, segments + i);
                    Quad(triangles, 2 * segments + i, 3 * segments + i, 3 * segments + next, 2 * segments + next);
                }
                else
                {
                    triangles.AddRange(new[] { i, segments + i, next,
                        2 * segments + i, 2 * segments + next, 3 * segments + i });
                }
                Quad(triangles, i, 2 * segments + i, 2 * segments + next, next);
                if (hole) Quad(triangles, segments + i, segments + next, 3 * segments + next, 3 * segments + i);
            }
            var mesh = new Mesh { name = hole ? "Pierścień z otworem" : "Uproszczenie bez otworu" };
            mesh.SetVertices(vertices); mesh.SetTriangles(triangles, 0); mesh.RecalculateNormals(); mesh.RecalculateBounds();
            return mesh;
        }

        private static void Quad(List<int> indices, int a, int b, int c, int d) =>
            indices.AddRange(new[] { a, c, b, a, d, c });

        public void Dispose()
        {
            if (root != null) { root.SetActive(false); UnityEngine.Object.Destroy(root); }
            foreach (var asset in owned) UnityEngine.Object.Destroy(asset);
        }
    }
}
