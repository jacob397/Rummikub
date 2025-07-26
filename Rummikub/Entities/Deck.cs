// Author: Jacob Lisogurski
// File Name: Deck.cs
// Project Name: Rummikub
// Creation Date: June 11, 2025
// Modified Date: July 23, 2025
// Description: The deck of tiles, which functions as a stack.

using System;
using System.Collections.Generic;

namespace Rummikub.Entities;

public class Deck
{
    private const int NumTiles = 104;
    public const int NumUniqueTiles = 52;
    public const int NumNumbers = 13;
    public const int NumColours = 4;
    public static readonly ConsoleColor[] Colours = 
        [ ConsoleColor.Red, ConsoleColor.Blue, ConsoleColor.Yellow, ConsoleColor.Black ];
    
    private readonly List<Tile> _deck = [];
    private static readonly Random Rng = new();

    // Pre: None
    // Post: A shuffled deck
    // Desc: Creates and shuffles the deck
    public Deck()
    {
        // Populates the deck with all the required tiles
        for (var i = 0; i < NumTiles; i++)
        {
            _deck.Add(new Tile(i));
        }

        // Shuffles the deck
        for (var i = 0; i < 1000; i++)
        {
            var rand1 = Rng.Next(0, NumTiles);
            var rand2 = Rng.Next(0, NumTiles);
            (_deck[rand1], _deck[rand2]) = (_deck[rand2], _deck[rand1]);
        }
    }

    // Pre: None
    // Post: The next tile in the deck
    // Desc: Draws a tile from the deck
    public Tile Pop()
    {
        if (IsEmpty())
        {
            return null;
        }
        
        Tile tempTile = _deck[^1];
        _deck.RemoveAt(_deck.Count - 1);
        return tempTile;
    }

    // Pre: None
    // Post: Whether the deck is empty
    // Desc: Returns if the deck is empty
    public bool IsEmpty()
    {
        return _deck.Count == 0;
    }
}