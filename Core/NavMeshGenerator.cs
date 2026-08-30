using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;
public class NavMeshGenerator : MonoBehaviour
{
    private SubdivisionBuilder subdivisionBuilder;

    [SerializeField]
    private float maxY;
    [SerializeField]
    private float minY;

    [SerializeField]
    private float maxX;
    [SerializeField]
    private float minX;

    public Action<NavMesh> navMeshInitialized;
    [SerializeField] private string obtaclesTag;

    [SerializeField] private float offset;
    
    [SerializeField] private bool gizmos;
    
    void Start()
    {
        List<Point> points = new List<Point>();
        points.Add(new Point(minX, maxY));
        points.Add(new Point(minX, minY));
        points.Add(new Point(maxX, maxY));
        points.Add(new Point(maxX, minY));
        List<CDT.ConstraintPolygon> contraintsPolygons = new List<CDT.ConstraintPolygon>();
        
        GameObject[] obtacles = GameObject.FindGameObjectsWithTag(obtaclesTag);
        foreach (var obtacle in obtacles)
        {
            List<Segment<Vertex>> contraintsSegments = new List<Segment<Vertex>>();
            if (obtacle.TryGetComponent(out Collider2D collider))
            {
                List<Vertex> vertices = getVerticesFromCollider(collider);
                if (vertices.Count > 3)
                {
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        int indexNegative = i - 1;
                        if (indexNegative < 0)
                        {
                            indexNegative +=  vertices.Count ;
                        }
                        Vertex vBefore = vertices[indexNegative];
                        Vertex vCurrent = vertices[i];
                        Vertex vAfter = vertices[(i+1) % vertices.Count];
                        float angle1 = Mathf.Atan2(vAfter.y - vCurrent.y, vAfter.x - vCurrent.x);
                        float angle2 = Mathf.Atan2(vBefore.y - vCurrent.y, vBefore.x - vCurrent.x);
                        float angleOffset = (angle2 - angle1) / 2 + angle1;
                        float direction =  vBefore.x - vCurrent.x + vBefore.y - vCurrent.y;
                        if (direction > -1)
                        {
                            angleOffset -= 180;
                        }

                        vCurrent.x += offset * Mathf.Cos(angleOffset);
                        vCurrent.y += offset * Mathf.Sin(angleOffset);
                    }
                }
                if (vertices.Count >= 2)
                {
                    for(int i = 0; i < vertices.Count; i++)
                    {
                        Vertex vertexi = vertices[i];
                        Vertex vertexi2 = vertices[(i+1) % vertices.Count];
                        contraintsSegments.Add(new Segment<Vertex>(vertexi,vertexi2));
                        points.Add(new Point(vertexi.x,vertexi.y));
                    }    
                }
                else
                {
                    Vertex vertexi = vertices[0];
                    points.Add(new Point(vertexi.x,vertexi.y));
                }
            }

            if (contraintsSegments.Count != 0)
            {
                var polygon = new CDT.ConstraintPolygon();
                polygon.segments = contraintsSegments;
                contraintsPolygons.Add(polygon);
            }
            
        }
        subdivisionBuilder = CDT.compute(points);
        CDT.applyConstraints(contraintsPolygons,subdivisionBuilder);
        CDT.removePolygons(subdivisionBuilder,contraintsPolygons);
        navMeshInitialized?.Invoke(getNavMesh());
    }

    public NavMesh getNavMesh()
    {
        return new NavMesh( subdivisionBuilder.vertices,subdivisionBuilder.faces);
    }
    
    /// <summary>
    /// Donne la représentation des sommets du collider.
    /// Les sommets sont donnés dans le sens horaire.
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    private List<Vertex> getVerticesFromCollider(Collider2D collider)
    {
        List<Vertex> vertices = new List<Vertex>();
        Vector3 scale = collider.gameObject.transform.localScale;
        float zRotation = collider.gameObject.transform.rotation.eulerAngles.z;
        float zRotationRadions = zRotation * Mathf.Deg2Rad;
        Matrix2x2 matrix2 = new Matrix2x2(new Vector2(Mathf.Cos(zRotationRadions), Mathf.Sin(zRotationRadions)),
            new Vector2(-Mathf.Sin(zRotationRadions), Mathf.Cos(zRotationRadions)));
        if (collider is CircleCollider2D circle)
        {
            Debug.Log("Circle collider " + circle.name );
            float radius = circle.radius;
            Vector2 center = circle.gameObject.transform.position;
            var scaleY45 = radius * scale.y * Mathf.Sin(Mathf.PI/4);
            var scaleX45 = radius * scale.x * Mathf.Cos(Mathf.PI/4);
            vertices.Add(toVertex(center + new Vector2(0, radius * scale.y)));

            vertices.Add(toVertex(center + new Vector2(scaleX45, scaleY45)));
            vertices.Add(toVertex(center+ new Vector2(radius*scale.x,0)));
            vertices.Add(toVertex(center + new Vector2(scaleX45, -scaleY45)));

            vertices.Add(toVertex(center+ new Vector2(0,-radius * scale.y)));
            vertices.Add(toVertex(center + new Vector2(-scaleX45, -scaleY45)));

            vertices.Add(toVertex(center+ new Vector2(-radius*scale.x,0)));
            vertices.Add(toVertex(center + new Vector2(-scaleY45, scaleY45)));
        } else if (collider is BoxCollider2D box)
        {
            Debug.Log("Box collider " + box.name);
            Vector2 center = box.gameObject.transform.position;
            Vector2 size = box.size * scale/2;
            vertices.Add(toVertex(center
                                  + matrix2.applyTransformation(new Vector2(size.x, size.y))));
            vertices.Add(toVertex(center
                                  + matrix2.applyTransformation(new Vector2(size.x, -size.y))));
            vertices.Add(toVertex(center
                                  + matrix2.applyTransformation(new Vector2(-size.x, -size.y))));
            vertices.Add(toVertex(center
                                  + matrix2.applyTransformation(new Vector2(-size.x, size.y))));
        }else if (collider is PolygonCollider2D polygon)
        {
            Debug.Log("Polygon collider " + polygon.name + polygon.points.Length);
            Vector2 center = polygon.gameObject.transform.position;
            var listP = polygon.points.ToList();
            listP.Sort(((vector2, vector3) =>
            {
                Vector2 normalizedV2 =  vector2.normalized;
                Vector2 normalizedV3 =  vector3.normalized;
                
                float rad = Mathf.Atan2(normalizedV2.y, normalizedV2.x);
                float rad2 = Mathf.Atan2(normalizedV3.y, normalizedV3.x);
                
                if (rad > rad2)
                {
                    return -1;
                } else if (Mathf.Approximately(rad, rad2))
                {
                    return 0;
                }

                return 1;
            }));
            foreach (var polygonPoint in listP)
            {
                vertices.Add(toVertex(center+matrix2.applyTransformation(polygonPoint *scale)));
            }
        } else if (collider is TilemapCollider2D tilemap)
        {
            Debug.Log("CompositeCollider 2D");
            var mesh = tilemap.CreateMesh(true, true);
            
            List<Point> points = new List<Point>();
            foreach (var polygonPoint in mesh.triangles)
            {
                vertices.Add(toVertex(mesh.vertices[polygonPoint]));
            }
        }
        
        vertices.Reverse();
        return vertices;
    }

    private Vertex toVertex(Vector2 pos)
    {
        return new Vertex(pos.x,pos.y);
    }

    private void OnDrawGizmos()
    {
        if (!gizmos) return;
        if(subdivisionBuilder == null) return;
        Gizmos.color = Color.red;
        foreach (var subdivisionBuilderFace in subdivisionBuilder.faces)
        {
            var list = subdivisionBuilderFace.Value;
            
            foreach (var halfEdge in list)
            {
                draw(halfEdge);
            }
        }
    }

    private void draw(HalfEdge halfEdge)
    {
        Vector3 to = new Vector3(halfEdge.v.x, halfEdge.v.y, 0);
        Vector3 from = new Vector3(halfEdge.next.v.x, halfEdge.next.v.y, 0);
        Gizmos.DrawLine(to, from);
    }
    
}
