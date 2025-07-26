// Author: Jacob Lisogurski
// File Name: TileCollection.cs
// Project Name: Rummikub
// Creation Date: June 11, 2025
// Modified Date: July 26, 2025
// Description: A collection of tiles that works by having an array of length 52 (the number of distinct tiles),
//              and having each element of the array represent how many of the associated tile there are. Note this is
//              always 0-2.

using System;
using System.Collections.Generic;

namespace Rummikub.Entities;

public class TileCollection
{
    private readonly int[] _tiles = new int[52];

    // Pre: None
    // Post: A list of all tiles this collection stores
    // Desc: Go from an array of ints to a collection of tiles
    public List<Tile> GetTiles()
    {
        List<Tile> tiles = [];
        
        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i] > 0) tiles.Add(new Tile(i));
            if (_tiles[i] > 1) tiles.Add(new Tile(i));
        }
        
        return tiles;
    }

    // Pre: A tile to add
    // Post: None
    // Desc: Add a tile to the collection by incrementing at the index of the associated tile
    public void Add(Tile tile)
    {
        _tiles[TileToIndex(tile)]++;
    }

    // Pre: A range of tiles to add
    // Post: None
    // Desc: Add a range of tiles
    public void AddRange(List<Tile> tiles)
    {
        foreach (Tile tile in tiles) Add(tile);
    }

    // Pre: A tile to remove
    // Post: None
    // Desc: Remove a tile from the collection if said tile is contained at least once
    public void Remove(Tile tile)
    {
        if (_tiles[TileToIndex(tile)] == 0) return;
        _tiles[TileToIndex(tile)]--;
    }

    // Pre: A tile
    // Post: The index of the element of _tiles that corresponds to how many of this tile are stored
    // Desc: Convert from tile to index, in a process that is essentially the reverse of the constructor of Tile
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
}