# 2D Dynamic NavMesh & Entity Pathfinding for Unity

A dynamic 2D Navigation Mesh generator and agent movement system for Unity. It bakes navmesh geometry at runtime considering scene obstacles.

## 🛠️ Components

### 1. `NavmeshGenerator`
Attached to a central Manager object. Scans the environment, detects colliders, and constructs the walkable mesh.

* **Obstacle Detection**: Automatically identifies scene colliders tagged as `Obstacle`.
* **Start baking**: Create a geometry on start.
* **Mesh Generation**: Converts walkable 2D space into a traversable navigation graph.

### 2. `NavmeshEntities`
Attached to any moving agent (NPCs, AI units).

* **A** Pathfinding**: Executes an internal A* algorithm over the NavMesh nodes to calculate the shortest path.
* **Waypoint Navigation**: Translates calculated paths into direct waypoint-to-waypoint movement.
---

## 🚀 Quick Setup

1. **Tag Obstacles**: Select your environment obstacle GameObjects, add a 2D Collider (`BoxCollider2D`, `PolygonCollider2D`, etc.), and set their Tag to `Obstacles`.
2. **Setup Generator**:
   * Create an empty GameObject named `NavmeshManager`.
   * Attach the `NavmeshGenerator` script.
   * Setup the boundary of the generation.
   * Setup a tag name for obstacles. Exemple: 'Obstacles'.
3. **Setup Agents**:
   * Attach `NavmeshEntities` to your agent GameObject.
   * Attach the generator.
   * Place a target position.
4. **Run Scene**: Click Play — the mesh will bake automatically and entities will navigate around obstacles.
