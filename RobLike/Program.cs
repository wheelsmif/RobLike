using System;

namespace robLike
{

    /*
        Program:
        This class is in charge of running the program. main contains the game loop.
    */
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(15, 10);  // Set size of console window. We make it small because this game is pretty minimal.
            Console.Title = "rL";


            /* Create board and initialize it */
            Board board = new Board();  // Create blank board
            board.init();       // Initialize board elements
            board.generate();   // Generate dungeon level 0

            /* Create the player */
            Tile player = new Tile('@', new Vector2D(1, 1), Tile.TileTypes.P);    //The player
            board.moveTile(player, player.getPos());

            bool exit = false;  // The exit condition
            /* * * * * * * * *
            *    GAME LOOP   *
            * * * * * * * * */
            while (!exit)
            {
                Console.Clear();    // Clear screen
                board.render(player);     // Render changed board

                Console.Write("[ARROW KEYS\nTO MOVE]\n['x' to EXIT]");

                ConsoleKey keyPressed = Console.ReadKey().Key;

                switch (keyPressed)
                {
                    // UP
                    case (ConsoleKey.UpArrow):
                        board.moveTile(player, new Vector2D(player.getPos().x - 1, player.getPos().y));
                        break;

                    case (ConsoleKey.W):
                        board.moveTile(player, new Vector2D(player.getPos().x - 1, player.getPos().y));
                        break;

                    // DOWN
                    case (ConsoleKey.DownArrow):
                        board.moveTile(player, new Vector2D(player.getPos().x + 1, player.getPos().y));
                        break;

                    case (ConsoleKey.S):
                        board.moveTile(player, new Vector2D(player.getPos().x + 1, player.getPos().y));
                        break;

                    // LEFT
                    case (ConsoleKey.LeftArrow):
                        board.moveTile(player, new Vector2D(player.getPos().x, player.getPos().y - 1));
                        break;

                    case (ConsoleKey.A):
                        board.moveTile(player, new Vector2D(player.getPos().x, player.getPos().y - 1));
                        break;

                    // RIGHT
                    case (ConsoleKey.RightArrow):
                        board.moveTile(player, new Vector2D(player.getPos().x, player.getPos().y + 1));
                        break;

                    case (ConsoleKey.D):
                        board.moveTile(player, new Vector2D(player.getPos().x, player.getPos().y + 1));
                        break;

                    // EXIT THE GAME
                    case (ConsoleKey.X):
                        Environment.Exit(0);
                        break;

                    default:
                        break;
                }




            }
            // - - END GAME LOOP - -
        }


    }
    //-/----------------------------------------------------------------

    /*
        Board:
        This class represents the game board, which is our case a zank dungorb.
        It creates the board, and modifies it for different levels. It manages everything in those levels as well as coordinating player movement.
        render() shows the board to the console, and is called routinely at the beginning of the game loop to show action that has happened.

    */
    class Board
    {
        /* STATIC VARIABLES -- These are where we put values we may mess with, so we only modify them in one spot instead of multiple */
        static int STARTINGBRDSIZE = 10;    // Can be set arbitrarily. I like this as a starting size. Levels are square because I'm lazy.
        static int VIEWSIZE = 5;    //Size of the viewable dimension. We will display as screen of this size to the player. Also square, see above.

        static int WALLCHANCE = 10; // Chance that wall will be generated instead of floor -- see generate() function below.

        static Tile GROUNDTILE = new Tile('.', new Vector2D(0, 0), Tile.TileTypes.G);    //Refer to the Tile class's FIRST constructor
        static Tile WALL = new Tile('#', new Vector2D(0, 0), Tile.TileTypes.W);         // ^    ^   ^   ^

        /* Variables */
        int boardsize;      //Size of the board. It's always square right now because I'm lazy. Easily MODDABLE
        Vector2D viewTopLeft;   // The location of the top-left of the board. Used in render()

        Tile[,] tileBoard;  //A 2-dimensional array of tiles. We will use this to hold our board(s).

        /*
            init():
            This function intializes the important variables of the Board class.

        */
        public void init()
        {
            boardsize = STARTINGBRDSIZE;    // Set starting board size
            tileBoard = new Tile[boardsize, boardsize];     // Initialize board to empty dimensions of tiles.
            viewTopLeft = new Vector2D(0, 0);    // Initialize the view vector.
        }

        /*
            generate():
            This function generates the current level.
        */
        public void generate()
        {
            Random rand = new Random();

            // iterate through first dimension
            for (int i = 0; i < boardsize; i++)
            {
                // iterate through second dimension
                for (int j = 0; j < boardsize; j++)
                {

                    if ((i > 0) && (i < boardsize - 1) && (j > 0) && (j < boardsize - 1))
                    {     // If its not a board wall, it CAN be ground
                        if (rand.Next(0, 99) > WALLCHANCE)  //Is it ground?
                        {   //YES IT IS GROUND
                            tileBoard[i, j] = new Tile(GROUNDTILE);
                            tileBoard[i, j].setPos(i, j);
                        }
                        else
                        {   //NO IT IS WALL
                            tileBoard[i, j] = new Tile(WALL);
                            tileBoard[i, j].setPos(i, j);
                        }
                    }
                    else    // It IS a board wall!
                    {
                        tileBoard[i, j] = new Tile(WALL);
                        tileBoard[i, j].setPos(i, j);
                    }

                }
            }

        }
        //- - - -

        /*
            render():
            This function renders the VIEWSIZE-d area around the given Tile in the Console.
        */
        public void render(Tile player)
        {
            // Check for new valid viewTopLeft.
            if (player.getPos().x - (VIEWSIZE / 2) >= 0 && player.getPos().x + (VIEWSIZE / 2) < boardsize)
            {
                viewTopLeft.x = (player.getPos().x) - (VIEWSIZE / 2);
            }
            if (player.getPos().y - (VIEWSIZE / 2) >= 0 && player.getPos().y + (VIEWSIZE / 2) < boardsize)
            {
                viewTopLeft.y = player.getPos().y - (VIEWSIZE / 2);
            }

            // Render the VIEWSIZE x VIEWSIZE box in the console

            for (int i = viewTopLeft.x; i < viewTopLeft.x + VIEWSIZE; i++)
            {
                for (int j = viewTopLeft.y; j < viewTopLeft.y + VIEWSIZE; j++)
                {

                    if (tileBoard[i, j].getType() == Tile.TileTypes.P)  //PLAYER
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(tileBoard[i, j].getImg());
                    }
                    else if (tileBoard[i, j].getType() == Tile.TileTypes.G) //GROUND
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(tileBoard[i, j].getImg());

                    }
                    else if (tileBoard[i, j].getType() == Tile.TileTypes.W) //WALL
                    {
                        Console.Write(tileBoard[i, j].getImg());
                    }
                    Console.ResetColor();

                    Console.Write(" ");
                }
                // Show HP on one-above middle line
                if (i == viewTopLeft.x + ((VIEWSIZE / 2) - 1))
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" H01");
                }
                // Show level on the middle line
                else if (i == viewTopLeft.x + (VIEWSIZE / 2))
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(" L01");
                }
                // Show ______ on one below middle line
                else if (i == viewTopLeft.x + ((VIEWSIZE / 2) + 1))
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write(" ???");
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Write("    ");
                }
                Console.Write("\n");
                Console.ResetColor();
            }
            Console.WriteLine("______________");
        }
        //- - - - 

        /*
            moveTile(Tile tIn, Vector2D toPos):
            Checks if the tile can move to the given position. If not, nothing happens.
        */
        public void moveTile(Tile tIn, Vector2D toPos)
        {
            if (checkTile(toPos.x, toPos.y))
            {
                tileBoard[tIn.getPos().x, tIn.getPos().y] = new Tile(GROUNDTILE);
                tIn.setPos(toPos);
                tileBoard[toPos.x, toPos.y] = tIn;
            }
        }


        bool checkTile(int a, int b)
        {
            bool valid = true;
            if (a <= 0 || b <= 0 || a > boardsize - 2 || b > boardsize - 2 || tileBoard[a, b].getType() == Tile.TileTypes.W) valid = false;     //Conditions for FALSE
            return valid;
        }

    }
    //-/----------------------------------------------------------------

    /*
        Tile:
        This class represents a board tile. Tiles come in four flavors:
                - Player (P)
                - Ground (G)
                - Wall (W)
                - Other (O)
         They also have an x and y position on the board, repesented in a Vector2D.

        They also have a symbol, such as '@', as their respective representations.
        The player can interact/fight Other items while walking on Ground but not through Walls. Easy.

    */
    class Tile
    {
        /* TileTypes -- These are represented in an enum. They serve as high-level tile identifiers. */
        public enum TileTypes
        {
            P,  //Player
            G,  //Ground
            W,  //Wall
            O   //Other (enemy/object)
        }


        char img;           // The char symbol, such as '@' and '#'
        Vector2D pos;       // 
        TileTypes type;


        /*
            Constructor 1:
            This creates a tile given a symbol, position, and high-level classification
        */
        public Tile(char _img, Vector2D _pos, TileTypes _type)
        {
            img = _img;
            pos = _pos;
            type = _type;
        }

        /*
            Copy Constructor:
            This takes a Tile and creates a copy of that Tile.
        */
        public Tile(Tile _t)
        {
            img = _t.img;
            pos = _t.pos;
            type = _t.type;
        }

        public char getImg()
        {
            return img;
        }

        public Vector2D getPos()
        {
            return pos;
        }
        public void setPos(int _x, int _y)
        {
            pos.x = _x;
            pos.y = _y;
        }
        public void setPos(Vector2D _in)
        {
            pos = _in;
        }

        public TileTypes getType()
        {
            return type;
        }



    }
    //-/----------------------------------------------------------------

    class Vector2D
    {
        public int x, y;

        /*
            Constructor:
            This takes an int element for x and y.
        */
        public Vector2D(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
        /*
            Copy Constructor:
            This takes a Vector2D and creates a copy of that Vector2D.
        */
        public Vector2D(Vector2D _v)
        {
            x = _v.x;
            y = _v.y;
        }
    }
    //-/----------------------------------------------------------------
}
