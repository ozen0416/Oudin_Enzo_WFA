using Snake.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        //Initialisation de toutes les variables utiles pour notre jeu
        private List<Circle> Snake = new List<Circle>();

        private Circle food = new Circle();
   
        int maxWidth;
        int maxHeight;

        int score;
        int highScore;

        Random rand = new Random();

        bool goLeft, goRight, goUp, goDown;

        private DateTime lastkey = DateTime.Now;

        //Lancement du jeu, la ligne 38 permet que notre Combobox puisse commencer avec la difficulté "Facile" et non null
        public Form1()
        {

            InitializeComponent();
            this.difficultyGame.SelectedIndex = 0;
            new Settings();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        //Création des directions et ajout volontaire d'une input lag
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            DateTime curTime = DateTime.Now;
            TimeSpan intervalBetweenKeys = TimeSpan.FromMilliseconds(90);
            if (curTime - lastkey >= intervalBetweenKeys)
            {
                if (e.KeyCode == Keys.Left && Settings.directions != "right")
                {
                    goLeft = true;
                }
                if (e.KeyCode == Keys.Right && Settings.directions != "left")
                {
                    goRight = true;
                }
                if (e.KeyCode == Keys.Up && Settings.directions != "down")
                {
                    goUp = true;
                }
                if (e.KeyCode == Keys.Down && Settings.directions != "up")
                {
                    goDown = true;
                }
                lastkey = curTime;
            }
        }

        //Inverse de la fonction au-dessus
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }
        }

        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
            Difficulty();
        }

        //Déplacement grâce aux directions
        private void GameTimerEvent(object sender, EventArgs e)
        {
            if (goLeft)
            {
                Settings.directions = "left";
            }
            if (goRight)
            {
                Settings.directions = "right";
            }
            if (goDown)
            {
                Settings.directions = "down";
            }
            if (goUp)
            {
                Settings.directions = "up";
            }

            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Settings.directions)
                    {
                        case "left":
                            Snake[i].X--;
                            break;
                        case "right":
                            Snake[i].X++;
                            break;
                        case "up":
                            Snake[i].Y--;
                            break;
                        case "down":
                            Snake[i].Y++; 
                            break;

                    }
                    //Changement de côté si le snake dépasse l'écran (si il va à gauche il se téléportera à droite...)
                    if (Snake[i].X < 0)
                    {
                        Snake[i].X = maxWidth;
                    }
                    if (Snake[i].X > maxWidth)
                    {
                        Snake[i].X = 0;
                    }
                    if (Snake[i].Y < 0)
                    {
                        Snake[i].Y = maxHeight;
                    }
                    if (Snake[i].Y > maxHeight)
                    {
                        Snake[i].Y = 0;
                    }
                    //Si les coordonnées du snake sont les mêmes que celles de la nourriture, appelle la fonction
                    if (Snake[i].X == food.X && Snake[i].Y == food.Y)
                    {
                        EatFood();
                    }
                    //Si les coordonnées de la tête du snake sont les mêmes que celles de son corps, appelle la fonction
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            GameOver();
                        }
                    }

                }
                //Le corps du snake suit la tête
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                } 

            }

            picCanvas.Invalidate();

        }
        //Dessine le snake avec des cercles
        private void UpdatePictureBoxGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush snakeColour;

            for (int i = 0; i < Snake.Count; i++)
            {
                //Couleur de la tête
                if (i == 0)
                {
                    snakeColour = Brushes.Black;
                }
                //Couleur du corps
                else
                {
                    snakeColour = Brushes.White;
                }
                //Rempli le premier cercle avec sa couleur
                canvas.FillEllipse(snakeColour, new Rectangle
                    (
                    Snake[i].X * Settings.Width,
                    Snake[i].Y * Settings.Height,
                    Settings.Width, Settings.Height
                    ));
            }
            //Rempli le reste avec sa couleur
            canvas.FillEllipse(Brushes.Yellow, new Rectangle
                   (
                   food.X * Settings.Width,
                   food.Y * Settings.Height,
                   Settings.Width, Settings.Height
                   ));
        }

        private void RestartGame()
        {
            //Dimensions de la fenêtre
            maxWidth = picCanvas.Width / Settings.Width - 1;
            maxHeight = picCanvas.Height / Settings.Height - 1;

            Snake.Clear();
            //Le boutton "Play" et la difficulté sont indisponibles pendant le jeu
            startButton.Enabled = false;
            difficultyGame.Enabled = false;

            score = 0;
            txtScore.Text = "Score :" + score;

            //Crée le snake
            Circle head = new Circle
            {
                X = 10,
                Y = 5
            };
            Snake.Add(head);
            //Crée le corps
            for (int i = 0; i < 3; i++)
            {
                Circle body = new Circle();
                Snake.Add(body);
            }
            //Crée la première nourriture et la dispose aléatoirement
            food = new Circle {
                X = rand.Next(2, maxWidth),
                Y = rand.Next(2, maxHeight)
            };
            gameTimer.Start();
        }

        private void dropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void EatFood()
        {
            score += 1;
            txtScore.Text = "Score : " + score;

            //Ajoute de la longueur au corps du snake
            Circle body = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };

            Snake.Add(body);

            //Crée à l'infini aléatoirement de la nourriture
            food = new Circle
            {
                X = rand.Next(2, maxWidth),
                Y = rand.Next(2, maxHeight)
            };


        }

        private void GameOver()
        {
            //Les bouttons sont à nouveau disponibles et affichage du score + du score maximale (ce dernier est actualisé si score > score max)
            gameTimer.Stop();
            startButton.Enabled = true;
            difficultyGame.Enabled = true;
            string title = "Vous êtes morts";
            MessageBox.Show("Changez la difficulté ou quittez, ce jeu n'est pas fait pour vous",title);

            if (score > highScore)
            {
                highScore = score;

                txtHighScore.Text = "Highscore : " + Environment.NewLine + highScore;

                txtHighScore.ForeColor = Color.Red;
                txtHighScore.TextAlign = ContentAlignment.MiddleCenter;

            }
        }

        private void Difficulty()
        {
            //Change la vitesse du snake en fonction de la difficulté choisie
            if (difficultyGame.SelectedItem.ToString() == "Facile")
            {
                gameTimer.Interval = 50;
            }
            if (difficultyGame.SelectedItem.ToString() == "Normal")
            {
                gameTimer.Interval = 30;
            }
            if (difficultyGame.SelectedItem.ToString() == "Difficile")
            {
                gameTimer.Interval = 10;
            }
        }


    }
}
