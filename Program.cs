using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SFML;
using SFML.Window;
using SFML.Graphics;

namespace TicTacToe
{
    public class Pos {
        public int x = 0;
        public int y = 0;
        public int piece = -1;
    }

    class Program
    {

        //clicked or unclicked
        static string button = "";
        static int mousepos_x = 0;
        static int mousepos_y = 0;
        static int box_width = 100;
        static bool youwin = false;
        static bool catgame = false;
        static int box_height = 100;
        static bool gameover = false;
        static bool mouseState = false;

        //load board background
        static Image img_board = new Image("assets/board.png");
        static Texture txt_board = new Texture(img_board);
        static Sprite sprt_board = new Sprite(txt_board);
        
        //load x background
        static Image img_x = new Image("assets/x.png");
        static Texture txt_x = new Texture(img_x);
        static Sprite sprt_x = new Sprite(txt_x);

        //load o background
        static Image img_o = new Image("assets/o.png");
        static Texture txt_o = new Texture(img_o);
        static Sprite sprt_o = new Sprite(txt_o);

        //load win background
        static Image img_win = new Image("assets/win.png");
        static Texture txt_win = new Texture(img_win);
        static Sprite sprt_win = new Sprite(txt_win);

        //load lose background
        static Image img_lose = new Image("assets/lose.png");
        static Texture txt_lose = new Texture(img_lose);
        static Sprite sprt_lose = new Sprite(txt_lose);

        //load draw background
        static Image img_draw = new Image("assets/draw.png");
        static Texture txt_draw = new Texture(img_draw);
        static Sprite sprt_draw = new Sprite(txt_draw);

        static void Main(string[] args)
        {

            //player turn?
            bool myturn = false;

            Random random = new Random();
            int randomnumber = random.Next(0, 10);

            myturn = randomnumber > 5 ? true: false;

            //window size
            uint win_width = 300;
            uint win_height = 300;

            //game window
            RenderWindow window = new RenderWindow(new VideoMode(win_width, win_height), "Tic Tac Toe - G4MR", Styles.Titlebar | Styles.Close);

            //window events
            window.Closed += new EventHandler(EventClose);
            window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(MouseDown);
            window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(MouseUp);

            //setup empty 3x3 grid of the game board
            Pos[] board = new Pos[9];

            for(int x = 0; x < 9; x++) {
                board[x] = new Pos();
            }

            int count = 0;
            for(int x = 0; x < 3; x++) {
                for(int y = 0; y < 3; y++) {
                    board[count].x = x;
                    board[count].y = y;
                    count++;
                }
            }

            //game loop
            while(window.IsOpen()) {
                window.DispatchEvents();

                //clear window
                window.Clear();

                //check the winner
                CheckWinner(ref board);

                //check if the game is tied
                if(CatsGame(board)) {
                    gameover = true;
                    catgame = true;
                }

                //lets see if the game's over
                if(!gameover) {

                    //check if it's your turn
                    if(myturn) {

                        //check if the player clicked on the screen
                        if(mouseState && button == "Left") {
                            PlayerTurn(ref board);
                            mouseState = false;//disable holding the mouse
                            myturn = false;
                        }

                    } else {

                        //let the computer do his thang
                        myturn = true;
                        ComputerTurn(ref board);
                    }

                    //draw board
                    window.Draw(sprt_board);

                    //draw pieces on the board
                    for(int piece = 0; piece < board.Count(); piece++) {
                        float x = board[piece].x * box_width;
                        float y = board[piece].y * box_height;
                        draw_xo(board[piece].piece, x, y, ref window);
                    }
                } else {

                    //exit the game when they click the screen
                    if(mouseState)
                        window.Close();

                    //you win?
                    if(youwin && !catgame)
                        window.Draw(sprt_win);

                    //you lose?
                    if(!youwin && !catgame)
                        window.Draw(sprt_lose);

                    //cat game?
                    if(!youwin && catgame)
                        window.Draw(sprt_draw);
                }

                //update the screen 
                window.Display();
            }

        }

        //draw x or y
        static void draw_xo(int piece, float x, float y, ref RenderWindow win) {

            //lets make sure we're doing something useful
            if(piece > -1) {
                Sprite draw_piece = new Sprite();

                //computer piece "o"
                if(piece == 0) {
                    draw_piece = sprt_o;
                }

                //player piece "x"
                if(piece == 1) {
                    draw_piece = sprt_x;
                }

                //update position
                draw_piece.Position = new Vector2f(x, y);

                //draw to the screen
                win.Draw(draw_piece);
            }

        }

        static void CheckWinner(ref Pos[] board) {
            
            //who won
            int result = -1;

            //check board for player & computer to find a winner
            for(int winner = 0; winner < 2; winner++) {

                //horizontal rows
                if(board[0].piece == winner && board[1].piece == winner && board[2].piece == winner)
                    result = winner;
                if(board[3].piece == winner && board[4].piece == winner && board[5].piece == winner)
                    result = winner;
                if(board[6].piece == winner && board[7].piece == winner && board[8].piece == winner)
                    result = winner;

                //vertical rows
                if(board[0].piece == winner && board[3].piece == winner && board[6].piece == winner)
                    result = winner;
                if(board[1].piece == winner && board[4].piece == winner && board[7].piece == winner)
                    result = winner;
                if(board[2].piece == winner && board[5].piece == winner && board[8].piece == winner)
                    result = winner;

                //crosses
                if(board[0].piece == winner && board[4].piece == winner && board[8].piece == winner)
                    result = winner;
                if(board[2].piece == winner && board[4].piece == winner && board[6].piece == winner)
                    result = winner;

            }

            //did player win? end game
            if(result == 1) {
                youwin = true;
                gameover = true;
            }

            //did computer win? end game
            if(result == 0) {
                youwin = false;
                gameover = true;
            }
        }

        //computer random ai choosing a square
        static void ComputerTurn(ref Pos[] board) {

            //max available spots
            ArrayList available = new ArrayList();

            for(int x = 0; x < board.Count(); x++) {
                if(board[x].piece == -1) {
                    available.Add(x);
                }
            }
            
            //make sure there are available slots to take
            if(available.Count > 0) {

                //get a random number based on the available slots
                Random random = new Random();
                int randomnumber = random.Next(0, available.Count);

                //assign x to a random position
                board[(int)available[randomnumber]].piece = 0;
            }

        }

        //allow player to check a spot on the board if available
        static void PlayerTurn(ref Pos[] board) {
            
            //loop through grid
            for(int piece = 0; piece < board.Count(); piece++) {
                
                //piece position on board
                int x = board[piece].x * box_height;
                int y = board[piece].y * box_height;

                //check mouse position
                if(mousepos_x > x - 1 && mousepos_x < x + box_width + 1
                    && mousepos_y > y - 1 && mousepos_y < y + box_height + 1) {
                    board[piece].piece = 1;
                }
            }
        }

        //check if boards filled, cat game
        static bool CatsGame(Pos[] board) {

            //check winner
            CheckWinner(ref board);
            
            if(gameover)
                return false;

            //start count
            int count = 0;

            //loop through grid to see if its filled
            foreach(var piece in board) {
                if(piece.piece != -1) count++;
            }

            return count == 9 ? true : false;
        }
        
        //close window
        static void EventClose(object s, EventArgs e) {
            RenderWindow win = (RenderWindow) s;
            win.Close();
        }

        //mouse state click
        static void MouseDown(object s, MouseButtonEventArgs e) {
            mouseState = true;
            mousepos_x = e.X;
            mousepos_y = e.Y;
            button = (string) e.Button.ToString();
        }

        //mouse state click
        static void MouseUp(object s, MouseButtonEventArgs e) {
            mouseState = false;
        }
    }
}
