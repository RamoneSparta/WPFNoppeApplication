﻿using System;
using System.Collections.Generic;
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

namespace Noppe_Note_Taking_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Note> listOfNotes = new List<Note>();
        Note selectedNoteToAdd = new Note();

        public MainWindow()
        {
            InitializeComponent();
            /*
             * If you are using the inner join
            using (var db = new TaskDBEntities)
            {
                // Inner join
                var taskList = 
                from task in db.Tasks
                join category in db.Categories 
                on task.CategoryID equals category.CategoryID
                // Have to create a new output object (Custom Object because C# doesn't know how to use it)
                select new
                {
                
                    taskID = task.TaskId
                    description = task.Description
                    category = Task.CategeoryName
                
                };
                foreach (var task in taskList.ToList())
                {
                    System.Diagnostics.Trace.WriteLine($"{task.taskId, -5}{task.category}");
                }
            }
            */

            InitiliseTheCode();
        }

        public void InitiliseTheCode()
        {
            using (var db = new NoppeDBEntities())
            {
                listOfNotes = db.Notes.ToList();
            }
            DisplayNotes.ItemsSource = listOfNotes;
            DisplayNotes.DisplayMemberPath = "NoteTitle";
        }

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            // SQL Query based off of the ComboBox selection
        }

        private void DisplayNotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxTitle.IsEnabled = true;
            TextBoxDescription.IsEnabled = true;
            TextBoxTitle.Document.Blocks.Clear();
            TextBoxDescription.Document.Blocks.Clear();
            selectedNoteToAdd = (Note)DisplayNotes.SelectedItem;
            TextBoxTitle.Document.Blocks.Add(new Paragraph(new Run(selectedNoteToAdd.NoteTitle.ToString())));
            TextBoxDescription.Document.Blocks.Add(new Paragraph(new Run(selectedNoteToAdd.NoteDescription.ToString())));
            TextBoxTitle.IsEnabled = false;
            TextBoxDescription.IsEnabled = false;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            if (GetButtonContent(ButtonEdit) == "Edit")
            {
                TextBoxTitle.IsEnabled = true;
                TextBoxDescription.IsEnabled = true;
                ButtonEdit.Content = "Save";
            }
            else if (GetButtonContent(ButtonEdit) == "Save")
            {
                // To Do saving code here
                using (var db = new NoppeDBEntities())
                {

                }

                TextBoxTitle.Document.Blocks.Clear();
                TextBoxDescription.Document.Blocks.Clear();
                TextBoxTitle.IsEnabled = false;
                TextBoxDescription.IsEnabled = false;
                ButtonEdit.Content = "Edit";
            }
        }

        private string GetButtonContent(Button o)
        {
            //Button name = (Button)o;
            return o.Content.ToString();
        }

        private void ButttonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (GetButtonContent(ButtonAdd) == "Add")
            {
                TextBoxTitle.IsEnabled = true;
                TextBoxDescription.IsEnabled = true;
                TextBoxTitle.Document.Blocks.Clear();
                TextBoxDescription.Document.Blocks.Clear();
                ButtonAdd.Content = "Confirm?";
            }
            else if (GetButtonContent(ButtonAdd) == "Confirm?")
            {
                selectedNoteToAdd = new Note()
                {
                    NoteTitle = new TextRange(TextBoxTitle.Document.ContentStart,TextBoxTitle.Document.ContentEnd).Text,
                    NoteDescription = new TextRange(TextBoxDescription.Document.ContentStart, TextBoxDescription.Document.ContentEnd).Text
                };

                using (var db = new NoppeDBEntities())
                {
                    db.Notes.Add(this.selectedNoteToAdd);
                    db.SaveChanges();
                    DisplayNotes.ItemsSource = null; // reset listbox
                    listOfNotes = db.Notes.ToList();
                    DisplayNotes.ItemsSource = listOfNotes;
                }
                ButtonAdd.Content = "Add";
                // Clear out boxes
                TextBoxDescription.Document.Blocks.Clear();
                TextBoxTitle.Document.Blocks.Clear();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (GetButtonContent(ButtonDelete)=="Delete")
            {
                ButtonDelete.Content = "Are You Sure?";
            }
            else if (GetButtonContent(ButtonDelete) == "Are You Sure?")
            {
                using (var db = new NoppeDBEntities())
                {
                    var removeItemID = db.Notes.Find(selectedNoteToAdd.NoteID);
                    db.Notes.Remove(removeItemID);
                    db.SaveChanges();
                    DisplayNotes.ItemsSource = null; // reset listbox
                    listOfNotes = db.Notes.ToList();
                    DisplayNotes.ItemsSource = listOfNotes;
                  
                }
                ButtonDelete.Content = "Delete";
                ButtonDelete.IsEnabled = false;
                ButtonEdit.IsEnabled = false;
                TextBoxTitle.Document.Blocks.Clear();
                TextBoxDescription.Document.Blocks.Clear();
            }
        }
    }


}
