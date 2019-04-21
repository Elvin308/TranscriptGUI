using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Transcript;
using System.Runtime.Serialization.Json;

namespace TranscriptGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Transcript.Transcript> transcripts;

        public MainWindow()
        {
            //centers the first window
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            InitializeComponent();
        }

        private void FindJSONFileButton_Click(object sender, RoutedEventArgs e)
        {
            String currentWorkingDirectory = Directory.GetCurrentDirectory(); // returns a string of the current directory

            #region Makes a file search dialog
            OpenFileDialog openFileDialog = new OpenFileDialog(); // creates an instance of openFileDialog GUI for the user to interact with
            openFileDialog.InitialDirectory = currentWorkingDirectory; // sets the initial directory to the one I defined
            openFileDialog.Filter = "Json files (*.json) | *.json"; //Add the filter for json files
            openFileDialog.FilterIndex = 0; // sets the initial filter to the define filter in the index 0
            openFileDialog.Title = "Open Transcript from JSON";
            openFileDialog.Multiselect = false;
            openFileDialog.ShowDialog(); //shows the dialog
            #endregion

            string filename = openFileDialog.FileName;

            if (filename != "")
            {
                #region Reads in the JSON data and parses it into a transcript list object
                FileStream reader = new FileStream(filename, FileMode.Open, FileAccess.Read);
                DataContractJsonSerializer serializerJSON = new DataContractJsonSerializer(typeof(List<Transcript.Transcript>));
                transcripts = (List<Transcript.Transcript>)serializerJSON.ReadObject(reader);
                #endregion

                reader.Close();

                #region Clear All Fields
                NameTextBox.Text = "";
                IDTextBox.Text = "";
                MajorTextBox.Text = "";
                CourseListView.Items.Clear();
                findNameTextBox.Text = "";
                #endregion

                FileNameTextBox.Text = filename;
            }

        }

        private void FindTranscriptbyNameButton_Click(object sender, RoutedEventArgs e)
        {
            #region Clear All Fields
            NameTextBox.Text = "";
            IDTextBox.Text = "";
            MajorTextBox.Text = "";
            CourseListView.Items.Clear();
            #endregion

            Boolean studentFound = false;
            String studentName = findNameTextBox.Text;
            

            if (transcripts != null && studentName.Length>0)
            {
                foreach (var transcriptItem in transcripts)
                {
                    //Finds where there is a Transcript in the transcript list that has a student name that matches the one the user gave
                    if (transcriptItem.Student.Name == studentName)
                    {
                        studentFound = true;

                        #region Retrieves and sets all student information to the gui textboxes
                        NameTextBox.Text = transcriptItem.Student.Name; // add name to the textbox..... could also just use the same name user inputted
                        IDTextBox.Text = transcriptItem.Student.Id; //adds the ID of the searched student in the IDTextBox
                        MajorTextBox.Text = transcriptItem.Student.Major; //adds the Major of the searched student in the MajorTextBox
                        #endregion

                        #region Retrieves and sets all course objects related to the specified student into the ListView 
                        //adds the whole course object to the CourseListView
                        foreach (var courseItem in transcriptItem.Courses)
                        {
                            CourseListView.Items.Add(courseItem); 
                        }
                        #endregion

                        break; //breaks out of the foreach loop since the student has been found
                    }
                }

                // If student was not found a dialog box will state that the student could not be found
                if (!studentFound)
                {
                    MessageBox.Show("Student not found");
                }
            }
            else if (transcripts == null)
            {
                MessageBox.Show("No JSON file found");
            }
            else
            {
                MessageBox.Show("Enter Student name");
            }
        }

        private void FindNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(findNameTextBox.Text.Length > 0)
            {
                hint.Visibility = Visibility.Hidden;
            }
            else
                hint.Visibility = Visibility.Visible;
        }
    }
}
