// Author: Jacob Lisogurski
// File Name: Board.cs
// Project Name: Rummikub
// Creation Date: June 11, 2025
// Modified Date: July 23, 2025
// Description: The board of all tiles in play.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Rummikub.Entities;

public class Board
{
    private const int X = 45;
    private const int Y = 20;
    private const int TileSpacing = 10;
    public const int MinSetSize = 3;
    
    private readonly Tile[,] _board = new Tile[16, 9];
    
    // Pre: None
    // Post: A board object
    // Desc: Creates the board and positions all spaces on the board
    public Board()
    {
        for (var i = 0; i < _board.GetLength(0); i++)
        {
            for (var j = 0; j < _board.GetLength(1); j++)
            {
                _board[i, j] = new Tile();
                _board[i, j].SetDestinationRectangle
                    (new Rectangle(X + i * (Tile.Width + TileSpacing), Y + j * 
                        (Tile.Height + TileSpacing), Tile.Width, Tile.Height));
            }
        }
    }

    // Pre: Another board object
    // Post: A new board object that is a copy of the board passed in
    // Desc: Create a copy of a board
    public Board(Board board)
    {
        for (var i = 0; i < _board.GetLength(0); i++)
        {
            for (var j = 0; j < _board.GetLength(1); j++)
            {
                PlaceTile(i, j, new Tile(board.GetTile(i, j)));
            }
        }
    }

    // Pre: The coordinates of a tile
    // Post: The tile at said coordinates
    // Desc: Get a tile from the board
    private Tile GetTile(int x, int y)
    {
        return _board[x, y];
    }
    
    // Pre: None
    // Post: All tiles on the board
    // Desc: Get all tiles on the board
    public List<Tile> GetTiles()
    {
        var tiles = new List<Tile>();
        
        for (var i = 0; i < _board.GetLength(0); i++)
        {
            for (var j = 0; j < _board.GetLength(1); j++)
            {
                if (!_board[i, j].IsEmpty())
                {
                    tiles.Add(_board[i, j]);
                }
            }
        }
        
        return tiles;
    }

    // Pre: None
    // Post: A list of all sets on the board
    // Desc: Get all sets on the board
    public List<List<Tile>> GetSets()
    {
        // Store the sets and the current considered set
        var sets = new List<List<Tile>>();
        var set = new List<Tile>();
        
        // Iterate through all tiles on the board
        for (int j = 0; j < _board.GetLength(1); j++)
        {
            for (int i = 0; i < _board.GetLength(0); i++)
            {
                // If an empty space is found, then the current set is a completed set (provided it's long enough),
                // so it can be added to the list of sets
                // Otherwise, add the tile found to the current set
                if (_board[i, j].IsEmpty())
                {
                    if (set.Count > 0)
                    {
                        if (set.Count > 0) sets.Add(set.ToList());
                        set = [];
                    }
                }
                else
                {
                    set.Add(_board[i, j]);
                }

                // If the end of the board is found, add the current set to the list of sets
                if (i == _board.GetLength(0) - 1)
                {
                    if (set.Count > 0) sets.Add(set.ToList());
                    set = [];
                }
            }
        }
        
        // If the end of the board is found, add the current set to the list of sets
        if (set.Count > 0) sets.Add(set.ToList());
        return sets;
    }
    
    // Pre: The previous mousestate
    // Post: The coordinates of the space on the board that has been clicked, or otherwise null
    // Desc: Get the coordinates of the clicked space on the board
    public int[] GetClickedCoordinates(MouseState currentMouseState, MouseState previousMouseState)
    {
        if (currentMouseState.LeftButton != ButtonState.Pressed || previousMouseState.LeftButton == ButtonState.Pressed)
        {
            return null;
        }

        for (var i = 0; i < _board.GetLength(0); i++)
        {
            for (var j = 0; j < _board.GetLength(1); j++)
            {
                if (_board[i, j].IsCursorOnTile(currentMouseState))
                {
                    return [i, j];
                }
            }
        }

        return null;
    }
    
    // Pre: The previous mouse state
    // Post: The tile on the board that has been clicked
    // Desc: Get a tile that has been clicked
    public Tile GetClickedTile(MouseState previousState)
    {
        if (Mouse.GetState().LeftButton != ButtonState.Pressed || previousState.LeftButton == ButtonState.Pressed)
        {
            return null;
        }

        for (var i = 0; i < _board.GetLength(0); i++)
        {
            for (var j = 0; j < _board.GetLength(1); j++)
            {
                if (_board[i, j].IsCursorOnTile(Mouse.GetState()))
                {
                    return _board[i, j];
                }
            }
        }

        return null;
    }

    // Pre: Coordinates of the tile of interest
    // Post: None
    // Desc: Remove a tile
    public void RemoveTile(int x, int y)
    {
        var rect = _board[x, y].GetDestinationRectangle();
        _board[x, y] = new Tile();
        _board[x, y].SetDestinationRectangle(rect);
    }
    
    // Pre: None
    // Post: None
    // Desc: Empties the board by making every space hold a blank tile
    public void Clear()
    {
        for (var i = 0; i < _board.GetLength(0); i++)
        {
            for (var j = 0; j < _board.GetLength(1); j++)
            {
                PlaceTile(i, j, new Tile());
            }
        }
    }
    
    // Pre: A tile and the coordinates where it should be placed
    // Post: None
    // Desc: Places a tile on the board
    public void PlaceTile(int x, int y, Tile tile)
    {
        var rect = new Rectangle(X + x * (Tile.Width + TileSpacing), Y + y * 
            (Tile.Height + TileSpacing), Tile.Width, Tile.Height);
        _board[x, y] = tile;
        tile.SetDestinationRectangle(rect);
        tile.SetX(x);
        tile.SetY(y);
    }

    // Pre: The set to place
    // Post: None
    // Desc: Places a set on the board
    public void PlaceSet(List<Tile> set)
    {
        var numEmptySpaces = 0;
        var startRow = 0;
        var startCol = 0;

        // Search for valid coordinates of the first tile in the set
        for (var j = 0; j < _board.GetLength(1); j++)
        {
            for (var i = 0; i < _board.GetLength(0); i++)
            {
                if (_board[i, j].IsEmpty())
                {
                    if (numEmptySpaces == 0)
                    {
                        startRow = j;
                        startCol = i;
                    }
                    else if (numEmptySpaces == set.Count + 1)
                    {
                        // This is done to break out of the loop, when valid coordinates have been found
                        i = _board.GetLength(0);
                        j = _board.GetLength(1);
                    }
                                    
                    numEmptySpaces++;
                }
                else
                {
                    numEmptySpaces = 0;
                }
            }
            
            numEmptySpaces = 0;
        }

        // Place the set
        for (int i = 1; i <= set.Count; i++)
        {
            if (startCol == 0)
            {
                PlaceTile(startCol + i - 1, startRow, new Tile(set[i - 1].Number(), set[i - 1].Colour()));
            }
            else
            {
                PlaceTile(startCol + i, startRow, new Tile(set[i - 1].Number(), set[i - 1].Colour()));
            }
        }
    }

    // Pre: None
    // Post: A bool representing whether the board is valid (i.e. made up of sets and blank spaces)
    // Desc: Determine if the board is valid
    public bool IsValid()
    {
        var tiles = new List<Tile>();

        // Search for collections of adjacent tiles on the board and ensures all such collections are sets
        for (int j = 0; j < _board.GetLength(1); j++)
        {
            for (int i = 0; i < _board.GetLength(0); i++)
            {
                if (_board[i, j].IsEmpty())
                {
                    if (tiles.Count > 0)
                    {
                        if (!IsSet(tiles))
                        {
                            return false;
                        }
                        
                        tiles = [];
                    }
                }
                else
                {
                    tiles.Add(_board[i, j]);
                }
            }
            
            if (tiles.Count > 0 && !IsSet(tiles)) return false;
            tiles = [];
        }
        
        return tiles.Count == 0 || IsSet(tiles);
    }
    
    // Pre: Coordinates of the tile of interest
    // Post: A boolean representing whether the tile is blank
    // Desc: Determine if a tile is blank
    public bool IsTileEmpty(int[] coordinates)
    {
        return _board[coordinates[0], coordinates[1]].IsEmpty();
    }
    
    // Pre: The spritebatch and the image of the tiles
    // Post: None
    // Desc: Draws the board
    public void Draw(SpriteBatch spriteBatch, Texture2D img)
    {
        for (var i = 0; i < _board.GetLength(0); i++)
        {
            for (var j = 0; j < _board.GetLength(1); j++)
            {
                _board[i, j].Draw(spriteBatch, img);
            }
        }
    }
    
    // Pre: A list of tiles
    // Post: A bool representing whether the list of tiles is a run
    // Desc: Return whether a list of tiles is a run
    public static bool IsRun(List<Tile> tiles)
    {
        if (tiles.Count < MinSetSize)
        {
            return false;
        }

        // Ensure the list is a run (all tiles are the same colour and have consecutive numbers)
        for (var i = 0; i + 1 < tiles.Count; i++)
        {
            if (!(tiles[i].Colour() == tiles[i + 1].Colour() && tiles[i].Number() + 1 == tiles[i + 1].Number()))
            {
                return false;
            }
        }
        
        return true;
    }

    // Pre: A list of tiles
    // Post: A bool representing whether the tiles form a group
    // Desc: Determine if a list of tiles is a group
    public static bool IsGroup(List<Tile> tiles)
    {
        // If the list is too large or too small to be a group, return false
        if (tiles.Count < MinSetSize || tiles.Count > Deck.NumColours)
        {
            return false;
        }

        var numRed = 0;
        var numBlue = 0;
        var numYellow = 0;
        var numBlack = 0;

        foreach (var tile in tiles)
        {
            switch (tile.Colour())
            {
                case ConsoleColor.Red:
                    numRed++;
                    break;
                case ConsoleColor.Blue:
                    numBlue++;
                    break;
                case ConsoleColor.Yellow:
                    numYellow++;
                    break;
                case ConsoleColor.Black:
                    numBlack++;
                    break;
            }
        }

        // If any tiles are the same colour, return false
        if (numRed > 1 || numBlue > 1 || numYellow > 1 || numBlack > 1)
        {
            return false;
        }

        // If any tiles are different numbers, return false
        for (var i = 0; i + 1 < tiles.Count; i++)
        {
            if (tiles[i].Number() != tiles[i + 1].Number())
            {
                return false;
            }
        }
        
        return true;
    }

    // Pre: A list of tiles
    // Post: A boolean representing whether the list is a valid set
    // Desc: Determine if a list of tiles is a set
    private static bool IsSet(List<Tile> tiles)
    {
        return IsGroup(tiles) || IsRun(tiles);
    }
}