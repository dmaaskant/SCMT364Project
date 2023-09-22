using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCMT364Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Global Variables
        Dictionary<string, Task> graph = new Dictionary<string, Task>();
        bool speed = false;
        bool sortChoice = false;
        double cycle_time = 0;
        double prodTime = 0;
        double unitDemand = 0;
        List<Task> WS_tasks = new List<Task>();
        List<double> WS_times = new List<double>();
        List<double> Rem_idletimes = new List<double>();
        int WScounter = 0; // Needed for eficiency calculation?

        public MainWindow()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Clear the visual textboxes in the designer
        /// </summary>
        private void clearAll()
        {
            clearData();
            clearDisplay();
            recolorTextboxes();

        }
        /// <summary>
        /// Preset 1 Data
        /// </summary>
        private void displayPreset1()
        {
            txtUnitDemand.Text = "3000";
            txtProdTime.Text = "25";
            lbxSortChoice.SelectedIndex = 0;
            lbxSpeed.SelectedIndex = 0;
            cbxUnit.SelectedIndex = 1;

            tn1.Text = "1";
            tn2.Text = "2";
            tn3.Text = "3";
            tn4.Text = "4";
            tn5.Text = "5";
            tn6.Text = "6";
            tn7.Text = "7";
            tn8.Text = "8";
            tn9.Text = "9";
            tn10.Text = "10";
            tn11.Text = "11";
            tn12.Text = "12";
            tn13.Text = "13";

            tt1.Text = "0.1";
            tt2.Text = "0.1";
            tt3.Text = "0.1";
            tt4.Text = "0.2";
            tt5.Text = "0.1";
            tt6.Text = "0.2";
            tt7.Text = "0.1";
            tt8.Text = "0.15";
            tt9.Text = "0.3";
            tt10.Text = "0.05";
            tt11.Text = "0.2";
            tt12.Text = "0.2";
            tt13.Text = "0.1";

            p1.Text = String.Empty;
            p2.Text = "1";
            p3.Text = "2";
            p4.Text = "2";
            p5.Text = "2";
            p6.Text = "3 4 5";
            p7.Text = "1";
            p8.Text = "7";
            p9.Text = "8";
            p10.Text = "9";
            p11.Text = "6";
            p12.Text = "10 11";
            p13.Text = "12";
        }

        /// <summary>
        /// Preset 2 Data
        /// </summary>
        private void displayPreset2()
        {
            txtUnitDemand.Text = "180";
            txtProdTime.Text = "8";
            lbxSortChoice.SelectedIndex = 0;
            lbxSpeed.SelectedIndex = 0;

            tn1.Text = "A";
            tn2.Text = "B";
            tn3.Text = "C";
            tn4.Text = "D";
            tn5.Text = "E";
            tn6.Text = "F";
            tn7.Text = "G";
            tn8.Text = "H";

            tt1.Text = "80";
            tt2.Text = "60";
            tt3.Text = "80";
            tt4.Text = "70";
            tt5.Text = "50";
            tt6.Text = "50";
            tt7.Text = "80";
            tt8.Text = "70";

            p1.Text = String.Empty;
            p2.Text = "A";
            p3.Text = "A";
            p4.Text = "A";
            p5.Text = "B C";
            p6.Text = "C D";
            p7.Text = "E F";
            p8.Text = "G";
        }
        /// <summary>
        /// Preset Tutorial Data, also called by btnTutorial
        /// </summary>
        private void displayPresetTutorial()
        {
            txtUnitDemand.Text = "400";
            txtProdTime.Text = "8";
            lbxSortChoice.SelectedIndex = 0;
            lbxSpeed.SelectedIndex = 1;

            tn1.Text = "A";
            tn2.Text = "B";
            tn3.Text = "C";
            tn4.Text = "D";
            tn5.Text = "E";
            tn6.Text = "F";
            tn7.Text = "G";
            tn8.Text = "H";

            tt1.Text = "30";
            tt2.Text = "35";
            tt3.Text = "30";
            tt4.Text = "35";
            tt5.Text = "15";
            tt6.Text = "65";
            tt7.Text = "40";
            tt8.Text = "25";

            p1.Text = String.Empty;
            p2.Text = "A";
            p3.Text = "A";
            p4.Text = "B";
            p5.Text = "C";
            p6.Text = "C";
            p7.Text = "E F";
            p8.Text = "D G";
        }
        /// <summary>
        /// Calls preset function and puts the data in the Textboxes (visual)
        /// </summary>
        private void displayPreset(int preset_num)
        {
            switch (preset_num) {
                case 0:
                    displayPreset1();
                    break;
                case 1:
                    displayPreset2();
                    break;
                case 2:
                    displayPresetTutorial();
                    break;
                default:
                    MessageBox.Show("Preset Not recognized");
                    break;

            }
        }

        /// <summary>
        /// Clicking this button will start the distribution algorithm and give you output by taking in input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                recolorTextboxes();
                if (!isAllValidInput()) return;
                //MessageBox.Show("All input was validated");
                distributeTasks();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong when distributing the tasks. " + ex.Message + " application closing now", "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                recolorTextboxes();
                return;
            }
        }
        /// <summary>
        /// Validates all input, checks for presence, lenght, characters etc.
        /// </summary>
        /// <returns>True if all Input validated, false if something wasn't</returns>
        private bool isAllValidInput()
        {
            Validator v = new Validator();
            if (!v.isValidRow(tn1, tt1, p1)) return false;
            if (!v.isValidRow(tn2, tt2, p2)) return false;
            if (!v.isValidRow(tn3, tt3, p3)) return false;
            if (!v.isValidRow(tn4, tt4, p4)) return false;
            if (!v.isValidRow(tn5, tt5, p5)) return false;
            if (!v.isValidRow(tn6, tt6, p6)) return false;
            if (!v.isValidRow(tn7, tt7, p7)) return false;
            if (!v.isValidRow(tn8, tt8, p8)) return false;
            if (!v.isValidRow(tn9, tt9, p9)) return false;
            if (!v.isValidRow(tn10, tt10, p10)) return false;
            if (!v.isValidRow(tn11, tt11, p11)) return false;
            if (!v.isValidRow(tn12, tt12, p12)) return false;
            if (!v.isValidRow(tn13, tt13, p13)) return false;
            if (!v.isValidRow(tn14, tt14, p14)) return false;
            if (!v.isValidRow(tn14, tt14, p14)) return false;
            if (!v.isValidRow(tn15, tt15, p15)) return false;

            if (!v.isValidInputCell(txtProdTime)) return false;
            if (!v.isValidInputCell(txtUnitDemand)) return false;

            return true;
        }

        /// <summary>
        /// A or D buckets contain information about task name and time, result put in txtAllTasks
        /// </summary>
        /// <param name="bucket"> Task Bucket </param>
        /// <returns></returns>
        private string displayBucket(Dictionary<string, Task> bucket)
        {
            string bucketString = "";
            foreach (var task in bucket)
            {
                bucketString += $"{task.Value.Name}({task.Value.Time})";
            }
            return bucketString;
        }
        /// <summary>
        /// Creates a string of tasks in the task list
        /// ex. A B D E
        /// </summary>
        /// <param name="bucket"></param>
        /// <returns></returns>
        private string displayBucket(List<Task> bucket)
        {
            //Thread.Sleep(speed);
            string bucketString = "";
            foreach (var task in bucket)
            {
                bucketString += $"{task.Name}({task.Time}) ";
            }
            return bucketString;
        }
        /// <summary>
        /// Creates a string of tasks in the D_bucket
        /// ex. A B D E
        /// </summary>
        /// <param name="bucket"></param>
        /// <returns>long string of tasks </returns>f
        private string displayBucket(List<string> bucket)
        {
            //Thread.Sleep(speed);
            string bucketString = "";
            foreach (var task in bucket)
            {
                bucketString += $"{task}({graph[task].Time}) ";
            }
            return bucketString;
        }
        /// <summary>
        /// Load the speed into variable from lbxSpeed control
        /// </summary>
        private void determineSpeed()
        {
            /** 
             * 0) Instant -> false
             * 1) Step-By-Step -> true
             */
            switch (lbxSpeed.SelectedIndex)
            {
                case 0:
                    speed = false;
                    break;
                case 1:
                    speed = true;
                    break;
                default:
                    speed = false;
                    break;
            }
        }
        /// <summary>
        /// Clear all output textboxes
        /// </summary>
        private void clearNonDataFields()
        {
            txtCT.Text = "";
            txtAllTasks.Text = "";
            txtABucket.Text = "";
            txtDBucket.Text = "";
            txtEff.Text = "";
            txtIdleTime.Text = "";

            txtWS1.Text = "";
            txtWS2.Text = "";
            txtWS3.Text = "";
            txtWS4.Text = "";
            txtWS5.Text = "";
            txtWS6.Text = "";
            txtWS7.Text = "";
        }
        /// <summary>
        /// Assumes graph and class vars are already correctly assigned, starts distributing tasks
        /// </summary>
        private void distributeTasks()
        {
            clearNonDataFields();
            clearClassVars();

            loadInfo();
            double RT = cycle_time;

            txtAllTasks.Text = displayBucket(graph);

            List<Task> A_bucket = new List<Task>();
            List<string> D_bucket = new List<string>();

            // First do the special case of the first Task.
            // Add all task with no parents to Available bucket since that is the start
            foreach (var task in graph)
            {
                if (task.Value.Parents.Count() == 0)
                {
                    A_bucket.Add(task.Value);
                }
            }
            if (A_bucket.Count() == 0)
            {
                MessageBox.Show("No task has 0 parents so can't start");
                return;
            }
            sortBucket(A_bucket, sortChoice);
            txtABucket.Text = displayBucket(A_bucket);
            if (speed) if (MessageBox.Show($"Added all tasks with 0 parents {displayBucket(A_bucket)} to Available Tasks Bucket", "Step-By-Step Analysis", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    speed = false;
                }

            // Assign top of A-Bucket to WS distro
            Task top = A_bucket[0];

            // visually add task values to WS distro
            if (top.Time > cycle_time)
            {
                MessageBox.Show("Task time: " + top.Time + " was bigger than cycle time: " + cycle_time + ". Return now");
                return;
            }
            WS_tasks.Add(top);
            WS_times.Add(top.Time);
            RT -= top.Time;

            // Pop top into D-Bucket / Remove top from A_bucket
            D_bucket.Add(top.Name);
            txtDBucket.Text = displayBucket(D_bucket);
            A_bucket.Remove(top);
            txtABucket.Text = displayBucket(A_bucket);
            if (speed) if (MessageBox.Show($"Removed the best possible first option from A_bucket {top.Name}({top.Time}) and put it in the Added Bucket", "Step-By-Step Analysis", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    speed = false;
                }


            // Add all followers of top to A_bucket
            foreach (var strFollower in top.Followers)
            {
                A_bucket.Add(graph[strFollower]);
            }
            txtABucket.Text = displayBucket(A_bucket);
            if (speed) if (MessageBox.Show($"Added {top.Name}({top.Time}) to Workstation Distribution, added its followers {displayBucket(top.Followers)} to Available Tasks Bucket", "Step-By-Step Analysis", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                {
                    speed = false;
                }


            // Second, go into the main phase of adding to the Workstation distro
            int currWSNum = 1;
            while (A_bucket.Count() != 0)
            {
                sortBucket(A_bucket, sortChoice);
                string debug2 = "Sorted A_bucket: ";
                foreach (var sortedTask in A_bucket)
                {
                    debug2 += $"{sortedTask.Name} ({sortedTask.Time}-{sortedTask.Followers.Count})";
                }

                int j = 0;
                // Go through the sorted list of tasks in A_bucket and keep adding until the RT is smaller than any of the A_bucket element times
                while (j < A_bucket.Count())
                {
                    sortBucket(A_bucket, sortChoice);

                    // A high score task can be assigned to current workstation, we can add it and move to the next task
                    top = A_bucket[j];
                    if (top.Time > cycle_time)
                    {
                        MessageBox.Show("Task time: " + top.Time + " was bigger than cycle time: " + cycle_time + ". Return now");
                        return;
                    }
                    if (RT - A_bucket[j].Time >= 0)
                    {
                        // check if all parents are present, if not, move to next possible element in A_bucket
                        if (allParentsPresent(top, D_bucket)) // we can add the top task to distro
                        {
                            // Assign to WS
                            RT -= top.Time;
                            WS_tasks.Add(top);
                            WS_times.Add(top.Time);
                            // Add all followers of top to A_bucket
                            foreach (var strFollower in top.Followers)
                            {
                                // We don't want to add duplicates
                                if (!A_bucket.Contains(graph[strFollower])) A_bucket.Add(graph[strFollower]);
                            }
                            // Put top in D-Bucket
                            D_bucket.Add(top.Name);
                            // Pop top 
                            A_bucket.Remove(top);
                            txtDBucket.Text = displayBucket(D_bucket);
                            txtABucket.Text = displayBucket(A_bucket);
                            if (speed) if (MessageBox.Show($"Added {top.Name}({top.Time}) to Workstation and added its followers, {displayBucket(top.Followers)}, to Available Tasks Bucket", "Step-By-Step Analysis", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                                {
                                    speed = false;
                                }
                            j = 0;
                            continue; // we break because we don't need to keep going with the current loop.
                                      // we found the best task we wanted to add and now we start at 0 again.

                        }
                        else // Not all parents are present, we move to the next best option which could be one element away
                        {
                            if (speed) if (MessageBox.Show($"Even though {top.Name}({top.Time}) could be the best next option, not all parents are present so we find the next best option", "Step-By-Step Analysis", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                                {
                                    speed = false;
                                }
                            j++;
                            continue;
                        }
                    }
                    else // Best option is too long for remaining time. Choose next best option.
                    {
                        // We also increment and try with the next one.
                        if (speed) if (MessageBox.Show($"Even though {top.Name}({top.Time}) could be the best next option, their Task Time is bigger than the remaining Idle Time ({RT}) so we find the next best option", "Step-By-Step Analysis", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                            {
                                speed = false;
                            }
                        j++;
                        continue;
                    }
                }
                // We went through the entire sorted unchanged list of task and we found no tasks that were short enough.
                // There is nothing to do here anymore so we close the workstation and start with the full cycle time as RIT
                txtABucket.Text = displayBucket(A_bucket);
                txtDBucket.Text = displayBucket(D_bucket);

                // Workstation was closed, clear all imporant info and add to record
                displayWorkstationInfo(currWSNum, RT);
                RT = Math.Round(RT, 3);
                Rem_idletimes.Add(RT);
                if (speed) if (MessageBox.Show($"Closed Workstation {currWSNum}", "Step-By-Step Analysis", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    {
                        speed = false;
                    }
                WScounter++;

                WS_tasks.Clear();
                WS_times.Clear();
                RT = cycle_time;
                currWSNum++;
            }

            // Calculate Effiency and Total Idle time
            double total = 0;
            foreach (var RIT in Rem_idletimes)
            {
                total += RIT;
            }
            total = Math.Round(total, 2);
            txtIdleTime.Text = total.ToString();
            // Calculate total task time
            double totalTaskTime = 0;
            foreach (var Task in graph)
            {
                totalTaskTime += Task.Value.Time;
            }

            // Calc efficiency
            double eff = totalTaskTime / (cycle_time * WScounter);
            eff *= 100;
            eff = Math.Round(eff, 2);
            totalTaskTime = Math.Round(totalTaskTime, 2);
            txtEff.Text = eff.ToString();
            txtTotalTaskTime.Text = totalTaskTime.ToString();
        }

        /// <summary>
        /// Clear all class level variables
        /// </summary>
        private void clearClassVars()
        {
            graph.Clear();
            speed = false;
            cycle_time = 0;
            sortChoice = false;
            WS_tasks.Clear();
            WS_times.Clear();
            Rem_idletimes.Clear();
            WScounter = 0;
        }

        /// <summary>
        /// Outputs the information of each workstation to the appropriate workstation
        /// </summary>
        /// <param name="currWSNum"></param>
        /// <param name="RT"></param>
        private void displayWorkstationInfo(int currWSNum, double RT)
        {
            string result;
            switch (currWSNum)
            {
                case 1:
                    result = displayWS_string(currWSNum, RT);
                    txtWS1.Text = result;
                    break;
                case 2:
                    result = displayWS_string(currWSNum, RT);
                    txtWS2.Text = result;
                    break;
                case 3:
                    result = displayWS_string(currWSNum, RT);
                    txtWS3.Text = result;
                    break;
                case 4:
                    result = displayWS_string(currWSNum, RT);
                    txtWS4.Text = result;
                    break;
                case 5:
                    result = displayWS_string(currWSNum, RT);
                    txtWS5.Text = result;
                    break;
                case 6:
                    result = displayWS_string(currWSNum, RT);
                    txtWS6.Text = result;
                    break;
                case 7:
                    result = displayWS_string(currWSNum, RT);
                    txtWS7.Text = result;
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// Is called when you're displaying workstation information when workstation is closed
        /// </summary>
        /// <param name="WSnum"> Changes based on current Workstation number (can't get higher than 7)</param>
        /// <param name="RIT"> Remainding Idle Time </param>
        /// <returns> string that gets put in the workstation textbox </returns>
        private string displayWS_string(int WSnum, double RIT)
        {
            // 1.   A C | 15 6 | 27 - 15 - 6 = 6
            // 2.   A -> C | 15 -> 6 | 27 - 15 - 6 = 6
            string result = "";
            // Add WSNum
            result += WSnum + ". ";

            // Add WSTasks
            for (int i = 0; i < WS_tasks.Count() - 1; i++)
            {
                result += WS_tasks[i].Name + " -> ";
            }
            result += WS_tasks[WS_tasks.Count() - 1].Name + " | ";

            // Add WSTimes
            for (int i = 0; i < WS_times.Count() - 1; i++)
            {
                result += WS_times[i] + " -> ";
            }
            result += WS_times[WS_times.Count() - 1] + " | ";

            // Add RIT movement
            result += cycle_time + " ";
            foreach (var time in WS_times)
            {
                result += "- " + time + " ";
            }
            RIT = Math.Round(RIT, 3);
            result += "= " + RIT;
            return result;
        }

        /// <summary>
        /// Check if all parents are present of a task in the Added bucket
        /// </summary>
        /// <param name="top"></param>
        /// <param name="D_bucket"></param>
        /// <returns> True if all parents of a task have already been added</returns>
        private bool allParentsPresent(Task top, List<string> D_bucket)
        {
            if (top.Parents.Count() == 0)
            {
                return true;
            }
            foreach (var parentTask in top.Parents)
            {
                if (!D_bucket.Contains(parentTask))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Clear the internal data of the program like the graph list
        /// </summary>
        private void clearDisplay()
        {
            recolorTextboxes();
            tn1.Clear();
            tn2.Clear();
            tn3.Clear();
            tn4.Clear();
            tn5.Clear();
            tn6.Clear();
            tn7.Clear();
            tn8.Clear();
            tn9.Clear();
            tn10.Clear();
            tn11.Clear();
            tn12.Clear();
            tn13.Clear();
            tn14.Clear();
            tn15.Clear();

            tt1.Clear();
            tt2.Clear();
            tt3.Clear();
            tt4.Clear();
            tt5.Clear();
            tt6.Clear();
            tt7.Clear();
            tt8.Clear();
            tt9.Clear();
            tt10.Clear();
            tt11.Clear();
            tt12.Clear();
            tt13.Clear();
            tt14.Clear();
            tt15.Clear();

            p1.Clear();
            p2.Clear();
            p3.Clear();
            p4.Clear();
            p5.Clear();
            p6.Clear();
            p7.Clear();
            p8.Clear();
            p9.Clear();
            p10.Clear();
            p11.Clear();
            p12.Clear();
            p13.Clear();
            p14.Clear();
            p15.Clear();

            txtUnitDemand.Clear();
            txtProdTime.Clear();
            txtCT.Text = "";

            txtAllTasks.Text = "";
            txtABucket.Text = "";
            txtDBucket.Text = "";
            txtEff.Text = "";
            txtIdleTime.Text = "";
            txtTotalTaskTime.Text = "";

            txtWS1.Text = "";
            txtWS2.Text = "";
            txtWS3.Text = "";
            txtWS4.Text = "";
            txtWS5.Text = "";
            txtWS6.Text = "";
            txtWS7.Text = "";

            cbxUnit.SelectedIndex = 0;
        }
        /// <summary>
        /// Clears all class level variables and clears the Workstation textboxes
        /// </summary>
        private void clearData()
        {
            graph.Clear();
            cycle_time = 0;
            sortChoice = false;
            WS_tasks.Clear();
            WS_times.Clear();
            Rem_idletimes.Clear();
            WScounter = 0;

            txtWS1.Text = "";
            txtWS2.Text = "";
            txtWS3.Text = "";
            txtWS4.Text = "";
            txtWS5.Text = "";
            txtWS6.Text = "";
            txtWS7.Text = "";

        }
        /// <summary>
        /// Take in the information from a textbox and populate the global variables
        /// </summary>
        /// <param name="tn"></param>
        /// <param name="tt"></param>
        /// <param name="p"></param>
        private void loadTextBox(TextBox tn, TextBox tt, TextBox p)
        {
            if (tn.Text.Length != 0)
            {
                Task task = new Task();
                task.Name = tn.Text.ToUpper();
                task.Time = Convert.ToDouble(tt.Text);
                if (cbxUnit.SelectedIndex == 1) // Convert minute input to seconds
                {
                    task.Time *= 60;
                }

                string[] parents = p.Text.ToUpper().Split();
                foreach (string parent in parents)
                {
                    if (parent.Length != 0)
                    {
                        task.Parents.Add(parent);
                    }
                }

                graph.Add(task.Name, task);
            }
        }

        /// <summary>
        /// Take in info from the textboxes and populate each tasks follower count
        /// </summary>
        private void loadInfo()
        {
            // Go through all Task Names and create Task objects
            loadTextBox(tn1, tt1, p1);
            loadTextBox(tn2, tt2, p2);
            loadTextBox(tn3, tt3, p3);
            loadTextBox(tn4, tt4, p4);
            loadTextBox(tn5, tt5, p5);
            loadTextBox(tn6, tt6, p6);
            loadTextBox(tn7, tt7, p7);
            loadTextBox(tn8, tt8, p8);
            loadTextBox(tn9, tt9, p9);
            loadTextBox(tn10, tt10, p10);
            loadTextBox(tn11, tt11, p11);
            loadTextBox(tn12, tt12, p12);
            loadTextBox(tn13, tt13, p13);
            loadTextBox(tn14, tt14, p14);
            loadTextBox(tn15, tt15, p15);

            // Load in Production time and Cycle time
            prodTime = Double.Parse(txtProdTime.Text);
            unitDemand = Convert.ToInt32(txtUnitDemand.Text);
            cycle_time = (prodTime * 60 * 60) / unitDemand;
            txtCT.Text = cycle_time.ToString();

            // Load in Algorithm speed
            determineSpeed();

            // Load in sortchoice
            // 0 - Longest Task Time
            // 1 - Longest Follower Count
            int sortingIndex = lbxSortChoice.SelectedIndex;
            switch (sortingIndex)
            {
                case 0:
                    sortChoice = true;
                    break;
                case 1:
                    sortChoice = false;
                    break;
                default:
                    sortChoice = true;
                    break;
            }

            // Add followers to each task
            foreach (var pair in graph)
            {
                foreach (string parent in pair.Value.Parents)
                {
                    if (parent.Length != 0)
                    {
                        //MessageBox.Show("Parent: " + parent + ", length: " + parent.Length);
                        graph[parent].Followers.Add(pair.Value.Name);

                    }
                }
            }
        }

        /// <summary>
        /// Sorts bucket based on
        /// TRUE: Longest Task Time
        /// FALSE: Longest Follower Count
        /// </summary>
        /// <param name="choice"></param>
        private void sortBucket(List<Task> bucket, bool choice)
        {
            switch (choice)
            {
                case true:
                    sortByLargestTT(bucket);
                    break;
                case false:
                    sortByLargestFollowNum(bucket);
                    break;
            }

        }

        /// <summary>
        /// Bubble sorts the bucket in descending order (biggest first) by Task Time
        /// </summary>
        /// <param name="bucket"></param>
        private void sortByLargestTT(List<Task> bucket)
        {
            int n = bucket.Count;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (bucket[j].Time < bucket[j + 1].Time)
                    {
                        var tempVar = bucket[j];
                        bucket[j] = bucket[j + 1];
                        bucket[j + 1] = tempVar;
                    }
                    // If time1 == time2, make swap based on bigger follower count. 
                    else if (bucket[j].Time == bucket[j + 1].Time)
                    {
                        if (bucket[j].Followers.Count() < bucket[j + 1].Followers.Count())
                        {
                            var tempVar = bucket[j];
                            bucket[j] = bucket[j + 1];
                            bucket[j + 1] = tempVar;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Bubble sorts the tasks by largest follower count
        /// </summary>
        /// <param name="bucket"></param>
        private void sortByLargestFollowNum(List<Task> bucket)
        {
            int n = bucket.Count;

            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (bucket[j].Followers.Count() < bucket[j + 1].Followers.Count())
                    {
                        var tempVar = bucket[j];
                        bucket[j] = bucket[j + 1];
                        bucket[j + 1] = tempVar;
                    }
                    // If time1 == time2, make swap based on bigger follower count. 
                    else if (bucket[j].Followers.Count() == bucket[j + 1].Followers.Count())
                    {
                        if (bucket[j].Time < bucket[j + 1].Time)
                        {
                            var tempVar = bucket[j];
                            bucket[j] = bucket[j + 1];
                            bucket[j + 1] = tempVar;
                        }
                    }

                }
            }
        }
        /// <summary>
        /// Runs the tutorial of the Program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTutorial_Click(object sender, EventArgs e)
        {
            clearAll();
            SolidColorBrush highlight = Brushes.PaleVioletRed;
            System.Windows.Media.Color hexColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#504f52");
            SolidColorBrush disabledColor = new SolidColorBrush(hexColor);
            string message = "Welcome to the Workstation Distribution Tutorial. " +
                "This tutorial will show you how to use this tool when distributing tasks over a number of workstations.";
            if (MessageBox.Show(message, "WS Distro Tutorial", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                clearAll();
                return;
            }

            // load the presetTutorial into the view
            lbxPresets.SelectedIndex = 2;
            displayPreset(2);
            // Then start the algo
            #region Highlighting data textboxes
            tn1.Background = highlight;
            tt1.Background = highlight;
            p1.Background = highlight;

            tn2.Background = highlight;
            tt2.Background = highlight;
            p2.Background = highlight;

            tn3.Background = highlight;
            tt3.Background = highlight;
            p3.Background = highlight;

            tn4.Background = highlight;
            tt4.Background = highlight;
            p4.Background = highlight;

            tn5.Background = highlight;
            tt5.Background = highlight;
            p5.Background = highlight;

            tn6.Background = highlight;
            tt6.Background = highlight;
            p6.Background = highlight;

            tn7.Background = highlight;
            tt7.Background = highlight;
            p7.Background = highlight;

            tn8.Background = highlight;
            tt8.Background = highlight;
            p8.Background = highlight;

            message = "We will use the following sample data to practice:";
            if (MessageBox.Show(message, "WS Distro Tutorial", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                clearAll();
                return;
            }
            recolorTextboxes();
            #endregion

            #region Highlighting ProdTime, UnitDemand, CycleTime Textboxes
            txtUnitDemand.Background = highlight;
            txtProdTime.Background = highlight;
            txtCT.Background = highlight;
            message = "You can type in your Production Time in Hours and Unit Demand in these boxes. " +
                "The Cycle Time is automatically calculated upon running the algorithm";
            if (MessageBox.Show(message, "WS Distro Tutorial", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                clearAll();
                return;
            }
            recolorTextboxes();
            #endregion

            #region Highlighting WS textboxes
            txtWS1.Background = highlight;
            message = "This textbox shows:\n" +
                "1) The current workstation number for which we are assigning\n" +
                "2) The distribution of tasks and the order to perform them.\n" +
                "3) The times each task takes and the order to do them.\n" +
                "4) The order of the task times that are being subtracted from the Cycle Time";
            if (MessageBox.Show(message, "WS Distro Tutorial", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                clearAll();
                return;
            }
            txtWS1.Background = disabledColor;
            #endregion

            #region Highligthing Efficiency textboxes
            alltasks.Background = highlight;
            message = "This textbox is used to show all the tasks we will add to the Workstation Distribution";
            if (MessageBox.Show(message, "WS Distro Tutorial", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                clearAll();
                return;
            }
            alltasks.Background = disabledColor;

            tottasks.Background = highlight;
            message = "This textbox is used to show the total task time the distribution takes in seconds";
            if (MessageBox.Show(message, "WS Distro Tutorial", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                clearAll();
                return;
            }
            tottasks.Background = disabledColor;

            Atasks.Background = highlight;
            message = "This textbox is used to show what tasks are eligible to be added to the current workstation. " +
                "The rule you choose decides which task is in the end added to the current workstation";
            if (MessageBox.Show(message, "WS Distro Tutorial", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                clearAll();
                return;
            }
            Atasks.Background = disabledColor;

            Dtasks.Background = highlight;
            message = "This textbox is used to show all the tasks that we have already added to the workstation";
            if (MessageBox.Show(message, "WS Distro Tutorial", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                clearAll();
                return;
            }
            Dtasks.Background = disabledColor;

            totidle.Background = highlight;
            message = "This textbox is used to show the total idle time of the current workstation distribution in seconds";
            if (MessageBox.Show(message, "WS Distro Tutorial", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                clearAll();
                return;
            }
            totidle.Background = disabledColor;

            eff.Background = highlight;
            message = "This textbox is used to show the efficiency of the process in a percentage";
            if (MessageBox.Show(message, "WS Distro Tutorial", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                clearAll();
                return;
            }
            eff.Background = disabledColor;
            #endregion

            speed = true; // Set speed to Step-By-Step just in case
            // Go through Algorithm and highlight things
            distributeTasks();
        }
        /// <summary>
        /// recolor all Textboxes (happens during tutorial)
        /// </summary>
        private void recolorTextboxes()
        {
            System.Windows.Media.Color inputHexColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#7c848a");
            SolidColorBrush inputColor = new SolidColorBrush(inputHexColor);
            System.Windows.Media.Color hexColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#504f52");
            SolidColorBrush disabledColor = new SolidColorBrush(hexColor);
            System.Windows.Media.Color hexbutton = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFDDDDDD");
            SolidColorBrush buttonColor = new SolidColorBrush(hexbutton);

            tn1.Background = inputColor;
            tn1.Background = inputColor;
            tn2.Background = inputColor;
            tn3.Background = inputColor;
            tn4.Background = inputColor;
            tn5.Background = inputColor;
            tn6.Background = inputColor;
            tn7.Background = inputColor;
            tn8.Background = inputColor;

            tt1.Background = inputColor;
            tt2.Background = inputColor;
            tt3.Background = inputColor;
            tt4.Background = inputColor;
            tt5.Background = inputColor;
            tt6.Background = inputColor;
            tt7.Background = inputColor;
            tt8.Background = inputColor;

            p1.Background = inputColor;
            p2.Background = inputColor;
            p3.Background = inputColor;
            p4.Background = inputColor;
            p5.Background = inputColor;
            p6.Background = inputColor;
            p7.Background = inputColor;
            p8.Background = inputColor;

            txtProdTime.Background = inputColor;
            txtUnitDemand.Background = inputColor;
            txtCT.Background = disabledColor;

            txtWS1.Background = disabledColor;
            txtWS2.Background = disabledColor;
            txtWS3.Background = disabledColor;
            txtWS4.Background = disabledColor;
            txtWS5.Background = disabledColor;
            txtWS6.Background = disabledColor;
            txtWS7.Background = disabledColor;

            txtABucket.Background = disabledColor;
            txtDBucket.Background = disabledColor;
            txtAllTasks.Background = disabledColor;
            txtEff.Background = disabledColor;
            txtIdleTime.Background = disabledColor;

            //btnReloadPresets.Background = buttonColor;
        }

        /// <summary>
        /// Clear all and return focus to first textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click_1(object sender, RoutedEventArgs e)
        {
            clearAll();
            tn1.Focus();
        }

        /// <summary>
        /// Someone has clicked a different preset so load said preset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbxPresets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear the display
            clearDisplay();

            // Display the information based on the preset data
            displayPreset(lbxPresets.SelectedIndex);
        }
    }
}
