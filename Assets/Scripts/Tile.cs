using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Tile : MonoBehaviour
{

    public int x, y;                                    // x & y Coordinate grid
    public bool isMine = false, isRevealed = false;     // Is it a mine, is it a tile
    [Header("References")]
    public Sprite[] emptySprites;                       //Reference to empty sprites
    public Sprite[] mineSprites;                        // state of mine
    private SpriteRenderer rend;                        //Reference to sprite renderer


    public GameObject pete;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        isMine = Random.value < .05f;
    }


    public void Reveal(int adjacentMines, int mineState = 0)
    {
        //Flag the tile as being revealed
        isRevealed = true;
        // Check if tile is a mine
        if (isMine)
        {
            // Set sprite to mine sprite
            rend.sprite = mineSprites[mineState];
            rend.sortingOrder = 5;
            pete.SetActive(true);
        }
        else
        {
            // set sprite to appropriate texture based on adjacent tiles
            rend.sprite = emptySprites[adjacentMines];
        }


    }
}

    