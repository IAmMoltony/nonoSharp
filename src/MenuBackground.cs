using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NonoSharp;

public class MenuBackground
{
	private List<MenuBackgroundTile> _tiles;

	public MenuBackground(GraphicsDevice graphDev)
	{
		_tiles = new();
		for (int i = 0; i < 7; i++)
		{
			int randomX = Random.Shared.Next(0, graphDev.Viewport.Bounds.Width);
			int randomY = Random.Shared.Next(0, graphDev.Viewport.Bounds.Height);
			int snappedX = (int)Math.Round((float)randomX / 32.0f) * 32;
			int snappedY = (int)Math.Round((float)randomY / 32.0f) * 32;
			_tiles.Add(new(snappedX, snappedY));
		}
	}

	public void Draw(SpriteBatch sprBatch)
	{
		foreach (MenuBackgroundTile tile in _tiles)
		{
			tile.Draw(sprBatch);
		}
	}

	public void Update()
	{
		foreach (MenuBackgroundTile tile in _tiles)
		{
			tile.Update();
		}
	}
}