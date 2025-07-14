using System.Windows;
using System.Windows.Controls;
using Ventuz.Models;

namespace Ventuz.Views
{
    /// <summary>
    /// Interaktionslogik für ControlDialog.xaml
    /// </summary>
    public partial class ControlDialog : Window
    {
        public ControlElement Result { get; private set; }

#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        public ControlDialog()
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Fügen Sie ggf. den „erforderlichen“ Modifizierer hinzu, oder deklarieren Sie den Modifizierer als NULL-Werte zulassend.
        {
            InitializeComponent();
            TypeComboBox.SelectedIndex = 0;
        }

        public ControlDialog(ControlElement existing) : this()
        {
            NameTextBox.Text = existing.Name;
            CommandTextBox.Text = existing.Command;
            var typeItem = TypeComboBox.Items.Cast<ComboBoxItem>()
                                .FirstOrDefault(i => (string)i.Content == existing.Type);
            if (typeItem != null)
                TypeComboBox.SelectedItem = typeItem;
            else
                TypeComboBox.Text = existing.Type ?? "";
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            var name = NameTextBox.Text.Trim();
            var type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? TypeComboBox.Text.Trim();
            var command = CommandTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Bitte einen Namen eingeben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                MessageBox.Show("Bitte einen Typ eingeben.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Result = new ControlElement
            {
                Name = name,
                Type = type,
                Command = command
            };

            DialogResult = true;
        }
    }
}