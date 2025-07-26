// Author: Jacob Lisogurski
// File Name: Move.cs
// Project Name: Rummikub
// Creation Date: June 14, 2025
// Modified Date: July 26, 2025
// Description: Represents a potential move that the opponent player may carry out.

using System.Collections.Generic;
using System.Linq;

namespace Rummikub.Entities;

public class Move(List<List<Tile>> sets, Board board, Hand hand)
{
    // Placeholder value for the score if it is uncalculated
    private const int Uncalculated = -2;

    private List<List<Tile>> Sets { get; } = sets;
    private Board Board { get; } = board;
    private Hand Hand { get; } = hand;
    private int _score = Uncalculated;
    private readonly List<Tile> _tilesToRemove = [];

    // Pre: None
    // Post: The score associated with this move
    // Desc: Get the score associated with this move (higher scores mean the move is better)
    public int Score()
    {
        // If the score has already been calculated, return it
        if (_score != Uncalculated) return _score;
        
        var score = 0;
        List<Tile> usedTiles = [];
        var boardTiles = Board.GetTiles();
        var handTiles = Hand.GetTiles();

        foreach (var set in Sets) usedTiles.AddRange(set);
        
        // If any board tile is not used in the move, the score is -1
        if (boardTiles.Any(boardTile => !RemoveTile(usedTiles, boardTile))) return -1;

        // For every hand tile used, the score goes up by 1
        // If a tile used in the move does not belong to the hand or the board, the move is invalid
        foreach (var usedTile in usedTiles)
        {
            if (RemoveTile(handTiles, usedTile))
            {
                score++;
                _tilesToRemove.Add(usedTile);
            }
            else
            {
                return -1;
            }
        }

        _score = score;
        return score;
    }

    // Pre: A list of tiles and a tile to remove from the list
    // Post: A boolean representing whether the tile was successfully removed
    // Desc: Remove a tile from a list of tiles
    private static bool RemoveTile(List<Tile> tiles, Tile tile)
    {
        for (var i = 0; i < tiles.Count; i++)
        {
            if (!tiles[i].IsEqual(tile)) continue;
            tiles.RemoveAt(i);
            return true;
        }
        
        return false;
    }
    
    // Pre: None
    // Post: None
    // Desc: Carry out this move
    public void Do()
    {
        // Empty the board, place sets, and remove used tiles from the hand
        Board.Clear();
        foreach (var set in Sets) Board.PlaceSet(set);
        foreach (var tile in _tilesToRemove) hand.RemoveTile(tile);
    }
}