using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Game_Of_Life
{
    public partial class Form1 : Form
    {
      
        // The universe array
        bool[,] universe = new bool[Convert.ToInt32(Properties.Settings.Default.GridHeight),Convert.ToInt32(Properties.Settings.Default.GridWidth)];
        bool[,] ScratchPad = new bool[Convert.ToInt32(Properties.Settings.Default.GridHeight), Convert.ToInt32(Properties.Settings.Default.GridWidth)];
      
        // universe = (bool[,]) ScratchPad.Clone();

        int neighbors = 0;
        bool viewNeighbors = true;
       

        // Drawing colors
        Color gridColor = Properties.Settings.Default.GridColor;
        Color cellColor = Properties.Settings.Default.CellColor;
        

        // The Timer class
        Timer timer = new Timer();

        Options op = new Options();

        Randomize random = new Randomize();
        int seed = Properties.Settings.Default.Seed;

        // Generation count
        int generations = 0;
        int alive = 0;
        int cellWidth = Convert.ToInt32(Properties.Settings.Default.GridWidth);
        int cellHeight = Convert.ToInt32(Properties.Settings.Default.GridHeight);


        public Form1()
        {
            InitializeComponent();
            this.Size = new System.Drawing.Size(648, 640);
            // Setup the timer
            timer.Interval = Properties.Settings.Default.Time; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
            // settings for the background
            graphicsPanel1.BackColor = Properties.Settings.Default.BackGroundColor;
            // settings for the grid
            gridColor = Properties.Settings.Default.GridColor;
            // settings for the cells
            cellColor = Properties.Settings.Default.CellColor;
            // HUD Color
            graphicsPanel2.BackColor = Properties.Settings.Default.HUDColor;
            graphicsPanel2.ForeColor = Properties.Settings.Default.HUDColor;

            labelUniverseWidth.Text = "Universe Width: " + universe.GetLength(0).ToString();
            labelUniverseHeight.Text = "Universe Height: " + universe.GetLength(1).ToString();
            
            


        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {
            // Update status strip to see time
            toolStripStatusLabel1.Text = "Time = " + timer.Interval.ToString();

            // Increment generation count
            generations++;

            // Update status strip generations

            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            labelGeneration.Text = "Generations: " + generations.ToString();

            // Displays height and width for the game
            labelUniverseWidth.Text = "Universe Width: " + universe.GetLength(0).ToString();
            labelUniverseHeight.Text = "Universe Height: " + universe.GetLength(1).ToString();





            // Rules

            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    neighbors = Neighbors(x, y);

                    

                    //if (universe[x, y] = false && neighbors == 3)
                    //    ScratchPad[x, y] = true;

                    //ScratchPad[x, y] = true;
                    if (universe[x, y] == true)
                    {
                        if (neighbors < 2)
                        {
                            ScratchPad[x, y] = false;
                            
                        }
                        else if (neighbors > 3)
                        {
                            ScratchPad[x, y] = false;
                           
                        }
                        else if (neighbors == 2 || neighbors == 3)
                        {
                            ScratchPad[x, y] = true;
                           

                        }

                    }

                    else if (universe[x, y] == false && neighbors == 3)
                    {
                        ScratchPad[x, y] = true;
                        
                    }


                }
            }
            

            universe = (bool[,])ScratchPad.Clone();
          


            graphicsPanel1.Invalidate();
   
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Convert to FLOATS
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
             cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            // Can also change the size of the grid
            Pen gridPen = new Pen(gridColor, 1);

            /// Making the thick barriers
            Pen boarder = new Pen(gridColor, 5);

            // A Brush for filling living cells interiors (color)
            // Turns cells on and off
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    //make lines for grid
                    if (x % 10 == 0 && y % 10 == 0)
                        e.Graphics.DrawRectangle(boarder, cellRect.X, cellRect.Y, cellRect.Width * 10, cellRect.Height * 10);

                    

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        
                            e.Graphics.FillRectangle(cellBrush, cellRect);
                      

                    }
                    if (ScratchPad[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);

                    }

                    // Displays neighbors around the cell
                    else if(viewNeighbors == true)
                    {
                        
                        Brush brush = Brushes.Black;
                        neighbors = Neighbors(x, y);
                        if (neighbors == 3)
                            brush = Brushes.Red;
                        StringFormat format = new StringFormat();
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;

                        Rectangle rectangle = new Rectangle(cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                        if (neighbors != 0)
                            e.Graphics.DrawString(neighbors.ToString(), Font, brush, rectangle, format);
                    }

                    
                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }
            Alive();

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }
        public int Neighbors(int cellX, int cellY)
        {
            int isAlive = 0;
          

            


            // checks y for boundries
            for (int y = cellY - 1; y <= cellY + 1; y++)
            {
                int yNeighbor = y;
                // makes sure y is not less than the boundry
                if (y < 0)
                {
                    yNeighbor = universe.GetLength(1) - 1;

                }
                // checks if y is greater than
                else if (y > universe.GetLength(1) - 1)
                {
                    yNeighbor = 0;

                }
               
                

                //checking x boundry
                for (int x = cellX - 1; x <= cellX + 1; x++)
                {
                    int xNeightbor = x;
                    if (x < 0)
                    {
                        xNeightbor = universe.GetLength(0) - 1;

                    }
                    

                    else if (x > universe.GetLength(0) - 1)
                    {
                        xNeightbor = 0;

                    }
                   


                    if (universe[xNeightbor, yNeighbor] == true && (x != cellX || y != cellY))
                    {
                        isAlive++;
                       
                        //return isAlive;
                    }


                   

                }
            }

            
            return isAlive;
        }

       
        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
               // isAlive = true;
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;


               

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }
        // Background Color Method
       private void BackGround()
        {
            ColorDialog back = new ColorDialog();

            back.Color = graphicsPanel1.BackColor;

            if (DialogResult.OK == back.ShowDialog())
            {
                graphicsPanel1.BackColor = back.Color;
               
            }
        }
        // Alive Count
        private int Alive()
        {
            alive = 0;
            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    if (universe[x, y] == true)
                        alive++;
                    
                }

            }
            toolStripStatusLabel2.Text = "Alive Cells = " + alive.ToString();

            return alive;
        }

        //Cell Color Method
        private void CellColor()
        {
            ColorDialog cell = new ColorDialog();

            cell.Color = cellColor;

            if (DialogResult.OK == cell.ShowDialog())
            {
                cellColor = cell.Color;
                graphicsPanel1.Invalidate();
               
            }
        }

        // Grid Color
        private void GridColor()
        {
            ColorDialog back = new ColorDialog();

            back.Color = gridColor;

            if (DialogResult.OK == back.ShowDialog())
            {
               gridColor = back.Color;
                graphicsPanel1.Invalidate();
               
            }
        }

        // New button on tool strip
        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                    ScratchPad[x, y] = false;
                    // resets the generation count to 0
                    generations = 0;
                    toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
                    //resets timer
                    //timer.Interval = 100;
                    //toolStripStatusLabel1.Text = "Time = " + timer.Interval.ToString();
                    labelGeneration.Text = "Generations = " + generations.ToString();
                    // Tells windows to repaint
                    graphicsPanel1.Invalidate();
                    

                }
                
            }
        }

        // New button on the file drop down
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    universe[x, y] = false;
                    // resets generations to 0
                    generations = 0;
                    toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
                    labelGeneration.Text = "Generations = " + generations.ToString();
                    // resets timer
                    //timer.Interval = 100;
                    //toolStripStatusLabel1.Text = "Time = " + timer.Interval.ToString();
                    // Tells windows to repaint
                    graphicsPanel1.Invalidate();
                    
                }
            }
        }

        // Random cell method for Time
        private void RandomChoice()
        {
            Random rand = new Random();

            //int time = Convert.ToInt32(op.Time);


            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    if (rand.Next(0, 3) == 0)
                        universe[x, y] = true;
                    else
                        universe[x, y] = false;
                }
            }

            graphicsPanel1.Invalidate();
        }

        // Save method
        private void Save()
        {

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.

                // Save the file comments
                writer.WriteLine("!Save File!");
                writer.WriteLine($"!Universe: {cellWidth} X {cellHeight}");
                writer.WriteLine($"!Generation: {generations}");
                writer.WriteLine(alive);


                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    StringBuilder currentRow = new StringBuilder();

                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (universe[x, y] == true)
                            currentRow.Append('O');

                        else if (universe[x, y] == false)
                            currentRow.Append('.');
                       
                       
                    }
                    writer.WriteLine(currentRow);
                }
                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        // Open File
        private void Open()
        {
           
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "All Files|*.*|Cells|*.cells";
            open.FilterIndex = 2;

            if (DialogResult.OK == open.ShowDialog())
            {
                StreamReader reader = new StreamReader(open.FileName);

                
                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();
                 
                            // If the row begins with '!' then it is a comment
                            // and should be ignored.
                    if(row[0] != '!')
                    {
                        maxHeight++;

                        if (row.Length > maxWidth)
                            maxWidth = row.Length;
                    }
                
                    
                  
                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.

                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                }

                // Resize the current universe and scratchPad
                universe = new bool[maxWidth, maxHeight];
                ScratchPad = new bool[maxWidth, maxHeight];
                cellWidth = maxWidth;
                cellHeight = maxHeight;
                // to the width and height of the file calculated above.

                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                int y = 0;
                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();
                    
                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row[0] != '!')
                    {
                        // If the row is not a comment then 
                        // it is a row of cells and needs to be iterated through.
                        for (int xPos = 0; xPos < row.Length; xPos++)
                        {
                            // If row[xPos] is a 'O' (capital O) then
                            // set the corresponding cell in the universe to alive.
                            if (row[xPos] == 'O')
                                universe[xPos, y] = true;
                            // If row[xPos] is a '.' (period) then
                            // set the corresponding cell in the universe to dead.
                            else if (row[xPos] == '.')
                                universe[xPos, y] = false;
                            
                        }
                        y++;
                    }
                  
                }

                // Close the file.
                reader.Close();
              
            }
            graphicsPanel1.Invalidate();
        }

        // Play button
        private void ToolStripPlay_Click(object sender, EventArgs e)
        {

          
            timer.Enabled = true;
            

        }

        // Stop button
        private void ToolStripStop_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        // Next button
        private void ToolStripNext_Click(object sender, EventArgs e)
        {
            NextGeneration();

         
        }

        // Play from the drop down
        private void PlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        // Stop from the drop down
        private void StopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }
        // Next from the dropdown
        private void NextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }
        // Exit
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Save();
            this.Close();
            
        }
        // Creat random neighbors
        private void RandomizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RandomChoice();

        }

        // Save the file
        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            Save();
        }
        // Open the file
        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            Open();
        }
        // Randomize from button
        private void RandomizeToolStripButton4_Click(object sender, EventArgs e)
        {
            RandomChoice();
        }

     
    
        // Toggle neighbor count
        private void NeighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            viewNeighbors = !viewNeighbors;
            graphicsPanel1.Invalidate();
        }

        // Turn the grid off
        private void OffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = Properties.Settings.Default.BackGroundColor;
            graphicsPanel1.Invalidate();
        }
      
     
        // Options to change time, height and width
        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            
            op.Time = timer.Interval;
            op.GridWidth = universe.GetLength(1);
            op.GridHeight = universe.GetLength(0);
           
            //  universe[Convert.ToInt32(op.GridHeight), Convert.ToInt32(op.GridWidth)] = true;


            if (DialogResult.OK == op.ShowDialog())
            {
                
                timer.Interval = Convert.ToInt32(op.Time);
                universe = new bool[Convert.ToInt32(op.GridHeight), Convert.ToInt32(op.GridWidth)];
                ScratchPad = new bool[Convert.ToInt32(op.GridHeight), Convert.ToInt32(op.GridWidth)];

                //  Height = Convert.ToInt32(op.GridHeight);
                //  Width = Convert.ToInt32(op.GridWidth);
                
               graphicsPanel1.Invalidate();

            }
            Properties.Settings.Default.GridHeight = op.GridHeight;
            Properties.Settings.Default.GridWidth = op.GridWidth;
            Properties.Settings.Default.Time = Convert.ToInt32(op.Time);
            Properties.Settings.Default.Save();

        }
        // Turns the grid on
        private void OnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridColor = Properties.Settings.Default.GridColor;
            graphicsPanel1.Invalidate();
        }

        // Turn HUD off
        private void OffToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            graphicsPanel2.Visible = false;
        }
        // Turn HUD on
        private void OnToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            graphicsPanel2.Visible = true;
        }

        // BackGround Color
        private void BackGroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackGround();
            Properties.Settings.Default.BackGroundColor = graphicsPanel1.BackColor;
           //Properties.Settings.Default.Save();
        }
        // Cell Color Change
        private void CellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CellColor();
            Properties.Settings.Default.CellColor = cellColor;
           //Properties.Settings.Default.Save();
        }
        // Grid Color Change
        private void GridToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GridColor();
            Properties.Settings.Default.GridColor = gridColor;
           // Properties.Settings.Default.Save();
        }
        // HUD off from right click
        private void OffToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            graphicsPanel2.Visible = false;
        }
        // HUD on from right click
        private void OnToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            graphicsPanel2.Visible = true;
        }

        // Restore Defaults
        private void ResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Properties.Settings.Default.Reset();
            graphicsPanel1.BackColor = Properties.Settings.Default.BackGroundColor;
            cellColor = Properties.Settings.Default.CellColor;
            gridColor = Properties.Settings.Default.GridColor;
            generations = 0;
           
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            labelGeneration.Text = "Generations: " + generations.ToString();
            timer.Interval = Properties.Settings.Default.Time;
            toolStripStatusLabel1.Text = "Time = " + timer.Interval.ToString();
            graphicsPanel2.Visible = true;
            universe = new bool[Convert.ToInt32(Properties.Settings.Default.GridHeight), Convert.ToInt32(Properties.Settings.Default.GridWidth)];
            ScratchPad = new bool[Convert.ToInt32(Properties.Settings.Default.GridHeight), Convert.ToInt32(Properties.Settings.Default.GridWidth)];
            labelUniverseWidth.Text = "Universe Width: " + universe.GetLength(0).ToString();
            labelUniverseHeight.Text = "Universe Height: " + universe.GetLength(1).ToString();
            graphicsPanel1.Invalidate();
        }
        // Neighbor COunt Toggle
        private void NeighborCountToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            viewNeighbors = !viewNeighbors;
            graphicsPanel1.Invalidate();
            
        }
        // Background from Settings
        private void BackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackGround();
            Properties.Settings.Default.BackGroundColor = graphicsPanel1.BackColor;
            
        }
        // CellColor from Settings
        private void CellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CellColor();
            Properties.Settings.Default.CellColor = cellColor;
           // Properties.Settings.Default.Save();
        }
        // Grid Color From Settings
        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            GridColor();
            Properties.Settings.Default.GridColor = gridColor;
          //  Properties.Settings.Default.Save();
        }
        // Reload the previous settings
        private void ReloadToolStripMenuItem_Click(object sender, EventArgs e) // doesn't work right
        {
            Properties.Settings.Default.Reload();

            graphicsPanel1.BackColor = Properties.Settings.Default.BackGroundColor;
            // settings for the grid
            gridColor = Properties.Settings.Default.GridColor;
            // settings for the cells
            cellColor = Properties.Settings.Default.CellColor;
            graphicsPanel1.Invalidate();
        }
        // New game without clearing the settings
        private void NewGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            universe= new bool[Convert.ToInt32(Properties.Settings.Default.GridHeight), Convert.ToInt32(Properties.Settings.Default.GridWidth)];
            ScratchPad = new bool[Convert.ToInt32(Properties.Settings.Default.GridHeight), Convert.ToInt32(Properties.Settings.Default.GridWidth)];

           
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            labelGeneration.Text = "Generations: " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        // Randomize from seed
        private void RandomizeBySeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            random.Seed = seed;
            Random rand = new Random(seed);
            generations = 0;
            

            if (DialogResult.OK == random.ShowDialog())
            {
               
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        if (rand.Next(0, 3) == 0)
                            universe[x, y] = true;
                        else
                            universe[x, y] = false;
                    }
                }

                seed = Convert.ToInt32(random.Seed);
                graphicsPanel1.Invalidate();
            }

           
        }
        // Saves things upon closing
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.BackGroundColor = graphicsPanel1.BackColor;
            Properties.Settings.Default.CellColor = cellColor;
            Properties.Settings.Default.GridColor = gridColor;
            Properties.Settings.Default.Save();

        }
        // Opens files
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
        }
    }
}
