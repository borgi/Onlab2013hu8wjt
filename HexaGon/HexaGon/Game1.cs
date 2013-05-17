#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

#endregion

namespace HexaGon
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Texture2D blank;
		KeyboardState oldState;
		MouseState oldMState;
		Camera2d cam = null;
		int MAX_LEVEL_SIZE = 5;
		int HEXASIZE = 50;
		int FIGURESIZE = 75;
		int HEXA_LINE_WIDTH = 2;
		Field[,] palya = new Field[5,5];
		Vector2 palyaPos = new Vector2(0,180);
		Vector2[] UnitHexaCoords = new Vector2[6];

		SpriteFont sf;
		String stateMessage;

		int clickingstate = 0;
		int playerturn = 1;

		Texture2D background;
		Texture2D mario;
		Texture2D bubble;

		int[,] palyatomb = new int[,]
		{
			{1,1,1,1,0,1,0},
			{1,1,1,1,1,1,1},
			{1,1,1,1,1,1,1},
			{1,1,1,1,1,1,1},
			{0,1,1,1,1,1,1},
			{1,1,1,1,1,1,1},
			{0,1,1,1,1,1,0}
		};


		public Game1 ()
		{
			//gd = graphics.CreateDevice();
			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";	            
			graphics.IsFullScreen = false;	

			for(int i=0; i < MAX_LEVEL_SIZE; i++)
			{
				for(int j=0; j < MAX_LEVEL_SIZE; j++)
				{
					palya[i,j] = new Field(palyatomb[i,j],0, i ,j);
				}

			}

			stateMessage = "It's Mario turn.";

			palya[0,0].UnitState = 1;
			palya[4,4].UnitState = 2;

		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here
			base.Initialize ();

			Content.RootDirectory = "Content";

			oldState = Keyboard.GetState();

			for(int i = 0; i < 6 ; i++)
			{
				UnitHexaCoords[i].X = (float)Math.Cos (60.0d*i*Math.PI / 180);
				UnitHexaCoords[i].Y = (float)Math.Sin (60.0d*i*Math.PI / 180);
			}

			cam = new Camera2d();
				
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);

			blank = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			blank.SetData(new[]{Color.White});

			background = Content.Load<Texture2D>("background");
			mario = Content.Load<Texture2D>("mario_transp");
			bubble = Content.Load<Texture2D>("bubble");

			sf = Content.Load<SpriteFont>("SpriteFont2");

			//TODO: use this.Content to load your game content here 
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
				Exit ();
			}

			stateMessage = "It's " + ((playerturn == 1) ? "Mario" : "Bub") + "'s turn.";

			MouseState currentMouseState = Mouse.GetState();

			if(currentMouseState.LeftButton == ButtonState.Pressed && oldMState.LeftButton == ButtonState.Released)
			{
				Console.WriteLine(currentMouseState.X + " " + currentMouseState.Y);

				Vector2 worldPosition = Vector2.Transform(new Vector2(currentMouseState.X, currentMouseState.Y), Matrix.Invert(cam.get_transformation(GraphicsDevice)));

				Console.WriteLine("VÖRD: " + worldPosition.X + " " + worldPosition.Y);

				bool eltatlaltegyhexagont = false;

				for(int i = 0; i < MAX_LEVEL_SIZE; i++)
				{
					for(int j = 0; j < MAX_LEVEL_SIZE; j++)
					{
						
						float moveX = 1.5f*i*HEXASIZE - 1.5f*j*HEXASIZE ;
						float moveY = (-1)*(j+i)*HEXASIZE*(float)Math.Sin (60*(float)Math.PI/180);
					
						Vector2 thispoint = new Vector2(moveX + palyaPos.X, moveY + palyaPos.Y);

						Vector2 diff = thispoint - worldPosition;

						if(diff.Length() < HEXASIZE*0.8f && palya[i,j].FieldState != 0)
							eltatlaltegyhexagont = true;

						if(clickingstate == 0 && diff.Length() < HEXASIZE*0.8f && palya[i,j].FieldState != 0 && palya[i,j].UnitState == playerturn)
						{
							palya[i,j].FieldState = palya[i,j].FieldState == 0 ? 0 :2;

							for(int i2=0; i2 < MAX_LEVEL_SIZE; i2++)
							{
								for(int j2=0; j2 < MAX_LEVEL_SIZE; j2++)
								{
									if(Distance(palya[i,j],palya[i2,j2]) == 1)
									{
										palya[i2,j2].FieldState = palya[i2,j2].FieldState == 0 ? 0 :3;
									}
									if(Distance(palya[i,j],palya[i2,j2]) == 2)
									{
										palya[i2,j2].FieldState = palya[i2,j2].FieldState == 0 ? 0 :4;
									}
								}
							}

							clickingstate = 1;
						} else if (clickingstate == 1 && diff.Length() < HEXASIZE*0.8f &&  palya[i,j].UnitState == 0  && palya[i,j].FieldState != 0)
						{

							if(palya[i,j].FieldState == 3)
							{
								palya[i,j].UnitState = playerturn;
							} else if(palya[i,j].FieldState == 4)
							{
								palya[i,j].UnitState = playerturn;
								for(int i2=0; i2 < MAX_LEVEL_SIZE; i2++)
								{
									for(int j2=0; j2 < MAX_LEVEL_SIZE; j2++)
									{
										if(palya[i2,j2].FieldState == 2)
											palya[i2,j2].UnitState = 0;

									}
								}
							}

							if(palya[i,j].FieldState == 3 || palya[i,j].FieldState == 4)
							{
								for(int i2=0; i2 < MAX_LEVEL_SIZE; i2++)
								{
									for(int j2=0; j2 < MAX_LEVEL_SIZE; j2++)
									{
										palya[i2,j2].FieldState = palya[i2,j2].FieldState == 0 ? 0 : 1;

										if(Distance(palya[i2,j2],palya[i,j])==1)
										{
											if(palya[i2,j2].UnitState != 0 && palya[i2,j2].UnitState != playerturn)
												palya[i2,j2].UnitState = playerturn;
										}
									}
								}

								playerturn = playerturn == 1 ? 2 : 1;
								clickingstate = 0;
							}


						}

					}
				}

				if(eltatlaltegyhexagont== false && clickingstate == 1)
				{
					clickingstate = 0;
					for(int i2=0; i2 < MAX_LEVEL_SIZE; i2++)
					{
						for(int j2=0; j2 < MAX_LEVEL_SIZE; j2++)
						{
							palya[i2,j2].FieldState = palya[i2,j2].FieldState == 0 ? 0 : 1;
						}
					}
				}



			}

			bool tudlepni = false;
			bool vanszabadhely = false;

			int Bubs = 0;
			int Marios = 0;

			for(int i = 0; i < MAX_LEVEL_SIZE; i++)
			{
				for(int j = 0; j < MAX_LEVEL_SIZE; j++)
				{
					if(palya[i,j].UnitState == 1)
						Marios++;
					
					if(palya[i,j].UnitState == 2)
						Bubs++;
					
					if(palya[i,j].UnitState == 0  && palya[i,j].FieldState != 0)
						vanszabadhely = true;
					
					for(int i2=0; i2 < MAX_LEVEL_SIZE; i2++)
					{
						for(int j2=0; j2 < MAX_LEVEL_SIZE; j2++)
						{
							int tav = Distance(palya[i,j],palya[i2,j2]);
							if(palya[i,j].UnitState == playerturn && palya[i2,j2].UnitState == 0 && palya[i2,j2].FieldState != 0 
							   && (tav == 1 || tav == 2) )
								tudlepni = true;
						}
					}
				}
			}

			
			if(tudlepni == false)
				stateMessage = ((playerturn == 1) ? "Mario" : "Bub") + " can't move. " + ((playerturn == 1) ? "Bub" : "Mario") + " won.";

			if(vanszabadhely == false)
			{
				if(Marios > Bubs)
					stateMessage = "Mario won with " + Marios + " against " + Bubs + " Bubs.";
				else if (Marios == Bubs)
					stateMessage = "Its a Draw";
				else stateMessage = "Bubs won with " + Bubs + " against " + Marios + " Marios.";
			}

			oldMState = currentMouseState;

			UpdateInput();
			
			base.Update(gameTime);
		}
		
		private void UpdateInput()
		{
			KeyboardState newState = Keyboard.GetState();
			
			// Is the SPACE key down?
			if (newState.IsKeyDown(Keys.Space))
			{
				// If not down last update, key has just been pressed.
				if (!oldState.IsKeyDown(Keys.Space))
				{
					cam.Move(new Vector2(10.0f,10.0f));
				}
			}
			else if (oldState.IsKeyDown(Keys.Space))
			{
				// Key was down last update, but not down now, so
				// it has just been released.
			}
			
			// Update saved state.
			oldState = newState;
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.CornflowerBlue);
		
			//TODO: Add your drawing code here


			spriteBatch.Begin(SpriteSortMode.BackToFront,
			                  BlendState.NonPremultiplied,
			                  null,
			                  null,
			                  null,
			                  null,
			                  cam.get_transformation(GraphicsDevice));

			//int pushloverbythis = 0*((MAX_LEVEL_SIZE-1)/2)*HEXASIZE;

			spriteBatch.Draw(background, new Rectangle(-500, -500, 1000, 1000), Color.White);

			for(int i = 0; i < MAX_LEVEL_SIZE; i++)
			{
				for(int j = 0; j < MAX_LEVEL_SIZE; j++)
				{

					float moveX = 1.5f*i*HEXASIZE - 1.5f*j*HEXASIZE ;
					float moveY = (-1)*(j+i)*HEXASIZE*(float)Math.Sin (60*(float)Math.PI/180);

					if(palya[i,j].FieldState != 0)
					{

					for(int k = 0; k < 6; k++){
						
						DrawLine(spriteBatch, blank, HEXA_LINE_WIDTH, 
							         palya[i,j].FieldState == 1 ? Color.White : 
							         palya[i,j].FieldState == 2 ? Color.Red :
							         palya[i,j].FieldState == 3 ? Color.Green :
							         palya[i,j].FieldState == 4 ? Color.Blue :
							         Color.Yellow, 
							         new Vector2(UnitHexaCoords[k % 6].X*HEXASIZE + moveX + palyaPos.X, UnitHexaCoords[k % 6].Y*HEXASIZE + moveY + palyaPos.Y), 
							         new Vector2(UnitHexaCoords[(k+1) % 6].X*HEXASIZE + moveX + palyaPos.X, UnitHexaCoords[(k+1) % 6].Y*HEXASIZE  + moveY + palyaPos.Y));
						}
					}



					if(palya[i,j].UnitState == 1)
					{
						spriteBatch.Draw(mario, new Rectangle((int)(moveX + + palyaPos.X - FIGURESIZE/2), (int)(moveY + palyaPos.Y - FIGURESIZE/2), FIGURESIZE, FIGURESIZE), Color.White);
						
					}

					if(palya[i,j].UnitState == 2)
					{
						spriteBatch.Draw(bubble, new Rectangle((int)(moveX + + palyaPos.X - FIGURESIZE/2), (int)(moveY + palyaPos.Y - FIGURESIZE/2), FIGURESIZE, FIGURESIZE), Color.White);
						
					}


				}
			}

			spriteBatch.DrawString(sf, stateMessage, new Vector2(-GraphicsDevice.Viewport.Width/2,-GraphicsDevice.Viewport.Height/2), Color.White);

			spriteBatch.End();            

			base.Draw (gameTime);
		}


		void DrawLine(SpriteBatch batch, Texture2D blank,
		              float width, Color color, Vector2 point1, Vector2 point2)
		{
			float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
			float length = Vector2.Distance(point1, point2);
			
			batch.Draw(blank, point1, null, color,
			           angle, Vector2.Zero, new Vector2(length, width),
			           SpriteEffects.None, 0);
		}

		int Distance(Field f1, Field f2)
		{

			int dx = f1.x - f2.x;
			int dy = f1.y - f2.y;

			if (Math.Sign(dx) != Math.Sign(dy))
			{
				return Math.Abs(dx) + Math.Abs(dy);
			}
			else
				return Math.Max(Math.Abs(dx), Math.Abs(dy));

		}
	}
}

