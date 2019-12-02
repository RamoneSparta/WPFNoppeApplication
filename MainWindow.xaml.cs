using System;
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
        #region Global Variables
        List<Note> listOfNotes = new List<Note>();
        Note selectedNoteToAdd = new Note();
        #endregion

        #region Public MainWindow
        public MainWindow()
        {
            InitializeComponent();
            /*
             * If you are using the inner join:
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
        #endregion

        #region Initialising 
        public void InitiliseTheCode()
        {
            using (var db = new NoppeDBEntities())
            {
                listOfNotes = db.Notes.ToList();
            }
            DisplayNotes.ItemsSource = listOfNotes;
            DisplayNotes.DisplayMemberPath = "NoteTitle";
        }
        #endregion

        #region Textbox: On Text Change
        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            // SQL Query based off of the ComboBox selection
            using (var db = new NoppeDBEntities())
            {
                listOfNotes = new List<Note>();
                if (((ComboBoxItem)ComboBoxSearch.SelectedItem).Content.ToString() == "Title")
                {
                    var queryForSeachBar =
                        (
                            from notes in db.Notes
                            where notes.NoteTitle.Contains(TextBoxSearch.Text)
                            orderby notes.NoteTitle
                            select notes

                        ).ToList();
                    foreach (Note note in queryForSeachBar)
                    {
                        listOfNotes.Add(note);
                    }
                    DisplayNotes.ItemsSource = listOfNotes;
                    DisplayNotes.DisplayMemberPath = "NoteTitle";
                }
                else if (((ComboBoxItem)ComboBoxSearch.SelectedItem).Content.ToString() == "Text")
                {
                    var queryForSeachBar =
                        (
                            from notes in db.Notes
                            where notes.NoteDescription.Contains(TextBoxSearch.Text)
                            orderby notes.NoteDescription
                            select notes

                         ).ToList();
                    foreach (Note note in queryForSeachBar)
                    {
                        listOfNotes.Add(note);
                    }
                    DisplayNotes.ItemsSource = listOfNotes;
                    DisplayNotes.DisplayMemberPath = "NoteDescription";
                }
            }
        }
        #endregion

        #region Displaynotes: Section Changed
        private void DisplayNotes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxTitle.IsEnabled = true;
            TextBoxDescription.IsEnabled = true;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
            TextBoxTitle.Document.Blocks.Clear();
            TextBoxDescription.Document.Blocks.Clear();
            selectedNoteToAdd = (Note)DisplayNotes.SelectedItem;
            if (selectedNoteToAdd != null)
            {
                TextBoxTitle.Document.Blocks.Add(new Paragraph(new Run(selectedNoteToAdd.NoteTitle.ToString())));
                TextBoxDescription.Document.Blocks.Add(new Paragraph(new Run(selectedNoteToAdd.NoteDescription.ToString())));
            }
            TextBoxTitle.IsEnabled = false;
            TextBoxDescription.IsEnabled = false;
            ButtonEdit.IsEnabled = true;
            ButtonDelete.IsEnabled = true;
        }
        #endregion

        #region Get button Content
        private string GetButtonContent(Button o)
        {
            //Button name = (Button)o;
            return o.Content.ToString();
        }
        #endregion

        #region Edit: Button
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
                if (selectedNoteToAdd != null)
                {
                    var noteID = selectedNoteToAdd.NoteID;
                    selectedNoteToAdd = new Note()
                    {
                        NoteID = noteID,
                        NoteTitle = new TextRange(TextBoxTitle.Document.ContentStart, TextBoxTitle.Document.ContentEnd).Text,
                        NoteDescription = new TextRange(TextBoxDescription.Document.ContentStart, TextBoxDescription.Document.ContentEnd).Text
                    };
                    // To Do saving code here
                    using (var db = new NoppeDBEntities())
                    {
                        var noteToEdit = db.Notes.Find(this.selectedNoteToAdd.NoteID);
                        noteToEdit.NoteDescription = this.selectedNoteToAdd.NoteDescription;
                        noteToEdit.NoteTitle = this.selectedNoteToAdd.NoteTitle;
                        db.SaveChanges();
                        DisplayNotes.ItemsSource = null; // reset listbox
                        listOfNotes = db.Notes.ToList();
                        DisplayNotes.ItemsSource = listOfNotes;
                    }

                    TextBoxTitle.Document.Blocks.Clear();
                    TextBoxDescription.Document.Blocks.Clear();
                    TextBoxTitle.IsEnabled = false;
                    TextBoxDescription.IsEnabled = false;
                    ButtonEdit.Content = "Edit";
                    ButtonDelete.IsEnabled = false;
                    ButtonEdit.IsEnabled = false;
                }
            }
        }
        #endregion

        #region Add: Button
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
        #endregion

        #region Delete: Button
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
        #endregion

    }


}
