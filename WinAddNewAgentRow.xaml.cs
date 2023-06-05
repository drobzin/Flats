using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using Xceed.Wpf.Toolkit.Panels;

namespace Flats
{
    /// <summary>
    /// Логика взаимодействия для WinAddNewAgentRow.xaml
    /// </summary>
    public partial class WinAddNewAgentRow : Window
    {
        private readonly string insertInstruction = "INSERT INTO agent " +
                                    "(login, password, name)" +
                                    " VALUES (@0,@1,@2)";
        private readonly string updateInstruction = "UPDATE agent  SET  login = @0, password =@1, name = @2 WHERE (idagent = @3)";
        private readonly ListView listView = new ListView();
        private readonly bool editing;
        private readonly int id;
        private readonly DataRow selectedRow;
        public WinAddNewAgentRow(DataRow _selectedRow, bool _editing)
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
