using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System.Globalization;

namespace WebGLSharp
{
    public class Geometry
    {
        List<Face> _faces;

        private Geometry(List<Face> faces)
        {
            _faces = faces;
        }

        public static Geometry Parse(string objFileContent)
        {
            var positionRegex = new Regex(@"^v\s+([\d\.\+\-eE]+)\s+([\d\.\+\-eE]+)\s+([\d\.\+\-eE]+)");
            var normalRegex = new Regex(@"^vn\s+([\d\.\+\-eE]+)\s+([\d\.\+\-eE]+)\s+([\d\.\+\-eE]+)");
            var uvRegex = new Regex(@"^vt\s+([\d\.\+\-eE]+)\s+([\d\.\+\-eE]+)");
            var faceRegex = new Regex(@"^f\s+(-?\d+)\/(-?\d+)\/(-?\d+)\s+(-?\d+)\/(-?\d+)\/(-?\d+)\s+(-?\d+)\/(-?\d+)\/(-?\d+)(?:\s+(-?\d+)\/(-?\d+)\/(-?\d+))?");


            List<Vector3> positions = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Face> faces = new List<Face>();
            var lines = objFileContent.Split('\n');
            foreach (var line in lines)
            {
                if (positionRegex.IsMatch(line))
                {
                    var match = positionRegex.Match(line);
                    Console.WriteLine($"{match.Groups[1].Value} => {match.Groups[2].Value} => {match.Groups[3].Value}");
                    positions.Add(new Vector3(
                        float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
                        float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
                        float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture)));
                }
                else if (normalRegex.IsMatch(line))
                {
                    var match = normalRegex.Match(line);
                    normals.Add(new Vector3(
                        float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
                        float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
                        float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture)));
                }
                else if (uvRegex.IsMatch(line))
                {
                    var match = uvRegex.Match(line);
                    uvs.Add(new Vector2(float.Parse(
                        match.Groups[1].Value, CultureInfo.InvariantCulture),
                        1 - float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture)));
                }
                else if (faceRegex.IsMatch(line))
                {
                    var verticies = new List<Vertex>();
                    var match = faceRegex.Match(line);
                    for (int i = 1; i < 10; i += 3)
                    {
                        verticies.Add(new Vertex(
                            positions[int.Parse(match.Groups[i].Value) - 1],
                            normals[int.Parse(match.Groups[i+2].Value) - 1],
                            uvs[int.Parse(match.Groups[i+1].Value) - 1]
                            ));
                    }
                    faces.Add(new Face(verticies));
                }
            }
            return new Geometry(faces);
        }
        internal int GetVertexCount()
        {
            return _faces.Count * 3;
        }
        internal float[] GetPositions()
        {
            var res = new List<float>();
            foreach (var face in _faces)
            {
                foreach (var vertex in face.Verticies)
                {
                    res.Add(vertex.Position.X);
                    res.Add(vertex.Position.Y);
                    res.Add(vertex.Position.Z);
                }
            }
            return res.ToArray();
        }
        internal float[] GetNormals()
        {
            var res = new List<float>();
            foreach (var face in _faces)
            {
                foreach (var vertex in face.Verticies)
                {
                    res.Add(vertex.Normal.X);
                    res.Add(vertex.Normal.Y);
                    res.Add(vertex.Normal.Z);
                }
            }
            return res.ToArray();
        }
        internal float[] GetUvs()
        {
            var res = new List<float>();
            foreach (var face in _faces)
            {
                foreach (var vertex in face.Verticies)
                {
                    res.Add(vertex.Uv.X);
                    res.Add(vertex.Uv.Y);
                }
            }
            return res.ToArray();
        }

        public struct Face
        {
            public List<Vertex> Verticies { get; set; }

            public Face(List<Vertex> verticies)
            {
                Verticies = verticies;
            }
        }
        public struct Vertex
        {
            public Vector3 Position { get; set; }
            public Vector3 Normal { get; set; }
            public Vector2 Uv { get; set; }

            public Vertex(Vector3 position, Vector3 normal, Vector2 uv)
            {
                Position = position;
                Normal = normal;
                Uv = uv;
            }
        }
    }
}