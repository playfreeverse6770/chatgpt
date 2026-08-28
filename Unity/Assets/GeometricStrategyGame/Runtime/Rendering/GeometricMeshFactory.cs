using System.Collections.Generic;
using UnityEngine;

namespace GeometricStrategy
{
    public static class GeometricMeshFactory
    {
        public static Mesh Create(GeometricSymbol symbol, float radius = 0.5f)
        {
            switch (symbol)
            {
                case GeometricSymbol.Triangle: return RegularPolygon(3, radius, 90f);
                case GeometricSymbol.Square: return RegularPolygon(4, radius, 45f);
                case GeometricSymbol.Sun: return Star(12, radius, radius * 0.72f, 90f);
                case GeometricSymbol.SixPointStar: return Star(6, radius, radius * 0.48f, 90f);
                case GeometricSymbol.Trapezoid: return Polygon(new[]
                {
                    new Vector2(-radius * 0.72f, -radius),
                    new Vector2(radius * 0.72f, -radius),
                    new Vector2(radius, radius),
                    new Vector2(-radius, radius)
                });
                case GeometricSymbol.Pentagon: return RegularPolygon(5, radius, 90f);
                case GeometricSymbol.Hexagon: return RegularPolygon(6, radius, 90f);
                case GeometricSymbol.Diamond: return RegularPolygon(4, radius, 90f);
                case GeometricSymbol.Octagon: return RegularPolygon(8, radius, 22.5f);
                default: return RegularPolygon(32, radius, 90f);
            }
        }

        private static Mesh RegularPolygon(int sides, float radius, float degreesOffset)
        {
            var points = new Vector2[sides];
            float offset = degreesOffset * Mathf.Deg2Rad;
            for (int i = 0; i < sides; i++)
            {
                float angle = offset + (Mathf.PI * 2f * i / sides);
                points[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            }
            return Polygon(points);
        }

        private static Mesh Star(int points, float outerRadius, float innerRadius, float degreesOffset)
        {
            int vertexCount = points * 2;
            var polygon = new Vector2[vertexCount];
            float offset = degreesOffset * Mathf.Deg2Rad;
            for (int i = 0; i < vertexCount; i++)
            {
                float radius = (i % 2 == 0) ? outerRadius : innerRadius;
                float angle = offset + (Mathf.PI * 2f * i / vertexCount);
                polygon[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            }
            return Polygon(polygon);
        }

        private static Mesh Polygon(IReadOnlyList<Vector2> points)
        {
            var mesh = new Mesh { name = "GeometricSymbolMesh" };
            var vertices = new Vector3[points.Count + 1];
            var uv = new Vector2[vertices.Length];
            var triangles = new int[points.Count * 3];

            vertices[0] = Vector3.zero;
            uv[0] = new Vector2(0.5f, 0.5f);

            for (int i = 0; i < points.Count; i++)
            {
                vertices[i + 1] = new Vector3(points[i].x, points[i].y, 0f);
                uv[i + 1] = points[i] + Vector2.one * 0.5f;

                int next = (i + 1) % points.Count;
                int tri = i * 3;
                triangles[tri] = 0;
                triangles[tri + 1] = i + 1;
                triangles[tri + 2] = next + 1;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
