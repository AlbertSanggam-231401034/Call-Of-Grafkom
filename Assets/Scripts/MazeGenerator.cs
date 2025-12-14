using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Header("Main Prefabs")]
    public MazeCell cellPrefab; // Masukkan Prefab 'MazeCell' di sini
    public GameObject enemyPrefab; // Masukkan Prefab 'EnemyCCTV' di sini
    public GameObject playerPrefab;
    public GameObject exitPrefab;

    [Header("Settings")]
    public int mazeWidth = 10;
    public int mazeHeight = 10;
    public float cellSize = 4f; // Jarak antar cell (Sesuaikan dengan ukuran lantai MazeCell)

    [Header("Enemy Settings")]
    public float enemyHeight = 1.5f; // TINGGI ENEMY (Turunkan jika ketinggian)
    public int enemyCount = 5;

    private MazeCell[,] _mazeGrid;

    void Start()
    {
        GenerateMaze();
    }

    void GenerateMaze()
    {
        _mazeGrid = new MazeCell[mazeWidth, mazeHeight];

        // 1. Spawn Grid Kosong (Semua dinding tertutup)
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeHeight; z++)
            {
                // Posisikan cell berdasarkan cellSize
                Vector3 pos = new Vector3(x * cellSize, 0, z * cellSize);
                MazeCell newCell = Instantiate(cellPrefab, pos, Quaternion.identity);

                newCell.transform.parent = transform;
                newCell.name = $"Cell {x},{z}";

                // Simpan di array
                _mazeGrid[x, z] = newCell;
            }
        }

        // 2. Algoritma Pembuka Jalan (Recursive Backtracker)
        // Ini yang membuat labirin "Tipis" dan menyambung sempurna
        WaitForSeconds delay = new WaitForSeconds(0.01f); // Animasi pelan (bisa dihapus biar instan)
        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        Vector2Int startCoords = new Vector2Int(0, 0);
        _mazeGrid[0, 0].Visit();
        stack.Push(startCoords);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Peek();
            var neighbors = GetUnvisitedNeighbors(current);

            if (neighbors.Count > 0)
            {
                Vector2Int chosen = neighbors[Random.Range(0, neighbors.Count)];
                RemoveWalls(current, chosen);

                _mazeGrid[chosen.x, chosen.y].Visit();
                stack.Push(chosen);

                // yield return delay; // Uncomment baris ini kalau mau lihat animasi pembentukan
            }
            else
            {
                stack.Pop();
            }
        }

        // 3. Spawn Player (Kecil) di (0,0)
        Vector3 startPos = _mazeGrid[0, 0].transform.position;
        startPos.y = 0.5f;
        GameObject player = Instantiate(playerPrefab, startPos, Quaternion.identity);
        player.name = "Player";
        // Pastikan player kecil sesuai request
        player.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        // 4. Spawn Exit di ujung
        Vector3 endPos = _mazeGrid[mazeWidth - 1, mazeHeight - 1].transform.position;
        endPos.y = 0.5f;
        Instantiate(exitPrefab, endPos, Quaternion.identity);

        // 5. Spawn Enemy di Dinding
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        int spawned = 0;
        int attempts = 0;

        while (spawned < enemyCount && attempts < 1000)
        {
            attempts++;
            int x = Random.Range(1, mazeWidth);
            int z = Random.Range(1, mazeHeight);

            // Jangan spawn di area start
            if (x < 3 && z < 3) continue;

            MazeCell cell = _mazeGrid[x, z];

            // Cari dinding yang AKTIF (masih ada) untuk ditempeli CCTV
            List<Transform> walls = new List<Transform>();
            List<Vector3> looks = new List<Vector3>(); // Arah pandang CCTV

            // Cek dinding Top (Utara)
            if (cell.wallTop.activeSelf) { walls.Add(cell.wallTop.transform); looks.Add(Vector3.back); }
            // Cek dinding Bottom (Selatan)
            if (cell.wallBottom.activeSelf) { walls.Add(cell.wallBottom.transform); looks.Add(Vector3.forward); }
            // Cek dinding Left (Barat)
            if (cell.wallLeft.activeSelf) { walls.Add(cell.wallLeft.transform); looks.Add(Vector3.right); }
            // Cek dinding Right (Timur)
            if (cell.wallRight.activeSelf) { walls.Add(cell.wallRight.transform); looks.Add(Vector3.left); }

            if (walls.Count > 0)
            {
                int r = Random.Range(0, walls.Count);

                // Posisi: Di dinding tersebut, geser sedikit ke dalam, tinggi sesuai setting
                Vector3 wallPos = walls[r].position;
                Vector3 spawnPos = wallPos + (looks[r] * 0.4f); // 0.4f adalah offset biar ga tenggelam di tembok
                spawnPos.y = enemyHeight; // Tinggi CCTV diatur di Inspector

                Instantiate(enemyPrefab, spawnPos, Quaternion.LookRotation(looks[r]));
                spawned++;
            }
        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int c)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        if (c.x > 0 && !_mazeGrid[c.x - 1, c.y].IsVisited) list.Add(new Vector2Int(c.x - 1, c.y));
        if (c.x < mazeWidth - 1 && !_mazeGrid[c.x + 1, c.y].IsVisited) list.Add(new Vector2Int(c.x + 1, c.y));
        if (c.y > 0 && !_mazeGrid[c.x, c.y - 1].IsVisited) list.Add(new Vector2Int(c.x, c.y - 1));
        if (c.y < mazeHeight - 1 && !_mazeGrid[c.x, c.y + 1].IsVisited) list.Add(new Vector2Int(c.x, c.y + 1));
        return list;
    }

    void RemoveWalls(Vector2Int a, Vector2Int b)
    {
        MazeCell cellA = _mazeGrid[a.x, a.y];
        MazeCell cellB = _mazeGrid[b.x, b.y];

        if (a.x < b.x) { cellA.ClearWall(4); cellB.ClearWall(3); } // A kiri B
        else if (a.x > b.x) { cellA.ClearWall(3); cellB.ClearWall(4); } // A kanan B
        else if (a.y < b.y) { cellA.ClearWall(1); cellB.ClearWall(2); } // A bawah B
        else if (a.y > b.y) { cellA.ClearWall(2); cellB.ClearWall(1); } // A atas B
    }
}