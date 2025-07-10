using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Rummikub.Entities;

public class TileCollection
{
    private int[] tiles = new int[52];

    public TileCollection()
    {
        
    }

    public TileCollection(Hand hand)
    {
        List<Tile> tiles = hand.GetTiles();

        foreach (Tile tile in tiles)
        {
            Add(tile);
        }
    }

    public TileCollection(Board board)
    {
        List<Tile> tiles = board.GetTiles();

        foreach (Tile tile in tiles)
        {
            Add(tile);
        }
    }

    public List<Tile> GetTiles()
    {
        List<Tile> tiles = [];

        for (int i = 0; i < this.tiles.Length; i++)
        {
            if (this.tiles[i] > 0)
            {
                tiles.Add(new Tile(i));
            }

            if (this.tiles[i] > 1)
            {
                tiles.Add(new Tile(i));
            }
        }
        
        return tiles;
    }

    public void Add(Tile tile)
    {
        tiles[TileToIndex(tile)]++;
    }

    public void AddRange(List<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            Add(tile);
        }
    }

    public bool Remove(Tile tile)
    {
        if (tiles[TileToIndex(tile)] == 0) return false;
        tiles[TileToIndex(tile)]--;
        return true;
    }

    private int TileToIndex(Tile tile)
    {
        ConsoleColor colour = tile.Colour();
        int number = tile.Number();
        int colourNumber = 0;

        switch (colour)
        {
            case ConsoleColor.Red:
                colourNumber = 0;
                break;
            case ConsoleColor.Blue:
                colourNumber = 1;
                break;
            case ConsoleColor.Yellow:
                colourNumber = 2;
                break;
            case ConsoleColor.Black:
                colourNumber = 3;
                break;
        }
        
        return Deck.NumNumbers * colourNumber + number - 1;
    }

    private Tile IndexToTile(int i)
    {
        return new Tile(i);
    }
}