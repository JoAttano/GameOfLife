using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife
{
	public class BoardGame 
	{
		private bool[,] boardTab;

		private int heightBoard;
		private int widthBoard;

		private int heightTile;
		private int widthTile;

		private Texture2D tileBlack;

		private int iteration;

		private float time;

		private StateEvolve stateEvolve;
		public string State { get; private set;}

		private KeyboardState previousStateInput;

		public BoardGame(int  height, int width, GraphicsDeviceManager graphics)
		{
			heightBoard = height;
			widthBoard = width;

			boardTab = new bool[height, width];
			initializeValueBoardTab();

			heightTile = graphics.GraphicsDevice.Viewport.Height / heightBoard;
			widthTile = graphics.GraphicsDevice.Viewport.Width / widthBoard; 

			tileBlack = new Texture2D(graphics.GraphicsDevice,
					   widthTile, heightTile );
			
			initializeColorTile();
			initializeTileInBoard();

			iteration = 0;
			time = 0;

			stateEvolve = StateEvolve.Run;
			State = "Run";

			previousStateInput = Keyboard.GetState();

		}


		public void Update(GameTime gameTime)
		{
			var delta = (float)gameTime.ElapsedGameTime.Milliseconds;

			KeyboardState stateInput = Keyboard.GetState();

			if (stateInput.IsKeyDown(Keys.Space) && !previousStateInput.IsKeyDown(Keys.Space))
			{
				if (stateEvolve == StateEvolve.Run)
				{
					stateEvolve = StateEvolve.Pause;
					State = "Pause";
				}
				else
				{
					stateEvolve = StateEvolve.Run;
					State = "Run";
				}
			}

			previousStateInput = Keyboard.GetState();

			if (stateEvolve == StateEvolve.Run)
			{
				time += 0.001f * delta;
				Console.WriteLine("Time : {0} Delta : {1}", time, delta);
				if (time >= 1)
				{
					evolveLife();
					iteration++;
					time = 0;
				}
			}


		}

		public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
		{
			spriteBatch.Begin();

			for (int y = 0; y < heightBoard; y++)
				for (int x = 0; x < widthBoard; x++)
					if(boardTab[y,x])
						spriteBatch.Draw(tileBlack,
									 	new Vector2(x * widthTile, y * heightTile),
									 	Color.Black);
			


			spriteBatch.End();
		}

		private void evolveLife()
		{
			var listBorn = new List<Tuple<int, int>>();
			var listDead = new List<Tuple<int, int>>();

			for (int y = 0; y < heightBoard; y++)
			{
				for (int x = 0; x < widthBoard; x++)
				{

					int total = 0;

					if (!isOutArray(y - 1, x - 1))
						if (boardTab[y - 1, x - 1])
							total++;
					if (!isOutArray(y - 1, x))
						if (boardTab[y - 1, x])
							total++;
					if (!isOutArray(y - 1, x + 1))
						if (boardTab[y - 1, x + 1])
							total++;
					if (!isOutArray(y + 1, x - 1))
						if (boardTab[y + 1, x - 1])
							total++;
					if (!isOutArray(y + 1, x))
						if (boardTab[y + 1, x])
							total++;
					if (!isOutArray(y + 1, x + 1))
						if (boardTab[y + 1, x + 1])
							total++;
					if (!isOutArray(y, x - 1))
						if (boardTab[y, x - 1])
							total++;
					if (!isOutArray(y, x + 1))
						if (boardTab[y, x + 1])
							total++;



					if (boardTab[y, x] == true)
					{
						if (total < 2 || total > 3)
						{
							listDead.Add(new Tuple<int, int>(x, y));
						}
					}
					else
					{
						if (total == 3)
							listBorn.Add(new Tuple<int, int>(x, y));
					}
				}
			}
			listBorn.ForEach(delegate (Tuple<int, int> born)
			{
				boardTab[born.Item2, born.Item1] = true;
			});
			listDead.ForEach(delegate (Tuple<int, int> dead)
			{
				boardTab[dead.Item2, dead.Item1] = false;
			});
		}

		private void initializeColorTile()
		{
			Color[] data = new Color[heightTile * widthTile];
			for (int color = 0; color < data.Length; color++)
				data[color] = Color.Black;
			tileBlack.SetData(data);
			
		}


		private void initializeValueBoardTab()
		{
			for (int hTile = 0; hTile < heightBoard; hTile++)
			{	for (int wTile = 0; wTile < widthBoard; wTile++)
				{
					boardTab[hTile, wTile] = false;
				}
			}
		}

		private void initializeTileInBoard()
		{
			bool[,] tab = new bool[2,5]{
				{false, true, false, false, false},
				{ true, true,  true,  true,  true}
			};

			int originX = widthBoard / 2 - 2;
			int originY = heightBoard / 2 - 1;

			for (int y = 0; y < tab.GetLength(0); y++)
			{
				for (int x = 0; x < tab.GetLength(1); x++)
				{
					boardTab[originY + y, originX + x] = tab[y, x];
				}
			}

		}
		private bool isOutArray(int y, int x)
		{
			if ((y < 0 || y >= heightBoard) || (x < 0 || x >= widthBoard))
				return true;
			
			return false;

		}


	}
}
