using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace OutToTheEdgePuzzleFinal
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //Region for all variables that are delcared for use within the game
        //Contians multiple sub Regions
        #region Declared Variables
        //Region for the variables used in relation to Random
        #region Random
        //Random Seed used for Shuffle(). 
        //Declared here to not run into the issue of creating new Random Numbers to close together resulting in the same number being created due to how Random uses the system clock
        private static Random rnd = new Random();
        //CurrentRnd declared for use in the switch statement within the Shuffle function
        private int currentRnd;
        #endregion
        //Region for variables used to set the size of the game window
        //Declared here so that they can be used in the calculation of image starting positions
        #region GameWindow Width/Height
        private int gameWindowWidth = 1920;
        private int gameWindowHeight = 1080;
        #endregion
        //Region for the Enums and Gamestates used to transition the game screens
        //Enums and Gamestates are used to allow for ease of transition and allow for future scalability if needed
        #region Enums and Gamestates
        private enum GameState
        {
            startScreen,
            mainMenu,
            introScreen,
            imageSelect,
            pauseScreen,
            winScreen,
        }
        GameState currentState = GameState.startScreen;
        #endregion
        //Region for variables that are used by options to count moves/minutes and seconds elapsed
        #region Moves/Minutes and Seconds Counters
        //Ints used for the Counter and Timer Options
        //Variables declared as they neeed to be incremented within the game once thier option is enabled
        //Multiple variables used for Timer function due to the way gameTime time is calculated
        private double milliseconds;
        private int framerate;
        private int seconds;
        private int minutes;
        //Int used to store the number of moves a player has made
        private int moves;
        #endregion
        //Region for various Booleans used in the Game
        #region Booleans
        //Because of how Spritebatch.Draw works, I found it easier to use a booleans to trigger the game to draw text
        //Boolean for displaying a try again message
        private bool tryAgain;
        //Boolean for removing the image increment/decrement buttons so the image can't be changed after shuffling
        private bool isShuffled;
        //Boolean for displaying a message to the player that the tiles have already been shuffled and it can't be done again
        //A seperate boolean is used so that it can be reset so the message doesn't stay permenantly displayed
        private bool isShuffledMessage;
        //Boolean for indicating whether the tiles are currently being shuffled
        //Used to insure that the game can't be paused while tiles are being shuffled, during testing if the game was paused it would break the entire game by cause tiles to disapear
        private bool beingShuffled;
        //Boolean for displaying a message indicating that the image can't be changed after it has been shuffled
        private bool cantChange;
        //Boolean for displaying a message indicating that the player can't "win" before the image has been shuffled
        //Even though this could be done using isShuffled, this boolean will be reset by a function to "erase" the message that displays after a few seconds
        private bool originTilePos;
        //Boolean for determining whether or not to display text on how to control the game
        //This is toggled off when another message needs to be displayed so that the text doesn't overlap
        private bool displayControls;
        //Boolean for displaying the menu buttons and thier text
        private bool mainMenu;
        //Boolean for displaying the Credits
        private bool credits;
        //Boolean for displaying the Options Menu
        private bool options;
        //Booleans for toggling the various game options available
        private bool disableMusic;
        private bool enableCounter;
        private bool enableTimer;
        private bool enableTileNumbers;
        private bool startTimer;
        //Boolean for use with the Puzzle Solved function used to display the full image and then the WinScreen once the puzzle has been finished
        private bool puzzleSolved;
        //Boolean used to enable the replay and exit buttons during the win screen
        private bool displayWinButtons;
        #endregion
        //Region for the various int variables used to change which image is being displayed with an array
        //Variables are declared for ease of use with in the program
        #region Array Increment/Decrementers
        //Variable used for cycling through the different images in the Puzzle Image Array
        private int imageNumber;
        //Variables used for cycling through the different images in the Button Image Array, this way the button colors can be changed when an option is toggled
        private int buttonNumber1;
        private int buttonNumber2;
        private int buttonNumber3;
        private int buttonNumber4;
        #endregion
        //Region for Texture2Ds that are used in the game as either puzzle images, backgrounds or other assests
        #region Images
        //Background Images
        private Texture2D background;
        private Texture2D introBackground;
        //Button Textures
        private Texture2D buttonMenu;
        private Texture2D button1Red;
        private Texture2D button2Green;
        private Texture2D button3Yellow;
        private Texture2D button4Red;
        private Texture2D introTextBox;
        private Texture2D textBubble;
        //Puzzle Images
        private Texture2D puzzleImageColosseum;
        private Texture2D puzzleImageGreatWall;
        private Texture2D puzzleImageParthenon;
        private Texture2D puzzleImageRushmore;
        private Texture2D puzzleImageSphynx;
        private Texture2D puzzleImageTajMahal;
        #endregion
        //Region for the different SpriteFont assests and predefined text strings that are used during the game
        //Different fonts are used depending on the text that needs to be displayed
        //Region also includes Arrays of strings, mainly used during the intro and credits screens
        //As well as the longer text box messages as these can be drawn using a for loop reducing the amount of repeated code
        #region SpriteFonts/Strings
        //Font and Strings for the Title of the game
        private SpriteFont titleFont;
        private string title = "Out to the Edge";
        private string subTitle = "History Puzzles";
        private SpriteFont pressEnterFont;
        private string pressEnterToBegin = "Press [Enter] to Begin";
        //Font and String Arrays for the intro of the game
        private SpriteFont introFont;
        private string[] introText = new string[]
        {
            "Following the events of Out to the Edge:",
            "A few days after your conversation with Taliesin",
            "he asked if you wanted to take a break and play",
            "a game he'd found in the archives.",
            "He explained the game was an early 21st century",
            "version of a puzzle game that had been around",
            "since sometime in the 19th century.",
            "The version that he had found used pictures of old",
            "historical sites from Earth and he thought this",
            "might be a nice way to pass the time.",
            "You agreed, as you had started missing Earth.",
            "You sit down at your desk as Taliesin loads the",
            "game on your workstation screen.",
        };

        //String Arrays for the Options and Credits menus
        private string[] creditsText = new string[]
        {
            "Created by Oliver Collier",
            "Art by Francesca Fell",
            "Music: Quiescent In Time by Shane Ivers",
            "Available at https://www.silvermansound.com",
            "Sound Effects from - https://www.zapsplat.com",
        };
        private string[] optionsText = new string[]
        {
            "Disable Music - ",
            "Enable Timer - ",
            "Enable Move Counter - ",
            "Enable Tile Numbers - ",
        };
        //Font and String Arrays for the Text Box and Controls Text displayed within
        private SpriteFont textBoxFont;
        private string[] controlsText = new string[]
        {
            "The buttons on the left control",
            "the image that is displayed",
            "Red: Change the image",
            "Yellow: Shuffles the tiles ",
            "After shuffling use Green to:",
            "Confirm the tiles are in the right",
            "place and complete the puzzle",
        };
        private string[] controlsText2 = new string[]
        {
            "Tiles have to be next",
            "to the blank tile to move",
            "Use [W] to move Tiles Up",
            "Use [S] to move Tiles Down",
            "Use [A] to move Tiles Left",
            "Use [D] to move Tiles Right",
            "Use [Spacebar] to pause"
        };
        #endregion
        //Region for the different Music/Sound effects used in the game
        //Due to Monogame restrictions Music for soundtrack are MP3s and SFX are WAV files
        #region Music/SFX
        private Song backgroundMusic;
        private SoundEffect tileMovementEffect;
        private SoundEffect buttonClickEffect;
        #endregion
        //Region for various Previous/Current Variables. 
        //These variables are used for things such as making sure a button has been pressed and released before executing a function or statement
        #region Previous/Current State Variables
        //KeyboardState Variables for ensuring a key has been pressed and released before moving a tile
        private KeyboardState previousKeyState;
        private KeyboardState currentKeyState;
        //Vector2 Variables to ensure that when tiles swap positions they don't swap to were the movable tile was when they intersected
        private Vector2 previousPosition;
        private Vector2 currentPosition;
        //MouseState variables for ensuring a mouse button has been clicked and released before performing an action
        private MouseState previousMouseState;
        private MouseState currentMouseState;
        #endregion
        //Region for the variables used to ensure that the puzzle image being broken into tiles is displayed at the center of the screen
        #region ImageCoords
        //X coordinate for where the image needs to start to be centered
        private int imageStartX;
        //Y coordinate for where the image needs to start to be centered
        private int imageStartY;
        #endregion
        //Region for the Variables that are used to break the puzzle image into tiles
        //As these values are reused they were declared for ease of use, also allows for non static ints to be used easily
        #region ImageGridCoords
        //X coordinate for first column
        private int gridX0;
        //X coordinate for second column
        private int gridX1;
        //X coordinate for third column
        private int gridX2;
        //X coordinate for fourth column
        private int gridX3;
        //Y coordinate for first row
        private int gridY0;
        //Y coordinate for second row
        private int gridY1;
        //Y coordinate for third row
        private int gridY2;
        //Y coordinate for fourth row
        private int gridY3;
        #endregion
        //Region for the various button coordinates
        //As these values are reused they were declared for ease of use and to reduce "magic number use"
        #region Button Coords
        //Coordinates for Button used to change the current image (increment ++)
        private Vector2 button1RedCoords;
        //Coordinates for Button used to confirm Tiles are in the proper place
        private Vector2 button2GreenCoords;
        //Coordinates for Shuffle Button Collision
        private Vector2 button3YellowCoords;
        //Coordinates for Button used to change the current image (decrement --)
        private Vector2 button4RedCoords;
        //Coordinates for Button used to start the game
        private Vector2 buttonPlayCoords;
        //Coordinates for Button used to pull up the options
        private Vector2 buttonOptionsCoords;
        //Coordinates for Button used to pull up the credits
        private Vector2 buttonCreditsCoords;
        //Coordinates for Button used to go back to the main menu from the option/credits screen
        //These coordinates are also used by the back button that is present at the pause screen and the replay button at the win screen
        private Vector2 buttonBackCoords;
        //Coordinates for Button used to exit the game after the puzzle has been solved
        private Vector2 buttonExitCoords;
        //Coordinates for Buttons used to toggle various in game options
        private Vector2 buttonOptionToggle1Coords;
        private Vector2 buttonOptionToggle2Coords;
        private Vector2 buttonOptionToggle3Coords;
        private Vector2 buttonOptionToggle4Coords;
        #endregion
        //Region for the Arrays used to store the various Puzzle Images and the final X and Y values that are assigned to each tile
        //Due to have to add the ImageStart and Grid values to get the final X and Y values arrays were created to easily store and recall these values
        #region Arrays
        //Array for final X Grid Coordinates
        private int[] gridXCoords;
        //Array for final Y Grid Coordinates
        private int[] gridYCoords;
        //Array for storing buttonImages for use on the options screen
        //An array is used so that the images can be easily toggled by a button press
        private Texture2D[] buttonImages;
        //Array for storing PuzzleImages
        private Texture2D[] puzzleImages;
        //Array for storing Tiles after they have been created
        //Used to allow the use of foreach loop to draw the game board of tiles
        private Tile[] gameBoard;
        #endregion
        //Region for the Rectangles that are used in determining what portion of the puzzle image each tile is displaying
        //Rectangles were used due to the functionality of the spritebatch.Draw function, which has a variant that uses Rectangles for determining the image source
        //These Rectangles are then assigned to the Tile Class
        #region TileImages
        //Rectangle for the image fragment displayed on Tile 1
        private Rectangle tileImage1;
        //Rectangle for the image fragment displayed on Tile 2
        private Rectangle tileImage2;
        //Rectangle for the image fragment displayed on Tile 3
        private Rectangle tileImage3;
        //Rectangle for the image fragment displayed on Tile 4
        private Rectangle tileImage4;
        //Rectangle for the image fragment displayed on Tile 5
        private Rectangle tileImage5;
        //Rectangle for the image fragment displayed on Tile 6
        private Rectangle tileImage6;
        //Rectangle for the image fragment displayed on Tile 7
        private Rectangle tileImage7;
        //Rectangle for the image fragment displayed on Tile 8
        private Rectangle tileImage8;
        //Rectangle for the image fragment displayed on Tile 9
        private Rectangle tileImage9;
        //Rectangle for the image fragment displayed on Tile 10
        private Rectangle tileImage10;
        //Rectangle for the image fragment displayed on Tile 11
        private Rectangle tileImage11;
        //Rectangle for the image fragment displayed on Tile 12
        private Rectangle tileImage12;
        //Rectangle for the image fragment displayed on Tile 13
        private Rectangle tileImage13;
        //Rectangle for the image fragment displayed on Tile 14
        private Rectangle tileImage14;
        //Rectangle for the image fragment displayed on Tile 15
        private Rectangle tileImage15;
        //Rectangle for the image fragment displayed on Tile 16
        private Rectangle tileImage16;
        #endregion
        //Region for Rectangles that are used for determining in game collisions
        //Seperate rectangles were used for Tile collision due to the need for these rectangles to update with current position data
        //Using the TileImage rectangles would have had the side effect of constantly changing the displayed puzzle image fragment
        //Also included in this region are the collision rectangles for the mouse pointer and buttons, so that they can have proper collision for click usage
        #region Collision Rectangles
        //Rectangle for Mouse Collision
        private Rectangle mouseRectangle;
        //Rectangle for Button used to change the current image (increment ++)
        private Rectangle button1RedRectangle;
        //Rectangle for Button used to confirm Tiles are in the proper place
        private Rectangle button2GreenRectangle;
        //Rectangle for Shuffle Button Collision
        private Rectangle button3YellowRectangle;
        //Rectangle for Button used to change the current image (decrement --)
        private Rectangle button4RedRectangle;
        //Rectangle for Button used to start the game
        private Rectangle buttonPlayRectangle;
        //Rectangle for Button used to pull up the options
        private Rectangle buttonOptionsRectangle;
        //Rectangle for Button used to pull up the credits
        private Rectangle buttonCreditsRectangle;
        //Rectangle for Button used to go back to the main menu from the option/credits screen
        //This rectangle is also used by the back button that is present at the pause screen and the replay button at the win screen
        private Rectangle buttonBackRectangle;
        //Rectangle for Button used to exit the game after the puzzle has been solved
        private Rectangle buttonExitRectangle;
        //Rectangles for Buttons used to toggle various in game options
        private Rectangle buttonOptionToggle1Rectangle;
        private Rectangle buttonOptionToggle2Rectangle;
        private Rectangle buttonOptionToggle3Rectangle;
        private Rectangle buttonOptionToggle4Rectangle;
        //Rectangle for Tile 1 Collisions
        private Rectangle collisionTile1;
        //Rectangle for Tile 2 Collisions
        private Rectangle collisionTile2;
        //Rectangle for Tile 3 Collisions
        private Rectangle collisionTile3;
        //Rectangle for Tile 4 Collisions
        private Rectangle collisionTile4;
        //Rectangle for Tile 5 Collisions
        private Rectangle collisionTile5;
        //Rectangle for Tile 6 Collisions
        private Rectangle collisionTile6;
        //Rectangle for Tile 7 Collisions
        private Rectangle collisionTile7;
        //Rectangle for Tile 8 Collisions
        private Rectangle collisionTile8;
        //Rectangle for Tile 9 Collisions
        private Rectangle collisionTile9;
        //Rectangle for Tile 10 Collisions
        private Rectangle collisionTile10;
        //Rectangle for Tile 11 Collisions
        private Rectangle collisionTile11;
        //Rectangle for Tile 12 Collisions
        private Rectangle collisionTile12;
        //Rectangle for Tile 13 Collisions
        private Rectangle collisionTile13;
        //Rectangle for Tile 14 Collisions
        private Rectangle collisionTile14;
        //Rectangle for Tile 15 Collisions
        private Rectangle collisionTile15;
        //Rectangle for Tile 16 Collisions
        private Rectangle collisionTile16;
        #endregion
        //Region for the Rectangles that are used for determining if the tiles are in there starting or win positions
        //Seperate Rectangles were used as they needed to have the the tiles' final X and Y positions, whereas the TileImage Rectangles did not include the ImageStart values
        //Unlike the Collision Rectangles these are static and are simply used by the CheckPosition function to see if the Tiles are in the "win" positions
        #region TileWinPosition Rectangles
        //Rectangle for Tile 1 Win Position
        private Rectangle winPositionTile1;
        //Rectangle for Tile 2 Win Position
        private Rectangle winPositionTile2;
        //Rectangle for Tile 3 Win Position
        private Rectangle winPositionTile3;
        //Rectangle for Tile 4 Win Position
        private Rectangle winPositionTile4;
        //Rectangle for Tile 5 Win Position
        private Rectangle winPositionTile5;
        //Rectangle for Tile 6 Win Position
        private Rectangle winPositionTile6;
        //Rectangle for Tile 7 Win Position
        private Rectangle winPositionTile7;
        //Rectangle for Tile 8 Win Position
        private Rectangle winPositionTile8;
        //Rectangle for Tile 9 Win Position
        private Rectangle winPositionTile9;
        //Rectangle for Tile 10 Win Position
        private Rectangle winPositionTile10;
        //Rectangle for Tile 11 Win Position
        private Rectangle winPositionTile11;
        //Rectangle for Tile 12 Win Position
        private Rectangle winPositionTile12;
        //Rectangle for Tile 13 Win Position
        private Rectangle winPositionTile13;
        //Rectangle for Tile 14 Win Position
        private Rectangle winPositionTile14;
        //Rectangle for Tile 15 Win Position
        private Rectangle winPositionTile15;
        //Rectangle for Tile 16 Win Position
        private Rectangle winPositionTile16;
        #endregion
        //Region for the custom Tiles Class
        //Tiles Class was created to simplify the process of assigning X,Y and Source Rectangles for use in drawing and moving the image fragments throughout the game
        //Individual Tiles are declared as I was unable to find a satisfactory way to generate tiles during the game
        //This does limit the overall scalability of the game as to have different variations of tiles for different levels of difficulty within the game 
        //they would need to be seperately coded. Given more time a solution could probably have been found.
        #region Tiles
        //Tile 1 Tile Class Object
        private Tile tile1;
        //Tile 2 Tile Class Object
        private Tile tile2;
        //Tile 3 Tile Class Object
        private Tile tile3;
        //Tile 4 Tile Class Object
        private Tile tile4;
        //Tile 5 Tile Class Object
        private Tile tile5;
        //Tile 6 Tile Class Object
        private Tile tile6;
        //Tile 7 Tile Class Object
        private Tile tile7;
        //Tile 8 Tile Class Object
        private Tile tile8;
        //Tile 9 Tile Class Object
        private Tile tile9;
        //Tile 10 Tile Class Object
        private Tile tile10;
        //Tile 11 Tile Class Object
        private Tile tile11;
        //Tile 12 Tile Class Object
        private Tile tile12;
        //Tile 13 Tile Class Object
        private Tile tile13;
        //Tile 14 Tile Class Object
        private Tile tile14;
        //Tile 15 Tile Class Object
        private Tile tile15;
        //Tile 16 Tile Class Object
        private Tile tile16;
        #endregion
        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //Sets the game window to 1920x1080 
            //Resolution is used as it is still the most common display resolution
            _graphics.PreferredBackBufferWidth = gameWindowWidth;
            _graphics.PreferredBackBufferHeight = gameWindowHeight;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            //Region containing all content that is loaded for use in the game
            //Contains sub regions for specific Content
            #region Loaded Content
            //Region for variables that are used by options to count moves/minutes and seconds elapsed
            #region Moves/Minutes and Seconds Counters
            moves = 0;
            milliseconds = 0;
            framerate = 0;
            seconds = 0;
            minutes = 0;
            #endregion
            //Region for booleans that are set at the start of the game
            #region Booleans
            //Sets the bool value to false
            tryAgain = false;
            isShuffled = false;
            isShuffledMessage = false;
            beingShuffled = false;
            cantChange = false;
            originTilePos = false;
            mainMenu = false;
            credits = false;
            options = false;
            disableMusic = false;
            enableCounter = false;
            enableTimer = false;
            enableTileNumbers = false;
            startTimer = false;
            puzzleSolved = false;
            displayWinButtons = false;
            //Sets the bool value to truth
            displayControls = true;
            #endregion
            //Region for Images and the Arrays to store Puzzle Images and Button Images that are loaded for use within the game
            #region Images
            background = Content.Load<Texture2D>("Textures/Backgrounds/background");
            introBackground = Content.Load<Texture2D>("Textures/Backgrounds/introBackground");
            introTextBox = Content.Load<Texture2D>("Textures/Backgrounds/introTextBox");
            buttonMenu = Content.Load<Texture2D>("Textures/Buttons/buttonMenu");
            button1Red = Content.Load<Texture2D>("Textures/Buttons/button1Red");
            button2Green = Content.Load<Texture2D>("Textures/Buttons/button2Green");
            button3Yellow = Content.Load<Texture2D>("Textures/Buttons/button3Yellow");
            button4Red = Content.Load<Texture2D>("Textures/Buttons/button4Red");
            textBubble = Content.Load<Texture2D>("Textures/Buttons/textBubble");
            puzzleImageColosseum = Content.Load<Texture2D>("Textures/PuzzleImages/puzzleImageColosseum");
            puzzleImageGreatWall = Content.Load<Texture2D>("Textures/PuzzleImages/puzzleImageGreatWall");
            puzzleImageParthenon = Content.Load<Texture2D>("Textures/PuzzleImages/puzzleImageParthenon");
            puzzleImageRushmore = Content.Load<Texture2D>("Textures/PuzzleImages/puzzleImageRushmore");
            puzzleImageSphynx = Content.Load<Texture2D>("Textures/PuzzleImages/puzzleImageSphynx");
            puzzleImageTajMahal = Content.Load<Texture2D>("Textures/PuzzleImages/puzzleImageTajMahal");

            puzzleImages = new Texture2D[]
            {
                puzzleImageColosseum,
                puzzleImageGreatWall,
                puzzleImageParthenon,
                puzzleImageRushmore,
                puzzleImageSphynx,
                puzzleImageTajMahal,
            };
            buttonImages = new Texture2D[]
            {
                button1Red,
                button2Green,
            };
            #endregion
            //Region for SpriteFonts that are loaded for use within the game
            #region SpriteFonts
            titleFont = Content.Load<SpriteFont>("Fonts/titleScreenFont");
            pressEnterFont = Content.Load<SpriteFont>("Fonts/pressEnterFont");
            introFont = Content.Load<SpriteFont>("Fonts/introFont");
            textBoxFont = Content.Load<SpriteFont>("Fonts/textBoxFont");
            #endregion
            //Region for Music/SFX that are loaded for use within the game
            #region Music/SFX
            backgroundMusic = Content.Load<Song>("Sounds/Music/quiescentInTime");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.03f;
            tileMovementEffect = Content.Load<SoundEffect>("Sounds/SFX/tileMovementEffect");
            buttonClickEffect = Content.Load<SoundEffect>("Sounds/SFX/buttonClickEffect");
            #endregion
            //Region for Array Increment/Decrementers that are used in the game
            #region Array Increment/Decrementers
            //Base value for imageNumber that can be incremented or decremented
            imageNumber = 0;
            //Base value for buttonNumbers that can be incremented or decremented
            buttonNumber1 = 0;
            buttonNumber2 = 0;
            buttonNumber3 = 0;
            buttonNumber4 = 0;
            #endregion
            //Region for assigning values to imageStartX/Y variables
            #region ImageCoords
            imageStartX = (gameWindowWidth / 2) - (puzzleImages[imageNumber].Width / 2);
            imageStartY = (gameWindowHeight / 2) - (puzzleImages[imageNumber].Height / 2);
            #endregion
            //Region for assigning values to gridX/Y variables
            #region ImageGridCoords
            gridX0 = 0;
            gridX1 = (puzzleImages[imageNumber].Width / 4) * 1;
            gridX2 = (puzzleImages[imageNumber].Width / 4) * 2;
            gridX3 = (puzzleImages[imageNumber].Width / 4) * 3;
            gridY0 = 0;
            gridY1 = (puzzleImages[imageNumber].Height / 4) * 1;
            gridY2 = (puzzleImages[imageNumber].Height / 4) * 2;
            gridY3 = (puzzleImages[imageNumber].Height / 4) * 3;
            #endregion
            //Region for assigning values to the various button coord variables
            #region ButtonCoords
            button1RedCoords = new Vector2(386, 367);
            button2GreenCoords = new Vector2(387, 458);
            button3YellowCoords = new Vector2(387, 548);
            button4RedCoords = new Vector2(386, 633);
            buttonPlayCoords = new Vector2(680, 350);
            buttonOptionsCoords = new Vector2(680, 550);
            buttonCreditsCoords = new Vector2(680, 750);
            buttonBackCoords = new Vector2(680, 650);
            buttonExitCoords = new Vector2(680, 750);
            buttonOptionToggle1Coords = new Vector2(825, 300);
            buttonOptionToggle2Coords = new Vector2(825, 400);
            buttonOptionToggle3Coords = new Vector2(900, 500);
            buttonOptionToggle4Coords = new Vector2(900, 600);
            #endregion
            //Region for assigning values to the Arrays that were already declared
            #region Lists
            gridXCoords = new int[]
            {
                (gridX0+imageStartX),
                (gridX1+imageStartX+10),
                (gridX2+imageStartX+20),
                (gridX3+imageStartX+30),
                (gridX0+imageStartX),
                (gridX1+imageStartX+10),
                (gridX2+imageStartX+20),
                (gridX3+imageStartX+30),
                (gridX0+imageStartX),
                (gridX1+imageStartX+10),
                (gridX2+imageStartX+20),
                (gridX3+imageStartX+30),
                (gridX0+imageStartX),
                (gridX1+imageStartX+10),
                (gridX2+imageStartX+20),
                (gridX3+imageStartX+30),
            };
            gridYCoords = new int[]
            {
                (gridY0+imageStartY),
                (gridY0+imageStartY),
                (gridY0+imageStartY),
                (gridY0+imageStartY),
                (gridY1+imageStartY+10),
                (gridY1+imageStartY+10),
                (gridY1+imageStartY+10),
                (gridY1+imageStartY+10),
                (gridY2+imageStartY+20),
                (gridY2+imageStartY+20),
                (gridY2+imageStartY+20),
                (gridY2+imageStartY+20),
                (gridY3+imageStartY+30),
                (gridY3+imageStartY+30),
                (gridY3+imageStartY+30),
                (gridY3+imageStartY+30),
            };
            #endregion
            //Region for assigning values to the TileImage Rectangles
            #region TileImages
            tileImage1 = new Rectangle(gridX0, gridY0, gridX1, gridY1);
            tileImage2 = new Rectangle(gridX1, gridY0, gridX1, gridY1);
            tileImage3 = new Rectangle(gridX2, gridY0, gridX1, gridY1);
            tileImage4 = new Rectangle(gridX3, gridY0, gridX1, gridY1);
            tileImage5 = new Rectangle(gridX0, gridY1, gridX1, gridY1);
            tileImage6 = new Rectangle(gridX1, gridY1, gridX1, gridY1);
            tileImage7 = new Rectangle(gridX2, gridY1, gridX1, gridY1);
            tileImage8 = new Rectangle(gridX3, gridY1, gridX1, gridY1);
            tileImage9 = new Rectangle(gridX0, gridY2, gridX1, gridY1);
            tileImage10 = new Rectangle(gridX1, gridY2, gridX1, gridY1);
            tileImage11 = new Rectangle(gridX2, gridY2, gridX1, gridY1);
            tileImage12 = new Rectangle(gridX3, gridY2, gridX1, gridY1);
            tileImage13 = new Rectangle(gridX0, gridY3, gridX1, gridY1);
            tileImage14 = new Rectangle(gridX1, gridY3, gridX1, gridY1);
            tileImage15 = new Rectangle(gridX2, gridY3, gridX1, gridY1);
            tileImage16 = new Rectangle(gridX3, gridY3, gridX1, gridY1);
            #endregion
            //Region for assigning values to the WinPosition Rectangles
            #region TileWinPosition
            winPositionTile1 = new Rectangle(gridXCoords[0], gridYCoords[0], gridX1, gridY1);
            winPositionTile2 = new Rectangle(gridXCoords[1], gridYCoords[1], gridX1, gridY1);
            winPositionTile3 = new Rectangle(gridXCoords[2], gridYCoords[2], gridX1, gridY1);
            winPositionTile4 = new Rectangle(gridXCoords[3], gridYCoords[3], gridX1, gridY1);
            winPositionTile5 = new Rectangle(gridXCoords[4], gridYCoords[4], gridX1, gridY1);
            winPositionTile6 = new Rectangle(gridXCoords[5], gridYCoords[5], gridX1, gridY1);
            winPositionTile7 = new Rectangle(gridXCoords[6], gridYCoords[6], gridX1, gridY1);
            winPositionTile8 = new Rectangle(gridXCoords[7], gridYCoords[7], gridX1, gridY1);
            winPositionTile9 = new Rectangle(gridXCoords[8], gridYCoords[8], gridX1, gridY1);
            winPositionTile10 = new Rectangle(gridXCoords[9], gridYCoords[9], gridX1, gridY1);
            winPositionTile11 = new Rectangle(gridXCoords[10], gridYCoords[10], gridX1, gridY1);
            winPositionTile12 = new Rectangle(gridXCoords[11], gridYCoords[11], gridX1, gridY1);
            winPositionTile13 = new Rectangle(gridXCoords[12], gridYCoords[12], gridX1, gridY1);
            winPositionTile14 = new Rectangle(gridXCoords[13], gridYCoords[13], gridX1, gridY1);
            winPositionTile15 = new Rectangle(gridXCoords[14], gridYCoords[14], gridX1, gridY1);
            winPositionTile16 = new Rectangle(gridXCoords[15], gridYCoords[15], gridX1, gridY1);
            #endregion
            //Region for assigning values to the previously declared Tile Class Objects
            #region Tiles
            tile1 = new Tile(gridXCoords[0], gridYCoords[0], tileImage1);
            tile2 = new Tile(gridXCoords[1], gridYCoords[1], tileImage2);
            tile3 = new Tile(gridXCoords[2], gridYCoords[2], tileImage3);
            tile4 = new Tile(gridXCoords[3], gridYCoords[3], tileImage4);
            tile5 = new Tile(gridXCoords[4], gridYCoords[4], tileImage5);
            tile6 = new Tile(gridXCoords[5], gridYCoords[5], tileImage6);
            tile7 = new Tile(gridXCoords[6], gridYCoords[6], tileImage7);
            tile8 = new Tile(gridXCoords[7], gridYCoords[7], tileImage8);
            tile9 = new Tile(gridXCoords[8], gridYCoords[8], tileImage9);
            tile10 = new Tile(gridXCoords[9], gridYCoords[9], tileImage10);
            tile11 = new Tile(gridXCoords[10], gridYCoords[10], tileImage11);
            tile12 = new Tile(gridXCoords[11], gridYCoords[11], tileImage12);
            tile13 = new Tile(gridXCoords[12], gridYCoords[12], tileImage13);
            tile14 = new Tile(gridXCoords[13], gridYCoords[13], tileImage14);
            tile15 = new Tile(gridXCoords[14], gridYCoords[14], tileImage15);
            tile16 = new Tile(gridXCoords[15], gridYCoords[15], tileImage16);
            #endregion
            //Region for Tile Array GameBoard, used to draw all created tiles
            #region GameBoard
            gameBoard = new Tile[]
            {
            tile1,
            tile2,
            tile3,
            tile4,
            tile5,
            tile6,
            tile7,
            tile8,
            tile9,
            tile10,
            tile11,
            tile12,
            tile13,
            tile14,
            tile15,
            tile16,
            };
            #endregion
            #endregion
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //Region for the variables given values in update to ensure that they are properly updated
            #region Variables Requiring Updates
            //Sets the previously declared variables for KeyState
            //Done in update to ensure that they are updated
            #region Keyboard State
            previousKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            #endregion
            //Sets the previously declared variables for MouseState
            //Also includes the collision Rectangle for the mouse pointer
            //Done in update to ensure that they are updated
            #region Mouse State
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            mouseRectangle = new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);
            #endregion
            //Region for all of the various Collision Rectangles that are used across various gameStates
            //Each state has a specified region, unless no rectangles are used. Rectangles that are used by multiple states are listed in the first state they appear in
            //They are commented to alert if they are used in a different state
            //Done in update to ensure that they are updated
            #region Collision Rectangles
            //Region for Main Menu Collision Rectangles
            #region Main Menu Collision Rectangles
            //Collision Rectangles for use in the MainMenu Screen
            buttonPlayRectangle = new Rectangle((int)buttonPlayCoords.X, (int)buttonPlayCoords.Y, buttonMenu.Width, buttonMenu.Height);
            buttonOptionsRectangle = new Rectangle((int)buttonOptionsCoords.X, (int)buttonOptionsCoords.Y, buttonMenu.Width, buttonMenu.Height);
            buttonCreditsRectangle = new Rectangle((int)buttonCreditsCoords.X, (int)buttonCreditsCoords.Y, buttonMenu.Width, buttonMenu.Height);
            //Rectangle used by the [Back] button for the Options and Credits subscreens, the [Back] on the PauseScreen, and by the [Replay] button on the WinScreen
            buttonBackRectangle = new Rectangle((int)buttonBackCoords.X, (int)buttonBackCoords.Y, buttonMenu.Width, buttonMenu.Height);
            buttonOptionToggle1Rectangle = new Rectangle((int)buttonOptionToggle1Coords.X, (int)buttonOptionToggle1Coords.Y, buttonImages[buttonNumber1].Width / 3, buttonImages[buttonNumber1].Height / 3);
            buttonOptionToggle2Rectangle = new Rectangle((int)buttonOptionToggle2Coords.X, (int)buttonOptionToggle2Coords.Y, buttonImages[buttonNumber2].Width / 3, buttonImages[buttonNumber2].Height / 3);
            buttonOptionToggle3Rectangle = new Rectangle((int)buttonOptionToggle3Coords.X, (int)buttonOptionToggle3Coords.Y, buttonImages[buttonNumber3].Width / 3, buttonImages[buttonNumber3].Height / 3);
            buttonOptionToggle4Rectangle = new Rectangle((int)buttonOptionToggle4Coords.X, (int)buttonOptionToggle4Coords.Y, buttonImages[buttonNumber4].Width / 3, buttonImages[buttonNumber4].Height / 3);
            #endregion
            //Collision Rectangles for use in the ImageSelect Screen
            //Sets the previously declared variables for TileCollision
            #region Image Select Collision Rectangles
            button1RedRectangle = new Rectangle(386, 367, button1Red.Width, button1Red.Height);
            button2GreenRectangle = new Rectangle(387, 458, button2Green.Width, button2Green.Height);
            button3YellowRectangle = new Rectangle(387, 549, button3Yellow.Width, button3Yellow.Height);
            button4RedRectangle = new Rectangle(386, 633, button4Red.Width, button4Red.Height);
            collisionTile1 = new Rectangle(gameBoard[0].X, gameBoard[0].Y, gridX1, gridY1);
            collisionTile2 = new Rectangle(gameBoard[1].X, gameBoard[1].Y, gridX1, gridY1);
            collisionTile3 = new Rectangle(gameBoard[2].X, gameBoard[2].Y, gridX1, gridY1);
            collisionTile4 = new Rectangle(gameBoard[3].X, gameBoard[3].Y, gridX1, gridY1);
            collisionTile5 = new Rectangle(gameBoard[4].X, gameBoard[4].Y, gridX1, gridY1);
            collisionTile6 = new Rectangle(gameBoard[5].X, gameBoard[5].Y, gridX1, gridY1);
            collisionTile7 = new Rectangle(gameBoard[6].X, gameBoard[6].Y, gridX1, gridY1);
            collisionTile8 = new Rectangle(gameBoard[7].X, gameBoard[7].Y, gridX1, gridY1);
            collisionTile9 = new Rectangle(gameBoard[8].X, gameBoard[8].Y, gridX1, gridY1);
            collisionTile10 = new Rectangle(gameBoard[9].X, gameBoard[9].Y, gridX1, gridY1);
            collisionTile11 = new Rectangle(gameBoard[10].X, gameBoard[10].Y, gridX1, gridY1);
            collisionTile12 = new Rectangle(gameBoard[11].X, gameBoard[11].Y, gridX1, gridY1);
            collisionTile13 = new Rectangle(gameBoard[12].X, gameBoard[12].Y, gridX1, gridY1);
            collisionTile14 = new Rectangle(gameBoard[13].X, gameBoard[13].Y, gridX1, gridY1);
            collisionTile15 = new Rectangle(gameBoard[14].X, gameBoard[14].Y, gridX1, gridY1);
            collisionTile16 = new Rectangle(gameBoard[15].X, gameBoard[15].Y, gridX1, gridY1);
            #endregion
            //Collision Rectangles for use in the Win Screen
            #region Win Screen Collision Rectangles
            buttonExitRectangle = new Rectangle((int)buttonExitCoords.X, (int)buttonExitCoords.Y, buttonMenu.Width, buttonMenu.Height);
            #endregion
            #endregion
            #endregion
            //Region for the Switch Statement used in conjunction with declared gameStates to update various elements of the game
            #region State Switch Statement
            switch (currentState)
            {
                case GameState.startScreen:
                    #region StartScreen
                    //If statement for moving to the Intro Screen state
                    if (currentKeyState.IsKeyUp(Keys.Enter) && previousKeyState.IsKeyDown(Keys.Enter))
                    {
                        currentState = GameState.introScreen;
                        buttonClickEffect.Play();
                    }
                    break;
                #endregion
                case GameState.introScreen:
                    #region IntroScreen
                    //If statement for moving to the Main Menu state
                    if (currentKeyState.IsKeyUp(Keys.Enter) && previousKeyState.IsKeyDown(Keys.Enter))
                    {
                        mainMenu = true;
                        currentState = GameState.mainMenu;
                        buttonClickEffect.Play();
                    }
                    break;
                #endregion
                case GameState.mainMenu:
                    #region MainMenu
                    //Region for Play Button If Statements
                    #region Play Button Statements
                    //If statement for Play Button
                    if (mouseRectangle.Intersects(buttonPlayRectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && mainMenu == true)
                        {
                            currentState = GameState.imageSelect;
                            buttonClickEffect.Play();
                        }
                    }
                    #endregion
                    //Region for Options Button If Statements
                    #region Options Button Statements
                    //If statement for Options Button
                    if (mouseRectangle.Intersects(buttonOptionsRectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && mainMenu == true)
                        {
                            mainMenu = false;
                            options = true;
                            buttonClickEffect.Play();
                        }
                    }
                    //Region for Options Toggle Button If Statements
                    #region Options Toggle Button Statements
                    //If statement for Options Button 1
                    if (mouseRectangle.Intersects(buttonOptionToggle1Rectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && options == true
                        && disableMusic == false)
                        {
                            disableMusic = true;
                            buttonNumber1 = 1;
                            MediaPlayer.Stop();
                            buttonClickEffect.Play();
                        }
                        else if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && options == true
                        && disableMusic == true)
                        {
                            disableMusic = false;
                            buttonNumber1 = 0;
                            MediaPlayer.Play(backgroundMusic);
                            buttonClickEffect.Play();
                        }
                    }
                    //If statement for Options Button 2
                    if (mouseRectangle.Intersects(buttonOptionToggle2Rectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && options == true
                        && enableTimer == false)
                        {
                            enableTimer = true;
                            buttonNumber2 = 1;
                            buttonClickEffect.Play();
                        }
                        else if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && options == true
                        && enableTimer == true)
                        {
                            enableTimer = false;
                            buttonNumber2 = 0;
                            buttonClickEffect.Play();
                        }
                    }
                    //If statement for Options Button 3
                    if (mouseRectangle.Intersects(buttonOptionToggle3Rectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && options == true
                        && enableCounter == false)
                        {
                            enableCounter = true;
                            buttonNumber3 = 1;
                            buttonClickEffect.Play();
                        }
                        else if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && options == true
                        && enableCounter == true)
                        {
                            enableCounter = false;
                            buttonNumber3 = 0;
                            buttonClickEffect.Play();
                        }
                    }
                    //If statement for Options Button 4
                    if (mouseRectangle.Intersects(buttonOptionToggle4Rectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && options == true
                        && enableTileNumbers == false)
                        {
                            enableTileNumbers = true;
                            buttonNumber4 = 1;
                            buttonClickEffect.Play();
                        }
                        else if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && options == true
                        && enableTileNumbers == true)
                        {
                            enableTileNumbers = false;
                            buttonNumber4 = 0;
                            buttonClickEffect.Play();
                        }
                    }
                    #endregion
                    //If statement for Back Button(Options Screen)
                    if (mouseRectangle.Intersects(buttonBackRectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && options == true)
                        {
                            options = false;
                            mainMenu = true;
                            buttonClickEffect.Play();
                        }
                    }
                    #endregion
                    //Region for Credits Button If Statements
                    #region Credits Button Statements
                    //If statement for Credits Button
                    if (mouseRectangle.Intersects(buttonCreditsRectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && mainMenu == true)
                        {
                            mainMenu = false;
                            credits = true;
                            buttonClickEffect.Play();
                        }
                    }
                    //If statement for Back Button(Credits Screen)
                    if (mouseRectangle.Intersects(buttonBackRectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && credits == true)
                        {
                            credits = false;
                            mainMenu = true;
                            buttonClickEffect.Play();
                        }
                    }
                    #endregion
                    break;
                #endregion
                case GameState.imageSelect:
                    #region ImageSelect
                    //If statement for switching to the pause screen from the image select screen
                    if (currentKeyState.IsKeyUp(Keys.Space)
                        && previousKeyState.IsKeyDown(Keys.Space)
                        && beingShuffled == false)
                    {
                        currentState = GameState.pauseScreen;
                        buttonClickEffect.Play();
                    }
                    //Region for the if statements for tile collision
                    //If statements are used so that when Tiles collide their X and Y values are swapped
                    //Individual X and Y values are used instead of Vector2s due to the way that Vector2s interact with the custom tiles class
                    //when they are stored within an array as the tiles currently are. Vector2s were attempted to be used but promptly broke all of the tile movement
                    //Additional testing was done in a seperate project file to confirm the issue was indeed due to the inflexibilty of Vecotr2s within a class stored in an array
                    //Also included in the region are the previously declared variables for Previous/CurrentPosition
                    //Done in update to ensure that they are updated
                    #region Tile Collision
                    //Previous/Current Position Variables
                    previousPosition = currentPosition;
                    currentPosition = new Vector2(gameBoard[15].X, gameBoard[15].Y);
                    //Tile 1 Collision Statement
                    if (collisionTile16.Intersects(collisionTile1))
                    {
                        gameBoard[15].X = gameBoard[0].X;
                        gameBoard[15].Y = gameBoard[0].Y;
                        gameBoard[0].X = (int)previousPosition.X;
                        gameBoard[0].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }
                    //Tile 2 Collision Statement
                    if (collisionTile16.Intersects(collisionTile2))
                    {
                        gameBoard[15].X = gameBoard[1].X;
                        gameBoard[15].Y = gameBoard[1].Y;
                        gameBoard[1].X = (int)previousPosition.X;
                        gameBoard[1].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }
                    //Tile 3 Collision Statement
                    if (collisionTile16.Intersects(collisionTile3))
                    {
                        gameBoard[15].X = gameBoard[2].X;
                        gameBoard[15].Y = gameBoard[2].Y;
                        gameBoard[2].X = (int)previousPosition.X;
                        gameBoard[2].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }
                    //Tile 4 Collision Statement
                    if (collisionTile16.Intersects(collisionTile4))
                    {
                        gameBoard[15].X = gameBoard[3].X;
                        gameBoard[15].Y = gameBoard[3].Y;
                        gameBoard[3].X = (int)previousPosition.X;
                        gameBoard[3].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }
                    //Tile 5 Collision Statement
                    if (collisionTile16.Intersects(collisionTile5))
                    {
                        gameBoard[15].X = gameBoard[4].X;
                        gameBoard[15].Y = gameBoard[4].Y;
                        gameBoard[4].X = (int)previousPosition.X;
                        gameBoard[4].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }
                    //Tile 6 Collision Statement
                    if (collisionTile16.Intersects(collisionTile6))
                    {
                        gameBoard[15].X = gameBoard[5].X;
                        gameBoard[15].Y = gameBoard[5].Y;
                        gameBoard[5].X = (int)previousPosition.X;
                        gameBoard[5].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }
                    //Tile 7 Collision Statement
                    if (collisionTile16.Intersects(collisionTile7))
                    {
                        gameBoard[15].X = gameBoard[6].X;
                        gameBoard[15].Y = gameBoard[6].Y;
                        gameBoard[6].X = (int)previousPosition.X;
                        gameBoard[6].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }
                    //Tile 8 Collision Statement
                    if (collisionTile16.Intersects(collisionTile8))
                    {
                        gameBoard[15].X = gameBoard[7].X;
                        gameBoard[15].Y = gameBoard[7].Y;
                        gameBoard[7].X = (int)previousPosition.X;
                        gameBoard[7].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }
                    //Tile 9 Collision Statement
                    if (collisionTile16.Intersects(collisionTile9))
                    {
                        gameBoard[15].X = gameBoard[8].X;
                        gameBoard[15].Y = gameBoard[8].Y;
                        gameBoard[8].X = (int)previousPosition.X;
                        gameBoard[8].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }
                    //Tile 10 Collision Statement
                    if (collisionTile16.Intersects(collisionTile10))
                    {
                        gameBoard[15].X = gameBoard[9].X;
                        gameBoard[15].Y = gameBoard[9].Y;
                        gameBoard[9].X = (int)previousPosition.X;
                        gameBoard[9].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }
                    //Tile 11 Collision Statement
                    if (collisionTile16.Intersects(collisionTile11))
                    {
                        gameBoard[15].X = gameBoard[10].X;
                        gameBoard[15].Y = gameBoard[10].Y;
                        gameBoard[10].X = (int)previousPosition.X;
                        gameBoard[10].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }
                    //Tile 12 Collision Statement
                    if (collisionTile16.Intersects(collisionTile12))
                    {
                        gameBoard[15].X = gameBoard[11].X;
                        gameBoard[15].Y = gameBoard[11].Y;
                        gameBoard[11].X = (int)previousPosition.X;
                        gameBoard[11].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }
                    //Tile 13 Collision Statement
                    if (collisionTile16.Intersects(collisionTile13))
                    {
                        gameBoard[15].X = gameBoard[12].X;
                        gameBoard[15].Y = gameBoard[12].Y;
                        gameBoard[12].X = (int)previousPosition.X;
                        gameBoard[12].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }
                    //Tile 14 Collision Statement
                    if (collisionTile16.Intersects(collisionTile14))
                    {
                        gameBoard[15].X = gameBoard[13].X;
                        gameBoard[15].Y = gameBoard[13].Y;
                        gameBoard[13].X = (int)previousPosition.X;
                        gameBoard[13].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }
                    //Tile 15 Collision Statement
                    if (collisionTile16.Intersects(collisionTile15))
                    {
                        gameBoard[15].X = gameBoard[14].X;
                        gameBoard[15].Y = gameBoard[14].Y;
                        gameBoard[14].X = (int)previousPosition.X;
                        gameBoard[14].Y = (int)previousPosition.Y;
                        tileMovementEffect.Play();
                    }

                    #endregion
                    //If statements for moving the movable tile within the game via keyboard input
                    #region Tile Movement
                    //Statement for moving Tiles below the blank tile into its space
                    if (currentKeyState.IsKeyUp(Keys.W) && previousKeyState.IsKeyDown(Keys.W) && gameBoard[15].Y < gridYCoords[15])
                    {
                        gameBoard[15].Y += 11;
                        moves++;
                    }
                    //Statement for moving Tiles above the blank tile into its space
                    if (currentKeyState.IsKeyUp(Keys.S) && previousKeyState.IsKeyDown(Keys.S) && gameBoard[15].Y > gridYCoords[0])
                    {
                        gameBoard[15].Y -= 11;
                        moves++;
                    }
                    //Statement for moving Tiles to the left of the blank tile into its space
                    if (currentKeyState.IsKeyUp(Keys.A) && previousKeyState.IsKeyDown(Keys.A) && gameBoard[15].X < gridXCoords[15])
                    {
                        gameBoard[15].X += 31;
                        moves++;
                    }
                    //Statement for moving Tiles to the right of the blank tile into its space
                    if (currentKeyState.IsKeyUp(Keys.D) && previousKeyState.IsKeyDown(Keys.D) && gameBoard[15].X > gridXCoords[0])
                    {
                        gameBoard[15].X -= 31;
                        moves++;
                    }
                    #endregion
                    //If statements for handling user interactions with the various buttons on the Image Select Screen
                    #region Button Click
                    //If statement for changing the puzzle image(++)
                    if (mouseRectangle.Intersects(button1RedRectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && imageNumber < puzzleImages.Length - 1
                        && isShuffled == false)
                        {
                            imageNumber++;
                            buttonClickEffect.Play();
                        }
                        else if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && isShuffled == true)
                        {
                            displayControls = false;
                            cantChange = true;
                            buttonClickEffect.Play();
                            ResetCantChange();
                        }
                    }
                    //If statement for CheckPosition Button
                    if (mouseRectangle.Intersects(button2GreenRectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && isShuffled == true)
                        {
                            CheckPosition();
                            buttonClickEffect.Play();
                        }
                        else if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && isShuffled == false)
                        {
                            displayControls = false;
                            originTilePos = true;
                            buttonClickEffect.Play();
                            ResetOriginTilePos();
                        }
                    }
                    //If statement for Shuffle Button
                    if (mouseRectangle.Intersects(button3YellowRectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && isShuffled == false)
                        {
                            Shuffle();
                            buttonClickEffect.Play();
                            isShuffled = true;
                        }
                        else if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && isShuffled == true)
                        {
                            displayControls = false;
                            isShuffledMessage = true;
                            buttonClickEffect.Play();
                            ResetIsShuffledMessage();
                        }
                    }
                    //If statement for changing the puzzle image(--)
                    if (mouseRectangle.Intersects(button4RedRectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && imageNumber > 0
                        && isShuffled == false)
                        {
                            imageNumber--;
                            buttonClickEffect.Play();
                        }
                        else if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && isShuffled == true)
                        {
                            displayControls = false;
                            cantChange = true;
                            buttonClickEffect.Play();
                            ResetCantChange();
                        }
                    }
                    #endregion
                    //Region for Statement responsible for starting Timer
                    #region Timer
                    //Statement for running the game timer
                    if (isShuffled == true && startTimer == true)
                    {
                        Timer(gameTime);
                    }
                    #endregion
                    break;
                #endregion
                case GameState.pauseScreen:
                    #region PauseScreen
                    //If statement for Back Button(Pause Screen)
                    if (mouseRectangle.Intersects(buttonBackRectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && currentState == GameState.pauseScreen)
                        {
                            currentState = GameState.imageSelect;
                            buttonClickEffect.Play();
                        }
                    }
                    break;
                #endregion
                case GameState.winScreen:
                    #region WinScreen
                    //Statement for the Replay Button on the Win Screen
                    if (mouseRectangle.Intersects(buttonBackRectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && currentState == GameState.winScreen
                        && puzzleSolved == false
                        && displayWinButtons == true)
                        {
                            currentState = GameState.imageSelect;
                            buttonClickEffect.Play();
                            isShuffled = false;
                            ResetTimerCounts();
                            ResetDisplayWinButtons();
                        }
                    }
                    //Statement for the Exit Button on the Win Screen
                    if (mouseRectangle.Intersects(buttonExitRectangle))
                    {
                        if (currentMouseState.LeftButton == ButtonState.Released
                        && previousMouseState.LeftButton == ButtonState.Pressed
                        && currentState == GameState.winScreen
                        && puzzleSolved == false
                        && displayWinButtons == true)
                        {
                            buttonClickEffect.Play();
                            Exit();
                        }
                    }
                    break;
                    #endregion
            }
            #endregion
            base.Update(gameTime);
        }
        //Region for custom Functions created for game operations
        #region Custom Functions
        //Region for custom functions created to draw various different lines of text in the game
        //Custom functions used to reduce repeat code and help with scalability
        #region DrawText Custom Functions
        //Custom function to draw lines of text for use in the game intro screen
        private void DrawIntroText()
        {
            int x = (gameWindowWidth / 2 - introTextBox.Width / 2) + 15;
            int y = (gameWindowHeight / 2 - introTextBox.Height / 2) + 25;
            int height = 50;

            for (int i = 0; i < introText.Length; i++)
            {
                _spriteBatch.DrawString(introFont, introText[i], new Vector2(x, y), Color.Yellow);
                y += height;
            }
        }
        //Custom function to draw the different elements of the options menu including text and buttons
        private void DrawOptionsMenu()
        {
            int x = 575;
            int y = 300;
            int height = 100;

            for (int i = 0; i < optionsText.Length; i++)
            {
                _spriteBatch.DrawString(textBoxFont, optionsText[i], new Vector2(x, y), Color.Yellow);
                y += height;
            }
        }
        //Custom function to draw lines of text for use in the credit screen
        private void DrawCreditsText()
        {
            int x = 575;
            int y = 300;
            int height = 50;

            for (int i = 0; i < creditsText.Length; i++)
            {
                _spriteBatch.DrawString(textBoxFont, creditsText[i], new Vector2(x, y), Color.Yellow);
                y += height;
            }
        }
        //Custom function to draw lines of text for use in the first set of controls text in the textbox
        private void DrawControlsText()
        {
            int x = 1428;
            int y = 120;
            int height = 30;

            for (int i = 0; i < controlsText.Length; i++)
            {
                _spriteBatch.DrawString(textBoxFont, controlsText[i], new Vector2(x, y), Color.Yellow);
                y += height;
            }
        }
        //Custom function to draw lines of text for use in the second set of controls text in the textbox
        private void DrawControlsText2()
        {
            int x = 1428;
            int y = 120;
            int height = 30;

            for (int i = 0; i < controlsText2.Length; i++)
            {
                _spriteBatch.DrawString(textBoxFont, controlsText2[i], new Vector2(x, y), Color.Yellow);
                y += height;
            }
        }
        #endregion
        //Region for custom functions created to draw the game tiles and the numbers that can be displayed on them
        //Custom functions used to reduce repeat code and help with scalability
        #region DrawTiles and TileNumbers
        //Custom function to draw the game tiles 
        private void DrawGameTiles()
        {
            for (int i = 0; i < gameBoard.Length - 1; i++)
            {
                _spriteBatch.Draw(puzzleImages[imageNumber], new Vector2(gameBoard[i].X, gameBoard[i].Y), gameBoard[i].TileImage, Color.White);
            }
        }
        //Custom function to draw numbers for each tile to help with solving the puzzle
        private void DrawTileNumbers()
        {
            for (int i = 0; i < gameBoard.Length; i++)
            {
                _spriteBatch.DrawString(introFont, "" + (i + 1), new Vector2(gameBoard[i].X, gameBoard[i].Y), Color.Yellow);
            }
        }
        #endregion
        //Region for bespoke custom functions created to execute a specific singular task
        //Functions include Shuffling tile, Checking tile positions, Displaying elements after the puzzle is solved and implementing a game timer
        //These functions were created to reduce code clutter and allow implementation to be clean
        #region Bespoke Functions
        //Custom function for shuffling the tiles
        //Async function used to allow for use of Task.Delay
        //Task.Delay is used to ensure that the tile movement can be done during each loop cycle
        private async void Shuffle()
        {
            for (int i = 0; i < 225; i++)
            {
                beingShuffled = true;
                currentRnd = rnd.Next(1, 5);
                switch (currentRnd)
                {
                    case 1:
                        if (gameBoard[15].Y < gridYCoords[15])
                        {
                            gameBoard[15].Y += 11;
                        }
                        break;
                    case 2:
                        if (gameBoard[15].Y > gridYCoords[0])
                        {
                            gameBoard[15].Y -= 11;
                        }
                        break;
                    case 3:
                        if (gameBoard[15].X < gridXCoords[15])
                        {
                            gameBoard[15].X += 31;
                        }
                        break;
                    case 4:
                        if (gameBoard[15].X > gridXCoords[0])
                        {
                            gameBoard[15].X -= 31;
                        }
                        break;
                }
                await Task.Delay(75);
            }
            beingShuffled = false;
            startTimer = true;
        }
        //Custom function for Checking the if the Tiles are in the win positions
        //Function just contains an if and else statement for checking win positions
        private void CheckPosition()
        {
            if (winPositionTile1 == collisionTile1
                && winPositionTile2 == collisionTile2
                && winPositionTile3 == collisionTile3
                && winPositionTile4 == collisionTile4
                && winPositionTile5 == collisionTile5
                && winPositionTile6 == collisionTile6
                && winPositionTile7 == collisionTile7
                && winPositionTile8 == collisionTile8
                && winPositionTile9 == collisionTile9
                && winPositionTile10 == collisionTile10
                && winPositionTile11 == collisionTile11
                && winPositionTile12 == collisionTile12
                && winPositionTile13 == collisionTile13
                && winPositionTile14 == collisionTile14
                && winPositionTile15 == collisionTile15
                && winPositionTile16 == collisionTile16)
            {
                currentState = GameState.winScreen;
                puzzleSolved = true;
            }
            else
            {
                displayControls = false;
                tryAgain = true;
                ResetTryAgain();
            }
        }
        //Custom function for displaying the full image after the puzzle has been solved and then displaying Win Screen Buttons
        //Async function used to allow for use of Task.Delay
        //Task.Delay is used to allow the image to be displayed before disappear
        private async void PuzzleSolved()
        {
            if (currentState == GameState.winScreen
                && puzzleSolved == true)
            {
                _spriteBatch.Draw(puzzleImages[imageNumber], new Vector2(gameBoard[0].X, gameBoard[0].Y), Color.White);
            }
            await Task.Delay(5000);
            puzzleSolved = false;
            displayWinButtons = true;
        }
        //Custom function creating a in game timer
        //Custom function used to clean up implementation
        //Due to oddities in the way gameTime is updated, I've had to parse it through multiple statements
        //This way I can have an accurate representation of how many seconds and minutes have passed in game
        private void Timer(GameTime gameTime)
        {
            milliseconds += gameTime.TotalGameTime.TotalMilliseconds;
            if (milliseconds >= 1000)
            {
                framerate++;
                milliseconds = 0;
            }
            if (framerate >= 60)
            {
                seconds++;
                framerate = 0;
            }
            if (seconds >= 60)
            {
                minutes++;
                seconds = 0;
            }

        }
        #endregion
        //Region for custom functions created to reset various booleans after a slight delay
        //Async function used to allow for use of Task.Delay
        //Task.Delay is used so that the drawn text can be easily removed after being displayed
        #region Boolean Reset Functions
        //Custom function for reseting the TryAgain boolean after a delay
        private async void ResetTryAgain()
        {
            await Task.Delay(3000);
            tryAgain = false;
            displayControls = true;
        }
        //Custom function for reseting the CantChange boolean after a delay
        private async void ResetCantChange()
        {
            await Task.Delay(3000);
            cantChange = false;
            displayControls = true;
        }
        //Custom function for reseting the OriginTilePos boolean after a delay
        private async void ResetOriginTilePos()
        {
            await Task.Delay(3000);
            originTilePos = false;
            displayControls = true;
        }
        //Custom function for reseting the IsShuffledMessage boolean after a delay
        private async void ResetIsShuffledMessage()
        {
            await Task.Delay(3000);
            isShuffledMessage = false;
            displayControls = true;
        }
        //Custom function for reseting the DisplayWinButtons boolean after a delay
        private async void ResetDisplayWinButtons()
        {
            await Task.Delay(3000);
            displayWinButtons = false;
        }
        //Custom function for reseting the game timer and move count after the puzzle has been solved
        private void ResetTimerCounts()
        {
            milliseconds = 0;
            framerate = 0;
            seconds = 0;
            minutes = 0;
            moves = 0;
            startTimer = false;
        }
        #endregion
        #endregion

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //Region for the Switch Statement used in conjunction with declared gameStates to draw various game sprites and texts
            #region State Switch Statement
            switch (currentState)
            {
                //Start Screen Drawing Functions
                case GameState.startScreen:
                    #region StartScreen
                    _spriteBatch.Begin();
                    _spriteBatch.Draw(introBackground, new Vector2(0, 0), Color.White);
                    _spriteBatch.DrawString(titleFont, title, new Vector2(630, 200), Color.White);
                    _spriteBatch.DrawString(titleFont, subTitle, new Vector2(645, 300), Color.White);
                    _spriteBatch.DrawString(pressEnterFont, pressEnterToBegin, new Vector2(650, 880), Color.White);
                    _spriteBatch.End();
                    break;
                #endregion
                //Intro Screen Drawing Functions
                case GameState.introScreen:
                    #region IntroScreen
                    _spriteBatch.Begin();
                    _spriteBatch.Draw(introBackground, new Vector2(0, 0), Color.White);
                    _spriteBatch.Draw(introTextBox, new Vector2((gameWindowWidth / 2 - introTextBox.Width / 2), (gameWindowHeight / 2 - introTextBox.Height / 2)), Color.White);
                    DrawIntroText();
                    _spriteBatch.DrawString(pressEnterFont, "Press [Enter] to Continue", new Vector2(600, 880), Color.Yellow);
                    _spriteBatch.End();
                    break;
                #endregion
                //Main Menu Screen Drawing Functions
                case GameState.mainMenu:
                    #region MainMenu
                    _spriteBatch.Begin();
                    _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                    //Region for the Main Menu Screen w/Buttons
                    #region Main Menu Screen
                    if (mainMenu == true)
                    {
                        _spriteBatch.Draw(buttonMenu, buttonPlayCoords, Color.White);
                        _spriteBatch.Draw(buttonMenu, buttonOptionsCoords, Color.White);
                        _spriteBatch.Draw(buttonMenu, buttonCreditsCoords, Color.White);
                        _spriteBatch.DrawString(introFont, "[Play]", new Vector2(875, buttonPlayCoords.Y + buttonMenu.Height / 4), Color.Yellow);
                        _spriteBatch.DrawString(introFont, "[Options]", new Vector2(850, buttonOptionsCoords.Y + buttonMenu.Height / 4), Color.Yellow);
                        _spriteBatch.DrawString(introFont, "[Credits]", new Vector2(850, buttonCreditsCoords.Y + buttonMenu.Height / 4), Color.Yellow);
                    }
                    #endregion
                    //Region for the Options Screen w/Buttons
                    #region Options Screen
                    if (options == true)
                    {
                        _spriteBatch.Draw(buttonMenu, buttonBackCoords, Color.White);
                        _spriteBatch.DrawString(introFont, "[Back]", new Vector2(875, buttonBackCoords.Y + buttonMenu.Height / 4), Color.Yellow);
                        _spriteBatch.Draw(buttonImages[buttonNumber1], buttonOptionToggle1Coords, new Rectangle(0, 0, buttonImages[buttonNumber1].Width, buttonImages[buttonNumber1].Height), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 1f);
                        _spriteBatch.Draw(buttonImages[buttonNumber2], buttonOptionToggle2Coords, new Rectangle(0, 0, buttonImages[buttonNumber2].Width, buttonImages[buttonNumber2].Height), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 1f);
                        _spriteBatch.Draw(buttonImages[buttonNumber3], buttonOptionToggle3Coords, new Rectangle(0, 0, buttonImages[buttonNumber3].Width, buttonImages[buttonNumber3].Height), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 1f);
                        _spriteBatch.Draw(buttonImages[buttonNumber4], buttonOptionToggle4Coords, new Rectangle(0, 0, buttonImages[buttonNumber4].Width, buttonImages[buttonNumber4].Height), Color.White, 0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 1f);
                        DrawOptionsMenu();
                    }
                    #endregion
                    //Region for the Credits Screen w/Buttons
                    #region Credits Screen
                    if (credits == true)
                    {
                        _spriteBatch.Draw(buttonMenu, buttonBackCoords, Color.White);
                        _spriteBatch.DrawString(introFont, "[Back]", new Vector2(875, buttonBackCoords.Y + buttonMenu.Height / 4), Color.Yellow);
                        DrawCreditsText();
                    }
                    #endregion
                    _spriteBatch.End();
                    break;
                #endregion
                //Image Select Screen Drawing Functions
                case GameState.imageSelect:
                    #region ImageSelect
                    _spriteBatch.Begin();
                    _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                    //Region for drawing the game tiles
                    #region TileDrawing
                    DrawGameTiles();
                    #endregion
                    //Region for drawing buttons for selecting, shuffling and checking if the tiles are in the proper place
                    #region Button Drawing
                    _spriteBatch.Draw(button1Red, button1RedCoords, Color.White);
                    _spriteBatch.Draw(button2Green, button2GreenCoords, Color.White);
                    _spriteBatch.Draw(button3Yellow, button3YellowCoords, Color.White);
                    _spriteBatch.Draw(button4Red, button4RedCoords, Color.White);
                    _spriteBatch.Draw(textBubble, new Vector2(1419, 115), Color.White);
                    #endregion
                    //Region for drawing different Text within the message box
                    #region Text Box Messages
                    //Region for Drawing the different sets of Controls Text
                    #region Controls Text
                    //Controls Text before Shuffling
                    if (displayControls == true && isShuffled == false)
                    {
                        DrawControlsText();
                    }
                    //Controls Text after Shuffling
                    else if (displayControls == true && isShuffled == true)
                    {
                        DrawControlsText2();
                    }
                    #endregion
                    //Region for Drawing Text that is display as "Error Messages" to be displayed when the player tries to do something the game doesn't allow
                    #region "Error Messages" Text
                    //Message to be displayed if the player hasn't solved the puzzle but clicks the CheckPosition button
                    if (tryAgain == true)
                    {
                        _spriteBatch.DrawString(textBoxFont, "Try Again", new Vector2(1600, 150), Color.Yellow);
                        _spriteBatch.DrawString(textBoxFont, "Tiles aren't in the correct position.", new Vector2(1428, 180), Color.Yellow);
                    }
                    //Message to be displayed if the player tries to change the image after shuffling the tiles
                    if (cantChange == true)
                    {
                        _spriteBatch.DrawString(textBoxFont, "Sorry you can't change the image", new Vector2(1436, 150), Color.Yellow);
                        _spriteBatch.DrawString(textBoxFont, "after you've shuffled the tiles.", new Vector2(1455, 180), Color.Yellow);
                    }
                    //Message to be displayed if the player tries to solve the puzzle before shuffling the tiles
                    if (originTilePos == true)
                    {
                        _spriteBatch.DrawString(textBoxFont, "You can't try to win", new Vector2(1540, 150), Color.Yellow);
                        _spriteBatch.DrawString(textBoxFont, "You haven't shuffled the tiles!", new Vector2(1455, 180), Color.Yellow);
                    }
                    //Message to be displayed if the player tries to shuffle the image a second time after shuffling
                    if (isShuffledMessage == true)
                    {
                        _spriteBatch.DrawString(textBoxFont, "Sorry you can't shuffle the image", new Vector2(1435, 150), Color.Yellow);
                        _spriteBatch.DrawString(textBoxFont, "again after you've shuffled it once", new Vector2(1430, 180), Color.Yellow);
                    }
                    #endregion
                    //Region for Drawing Text that is displayed if the player enables certian game options
                    #region Game Options Text
                    //Text that is displayed if the player enables the game timer
                    if (enableTimer == true)
                    {
                        _spriteBatch.DrawString(textBoxFont, "Time - " + minutes + ":" + seconds, new Vector2(1428, 360), Color.Yellow);
                    }
                    //Text that is displayed if the player enables the move counter
                    if (enableCounter == true)
                    {
                        _spriteBatch.DrawString(textBoxFont, "Moves: " + moves, new Vector2(1428, 330), Color.Yellow);
                    }
                    #endregion
                    #endregion
                    //Region for the Statement used for drawing numbers on each tile if the option is enabled
                    #region Tile Numbers
                    if (enableTileNumbers == true)
                    {
                        DrawTileNumbers();
                    }
                    #endregion
                    _spriteBatch.End();
                    break;
                #endregion
                //Pause Screen Drawing Functions
                case GameState.pauseScreen:
                    #region PauseScreen
                    _spriteBatch.Begin();
                    _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                    _spriteBatch.DrawString(titleFont, "Paused", new Vector2(780, 425), Color.Yellow);
                    _spriteBatch.Draw(buttonMenu, buttonBackCoords, Color.White);
                    _spriteBatch.DrawString(introFont, "[Back]", new Vector2(875, buttonBackCoords.Y + buttonMenu.Height / 4), Color.Yellow);
                    _spriteBatch.Draw(textBubble, new Vector2(1419, 115), Color.White);
                    _spriteBatch.End();
                    break;
                #endregion
                //Win Screen Drawing Functions
                case GameState.winScreen:
                    #region WinScreen
                    _spriteBatch.Begin();
                    _spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
                    PuzzleSolved();
                    //Region for the Win Screen w/Buttons
                    #region Win Screen w/Buttons
                    if (currentState == GameState.winScreen
                        && puzzleSolved == false
                        && displayWinButtons == true)
                    {
                        _spriteBatch.DrawString(titleFont, "You Win!", new Vector2(760, 425), Color.Yellow);
                        _spriteBatch.Draw(buttonMenu, buttonBackCoords, Color.White);
                        _spriteBatch.DrawString(introFont, "[Replay]", new Vector2(850, buttonBackCoords.Y + buttonMenu.Height / 4), Color.Yellow);
                        _spriteBatch.Draw(buttonMenu, buttonExitCoords, Color.White);
                        _spriteBatch.DrawString(introFont, "[Exit]", new Vector2(875, buttonExitCoords.Y + buttonMenu.Height / 4), Color.Yellow);
                        _spriteBatch.Draw(textBubble, new Vector2(1419, 115), Color.White);
                        _spriteBatch.DrawString(textBoxFont, "Congratulations, you solved the", new Vector2(1428, 120), Color.Yellow);
                        _spriteBatch.DrawString(textBoxFont, "puzzle, if you want you can play", new Vector2(1428, 150), Color.Yellow);
                        _spriteBatch.DrawString(textBoxFont, "again using [Replay].", new Vector2(1428, 180), Color.Yellow);
                    }
                    #endregion
                    _spriteBatch.End();
                    break;
                    #endregion
            }
            #endregion

            base.Draw(gameTime);
        }
    }
}
