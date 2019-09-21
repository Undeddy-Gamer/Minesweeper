using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 20, height = 20;
    public float spacing = .155f;

    private Tile[,] tiles;

    //Spawn a tile at specific location (x, y coords)
    void SpawnTile(int x, int y)
    {
        
        float halfWidth = (float)width / 2;
        float halfHeight = (float)height / 2;

        // Move the X position to centre
        float xPosition = x - halfWidth;
        float yPosition = y - halfHeight;


        GameObject clone = Instantiate(tilePrefab);
        //Reposition tile & Apply spacing
        clone.transform.position = new Vector2(xPosition + .5f, yPosition + .5f) * spacing;

        // Get the 'Tile' component from Clone
        Tile tile = clone.GetComponent<Tile>();
        //Tile to store the current X and Y values
        tile.x = x;
        tile.y = y;
        // Store Tile in 2D Array Location (using X and Y)
        tiles[x, y] = tile;


        
    }

    void GenerateGrid()
    {

        // Allocate memory  for tile 2D Array
        tiles = new Tile[width, height];


        // loop through the height and width set above to generate the grid
        for (int row = 0; row < width; row++)
        {
            for (int col = 0; col < height; col++)
            {
                // Spawn a new instance of tile
                SpawnTile(row, col);
            }
        }
        
    }

    void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
       
        // Check if mouse Button is Down (this frame)
        if (Input.GetMouseButtonDown(0))
        {
            // Run the method for selecting tile
            SelectATile();
        }
       
    }


    void SelectATile()
    {
        // Generate a ray from the Camera with mouse position
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Perform 2D Raycast
        RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
        // If the mouse hit something
        if (hit.collider != null)
        {
            // Try getting a tile component from the thing we hit
            Tile hitTile = hit.collider.GetComponent<Tile>();
            // Check if the thing we hit was a tile 
            if (hitTile != null)
            {
                SelectTile(hitTile);
            }

        }

    }

    public int GetAdjacentMineCount(Tile tile)
    {

        // set count to 0
        int count = 0;
        // look through all adjacent Tiles on x
        for (int x = -1; x <= 1; x++)
        {
            // loop through all adjacent tiles on Y
            for ( int y = -1; y <= 1; y++ )
            {
                // set x to tile.x + x
                int desiredX = tile.x + x;
                // set y to tile.y + y
                int desiredY = tile.y + y;

                // If x and y is out of bounds
                if (desiredX < 0 || desiredX >= width || desiredY < 0 || desiredY >= height)
                {
                    // Continue (to next loop)
                    continue;
                }

                Tile currentTile = tiles[desiredX, desiredY];
                // If current tile is a mine
                if (currentTile.isMine)
                {
                    // Set count to count +1
                    count++;
                }

            }
            
        }
        return count;
    }


    void FFuncover(int x, int y, bool[,] visited)
    {
        // Is x and Y withing bounds of the grid?
        if (x >= 0 && x < width && y >=0 && y < height)
        {
            //Have these coords been visited?
            if (visited[x, y])
                return;

            //Reveal tile in that x and y coordinate
            Tile tile = tiles[x, y];
            int adjacentMines = GetAdjacentMineCount(tile);
            tile.Reveal(adjacentMines);

            if(adjacentMines == 0)
            {
                // This tile has been visited
                visited[x, y] = true;
                //
                FFuncover(x - 1, y, visited);
                FFuncover(x + 1, y, visited);
                FFuncover(x, y - 1, visited);
                FFuncover(x, y + 1, visited);
            }

        }
    }

    void UncoverMines(int mineState = 0)
    {
        //Loop through 2D Array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];
                if (tile.isMine)
                {
                    // Reveal that tile
                    int adjacentMaines = GetAdjacentMineCount(tile);
                    tile.Reveal(adjacentMaines, mineState);
                }
            }
        }
    }

    //Scans the grid to check if there are no more empty tiles
    bool NoMoreEmptyTiles()
    {
        // Set empty tile count to zero
        int emptyTileCount = 0;

        //Loop through 2D array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];
                // If tile is NOT revealed AND NOT a mine
                if (!tile.isRevealed && !tile.isMine)
                {
                    // We found and empty tile!
                    emptyTileCount += 1;
                }
            }

        }
        
        // If there are empty tiles - return true
        // If there are no empty tiles - return false
        
        return emptyTileCount == 0;
    }

    void SelectTile(Tile selected)
    {
        int adjacentMines = GetAdjacentMineCount(selected);
        selected.Reveal(adjacentMines);

        if (selected.isMine)
        {
            UncoverMines();
        }
        else if (adjacentMines == 0)
        {
            int x = selected.x;
            int y = selected.y;
            // Then use flood fill to uncover all adjacent mines

            FFuncover(x, y, new bool[width, height]);
        }

        //Are there no more empty tiles in the game at this point?
        if (NoMoreEmptyTiles())
        {
            // Uncover all mines - with the state '1'
            UncoverMines(1);
        }

    }
}
