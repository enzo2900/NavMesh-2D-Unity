using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;
using Utility;

public class NavMeshEntity : MonoBehaviour
{
    [SerializeField] private NavMeshGenerator generator;
    NavMesh navMesh;

    private Face fStart;
    Face fEnd;
    [SerializeField] private GameObject targetPos;
    
    [SerializeField] private bool computedPathFinding;

    public bool reachedCurrentGoal;
    private Path currentPath;

    public Vector3 currentGoalPos;
    public Face getStart()
    {
        return fStart;
    }
    private void Awake()
    {
        reachedCurrentGoal = true;
        generator.navMeshInitialized += mesh =>
        {
            
            navMesh = mesh;
            var p = computePathFinding(targetPos.transform.position);
            currentPath = p;
        };
    }

    private void Update()
    {
        if (computedPathFinding && currentPath.path.Count != 0 && reachedCurrentGoal)
        {
            currentGoalPos = currentPath.path.First();
            currentPath.path.RemoveAt(0);
            reachedCurrentGoal = false;

        }

        if (Vector2.Distance(currentGoalPos, transform.position) > 0.3f)
        {
            float speed = 1;
            Vector3 direction = (currentGoalPos - transform.position).normalized;
            transform.position += speed * Time.deltaTime * direction;
            
        }
        else
        {
            reachedCurrentGoal = true;
        }
        
    }

    public struct Path
    {
        public List<Vector3> path;

        public Path(List<Vector3> path)
        {
            this.path = path;
        }
    }

    public Path computePathFinding(Vector2 target)
    {
        computedPathFinding = false;
        var path = pathfind(this.transform.position,target);
        computedPathFinding = true;
        return path;
    }
    
    private Path pathfind(Vector2 end,Vector2 start)
    {

        HashSet<Face> trianglesVisited = new HashSet<Face>();
        var fStart = navMesh.getFaceFromPoint(new Point(start.x,start.y));
        var fEnd = navMesh.getFaceFromPoint(new Point(end.x, end.y));
        this.fStart = fStart;
        this.fEnd = fEnd;
        var fCurrent = fStart;
        var endPos = getFaceCenterPos(fEnd);
        var startPos = getFaceCenterPos(fStart);
        Dictionary<Face,Face> parents = new Dictionary<Face,Face>();
        Dictionary<Face,double> distances = new Dictionary<Face,double>();
        Dictionary<Face,double> fScores = new Dictionary<Face,double>();
        foreach (var keyValuePair in navMesh.faces)
        {
            distances[keyValuePair.Key] = Double.PositiveInfinity;
            fScores[keyValuePair.Key] = Double.PositiveInfinity;
        }
        List<Face> queue = new List<Face>();
        queue.Add(fStart);
        parents.Add(fStart, fStart);
        distances[fStart] = 0;
        fScores[fStart] = 0 + (endPos -  startPos).magnitude; ;
        while (queue.Count > 0)
        {
            var current = getLowestDist(queue,fScores, endPos);
            if (current == fEnd)
            {
                
                return reconstructPath(parents,current);
            }

            queue.Remove(current);
            var neigbors = getFaceNeighbors(current);
            foreach (var neigbor in neigbors)
            {
                var posNeigbor = getFaceCenterPos(neigbor);
                var posCurrent = getFaceCenterPos(current);
                double p = distances[current] + (posNeigbor - posCurrent).magnitude;
                if (distances[neigbor] > p)
                {
                    
                    if (parents.ContainsKey(neigbor))
                    {
                        parents[neigbor] = current;
                        
                    }
                    else
                    {
                        parents.Add(neigbor, current);
                    }
                    
                    distances[neigbor] = p;
                    
                    fScores[neigbor] = p + (posNeigbor - endPos).magnitude;
                    if (!trianglesVisited.Contains(neigbor))
                    {
                        trianglesVisited.Add(neigbor);
                        queue.Add(neigbor);
                    }
                }
            }
            //path.Add(getFaceCenterPos(fCurrent));
        }
        Debug.LogError("Cant find a path");
        return new Path();
    }

    private Path reconstructPath(Dictionary<Face,Face> parents, Face end ) 
    {
        List<Vector3> path = new List<Vector3>();
        Face current = end;
        
        while (current != parents[current])
        {
            path.Add(getFaceCenterPos(current));
            current = parents[current];
        }
        path.Add(targetPos.transform.position);
        return new Path(path);
    }

    private Face getLowestDist(List<Face> queue, Dictionary<Face, double> fScore, Vector3 endPos)
    {
        double lowestDistance = double.MaxValue;
        Face lowest = null;
        foreach (var face in queue)
        {
            var pos = fScore[face];
            if (lowestDistance > pos)
            {
                lowestDistance = pos;
                lowest = face;
            }
        }

        return lowest;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.darkGreen;
        for (var i = 0; i < currentPath.path.Count -1; i++)
        {
            var point1 = currentPath.path[i];
            var point2 = currentPath.path[i + 1];
            Gizmos.DrawLine(point1, point2);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        for (var i = 0; i < currentPath.path.Count -1; i++)
        {
            var point1 = currentPath.path[i];
            var point2 = currentPath.path[i + 1];
            Gizmos.DrawLine(point1, point2);
        }
        Gizmos.color = Color.blue;
        foreach (var halfEdge in navMesh.faces[fStart])
        {
            draw(halfEdge);
        }
        Gizmos.color = Color.yellow;
        foreach (var halfEdge in navMesh.faces[fEnd])
        {
            draw(halfEdge);
        }
    }
    
    private void draw(HalfEdge halfEdge)
    {
        Vector3 to = new Vector3(halfEdge.v.x, halfEdge.v.y, 0);
        Vector3 from = new Vector3(halfEdge.next.v.x, halfEdge.next.v.y, 0);
        Gizmos.DrawLine(to, from);
    }

    private List<Face> getFaceNeighbors(Face face)
    {
        var edges = navMesh.faces[face];
        var i = edges[0];
        var j = edges[2];
        var k = edges[4];
        List<Face> neighbors = new List<Face>();

        var fI = i.twin.incidentFace;
        if (fI != null && fI != face)
        {
            neighbors.Add(fI);
        } 
        
        var fJ = j.twin.incidentFace;
        if (fJ != null && fJ != face)
        {
            neighbors.Add(fJ);
        } 
        
        var fK = k.twin.incidentFace;
        if (fK != null && fK != face)
        {
            neighbors.Add(fK);
        }

        return neighbors;

    }

    private Vector3 getFaceCenterPos(Face f)
    {
        var edges = navMesh.faces[f];
        var i = edges[0];
        var j = edges[2];
        var k = edges[4];
        
        float xCenter = (i.v.x + j.v.x +  k.v.x) /3;
        float yCenter = (i.v.y + j.v.y +  k.v.y) /3;
        return new Vector3(xCenter,yCenter,0);
    }
    
}
