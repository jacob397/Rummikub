// Author: Jacob Lisogurski
// File Name: Tile.cs
// Project Name: Rummikub
// Creation Date: June 11, 2025
// Modified Date:
// Description: A tile, complete with an associated number and colour.

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Rummikub.Entities;

public class Tile
{
    public const int Width = 60;
    public const int Height = 60;
    private const int BlankTile = -1;
    
    private readonly int _number;
    private readonly ConsoleColor _colour;
    
    private readonly Rectangle _sourceRect;
    private Rectangle _destRect;

    // Pre: The position of the tile in the deck
    // Post: A tile object
    // Desc: Creates a tile
    public Tile(int i)
    {
        i %= Deck.NumUniqueTiles;
        _number = i % Deck.NumNumbers + 1;
        _colour = Deck.Colours[i / Deck.NumNumbers];
        _sourceRect = new Rectangle(i % Deck.NumNumbers * Width, i / Deck.NumNumbers * Height, Width, Height);
    }

    // Pre: The tile number and colour
    // Post: A tile object
    // Desc: Creates a tile
    public Tile(int number, ConsoleColor colour)
    {
        int i = 0;

        switch (colour)
        {
            case ConsoleColor.Red:
                i = 0;
                break;
            case ConsoleColor.Blue:
                i = 1;
                break;
            case ConsoleColor.Yellow:
                i = 2;
                break;
            case ConsoleColor.Black:
                i = 3;
                break;
        }
        
        _number = number;
        _colour = colour;
        _sourceRect = new Rectangle((_number - 1) * Width, i * Height, Width, Height);
    }

    // Pre: None
    // Post: A blank tile
    // Desc: Create a placeholder tile (the blank tiles on empty spaces of the board)
    public Tile()
    {
        _number = BlankTile;
    }

    // Pre: A tile object
    // Post: A copy of that tile
    // Desc: Creates a tile object
    public Tile(Tile tile)
    {
        _number = tile.Number();
        _colour = tile.Colour();
        _destRect = tile.GetDestinationRectangle();
        _sourceRect = tile.GetSourceRectangle();
    }

    // Pre: None
    // Post: The tile's number
    // Desc: Getter for the tile's number
    public int Number()
    {
        return _number;
    }

    // Pre: None
    // Post: The tile's colour
    // Desc: Getter for the tile's colour
    public ConsoleColor Colour()
    {
        return _colour;
    }
    
    // Pre: None
    // Post: The rectangle that represents where this tile is drawn on the screen
    // Desc: Getter for the destination rectangle
    public Rectangle GetDestinationRectangle()
    {
        return new Rectangle(_destRect.X, _destRect.Y, _destRect.Width, _destRect.Height);
    }
    
    // Pre: A rectangle
    // Post: None
    // Desc: Sets where the tile should be drawn on the screen
    public void SetDestinationRectangle(Rectangle rect)
    {
        _destRect = rect;
    }
    
    // Pre: None
    // Post: The source rectangle
    // Desc: Gets the source rectangle
    private Rectangle GetSourceRectangle()
    {
        return _sourceRect;
    }

    // Pre: None
    // Post: Boolean representing whether the cursor is hovering over this tile
    // Desc: Determines if the cursor is hovering over this tile
    public bool IsCursorOnTile(MouseState mouseState)
    {
        return _destRect.Contains(mouseState.Position);
    }

    // Pre: None
    // Post: Boolean representing whether this is a blank tile
    // Desc: Determines if this tile is blank (i.e. just a placeholder tile)
    public bool IsEmpty()
    {
        return _number == BlankTile;
    }

    // Pre: None
    // Post: Boolean representing whether two tiles share a number and colour
    // Desc: Determine if two tiles have the same number and colour
    public bool IsEqual(Tile tile)
    {
        return _number == tile.Number() && _colour == tile.Colour();
    }
    
    // Pre: Spritebatch and the image of all the tiles
    // Post: None
    // Desc: Draws the tile
    public void Draw(SpriteBatch spriteBatch, Texture2D img)
    {
        if (_number != BlankTile)
        {
            spriteBatch.Draw(img, _destRect, _sourceRect, Color.White);
        }
    }

    // Pre: None
    // Post: None
    // Desc: Moves the tile right
    public void MoveRight()
    {
        _destRect.X += 2;
    }

    // Pre: None
    // Post: None
    // Desc: Moves the tile left
    public void MoveLeft()
    {
        _destRect.X -= 2;
    }
}