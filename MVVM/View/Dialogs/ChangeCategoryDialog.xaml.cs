using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Universal_THCRAP_Launcher.MVVM.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ChangeCategoryDialog.xaml
    /// </summary>
    public partial class ChangeCategoryDialog : Window
    {
        public ObservableCollection<string> Categories { get; private set; } = new ObservableCollection<string>();
        public string SelectedCategory { get; set; }

        // fields to support dragging
        private Point _dragStartPoint;
        private object _draggedItem;

        public ChangeCategoryDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        // Call this to populate the dialog with current categories before showing
        public void InitializeCategories(System.Collections.Generic.IEnumerable<string> categories)
        {
            Categories.Clear();
            foreach (var c in categories) Categories.Add(c);
            if (Categories.Count > 0) SelectedCategory = Categories[0];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Focus the new category box for convenience
            NewCategoryTextBox.Focus();
        }

        #region Drag & Drop reorder
        private void CategoryListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
            // store the item under the mouse as potential drag source
            var listBox = (ListBox)sender;
            var item = VisualUpwardSearch<ListBoxItem>((DependencyObject)e.OriginalSource);
            _draggedItem = item?.DataContext;
        }

        private void CategoryListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            var position = e.GetPosition(null);
            var diff = _dragStartPoint - position;
            if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                if (_draggedItem == null) return;

                var listBox = (ListBox)sender;
                DragDropEffects finalDropEffect = DragDrop.DoDragDrop(listBox, _draggedItem, DragDropEffects.Move);
                // cleanup
                _draggedItem = null;
            }
        }

        private void CategoryListBox_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(string)) && !e.Data.GetDataPresent(typeof(object))) return;

            var droppedData = e.Data.GetData(typeof(string)) as string ?? e.Data.GetData(typeof(object)) as string;
            // fallback: data may be object, so try cast
            if (droppedData == null)
            {
                // try to get object and ToString()
                var obj = e.Data.GetData(typeof(object)) as object;
                if (obj != null) droppedData = obj.ToString();
            }

            // get target item
            var listBox = (ListBox)sender;
            var targetItem = VisualUpwardSearch<ListBoxItem>((DependencyObject)e.OriginalSource);
            string targetData = targetItem?.DataContext as string;

            if (droppedData == null) return;

            var srcIndex = Categories.IndexOf(droppedData);
            var dstIndex = (targetData == null) ? Categories.Count - 1 : Categories.IndexOf(targetData);

            if (srcIndex < 0) return;

            if (dstIndex < 0 || srcIndex == dstIndex) return;

            // remove and insert
            Categories.Move(srcIndex, dstIndex);
            listBox.SelectedItem = droppedData;
        }

        // helper to find parent ListBoxItem
        private static T VisualUpwardSearch<T>(DependencyObject source) where T : DependencyObject
        {
            while (source != null && !(source is T))
                source = System.Windows.Media.VisualTreeHelper.GetParent(source);

            return source as T;
        }
        #endregion

        #region Add / Rename / Delete
        private void CreateCategory_Click(object sender, RoutedEventArgs e)
        {
            TryAddNewCategory();
        }

        private void NewCategoryTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TryAddNewCategory();
            }
        }

        private void TryAddNewCategory()
        {
            var name = NewCategoryTextBox.Text?.Trim();
            if (string.IsNullOrEmpty(name)) return;

            if (Categories.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show(this, "A category with that name already exists.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Categories.Add(name);
            NewCategoryTextBox.Clear();
            CategoryListBox.ScrollIntoView(name);
            CategoryListBox.SelectedItem = name;
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.Tag is string name)
            {
                var result = MessageBox.Show(this, $"Delete category '{name}'?", "Confirm delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Categories.Remove(name);
                }
            }
        }

        private void RenameCategory_Click(object sender, RoutedEventArgs e)
        {
            /*if (sender is Button b && b.Tag is string oldName)
            {
                var input = Microsoft.VisualBasic.Interaction.InputBox("Rename category:", "Rename", oldName);
                var newName = input?.Trim();
                if (string.IsNullOrEmpty(newName) || newName == oldName) return;
                if (Categories.Contains(newName, StringComparer.OrdinalIgnoreCase))
                {
                    MessageBox.Show(this, "A category with that name already exists.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var idx = Categories.IndexOf(oldName);
                if (idx >= 0)
                {
                    Categories[idx] = newName;
                    CategoryListBox.SelectedItem = newName;
                }
            }*/
        }
        #endregion

        #region Bottom / Window controls
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            // set DialogResult true so caller knows it was accepted
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // optional: if you have the same Window_LeftMouseButtonDown handler signature in MainWindow, reuse it
        private void Window_LeftMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        #endregion
    }
}