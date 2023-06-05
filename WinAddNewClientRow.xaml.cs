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
using System.Windows.Shapes;
using System.Data;
using MySql.Data.MySqlClient;

namespace Flats
{
    /// <summary>
    /// Логика взаимодействия для WinAddNewClientRow.xaml
    /// </summary>
    public partial class WinAddNewClientRow : Window
    {
        private readonly string insertInstruction = "INSERT INTO client " +
                                    "(Street, House, Name, Document, Login,Password)" +
                                    " VALUES (@0,@1,@2,@3)";
        private readonly string updateInstruction = "UPDATE `center`.`client` SET `StreetId` = @0, House = @1, `Name` = @2, `Document` = @3, Login = @4, Password = @5 WHERE (`RegId` = @6)";
        private readonly ListView listView = new ListView();
        private readonly bool editing;
        private readonly int id;
        private readonly DataRow selectedRow;
        public WinAddNewClientRow(DataRow _selectedRow, bool _editing)
        {
            InitializeComponent();
            selectedRow = _selectedRow;
            editing = _editing;
            if (editing)
            {
                id = Convert.ToInt32(selectedRow[0]);
                ChangeTable.FillTextBoxes(selectedRow, wrapPanel);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (editing)
            {
                ChangeTable.Add_Row(updateInstruction, wrapPanel, this, id);

            }
            else
            {
                ChangeTable.Add_Row(insertInstruction, wrapPanel, this);
            }
        }
    }

}


