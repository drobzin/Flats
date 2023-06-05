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
    /// Логика взаимодействия для WinNewCity.xaml
    /// </summary>
    public partial class WinAddNewAppartmentRow : Window
    {
        private readonly string insertInstruction = "INSERT INTO appartament " +
                                    "(Street, House, Flat, DistrictId, Floors, Floor, TypeHouse, TypeToilet, TypePlan, SqAll, Private, Phone, Cost, Photo, Plan, RegID, AgentId)" +
                                    " VALUES (@0,@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13,@14,@15, @16)";
        private readonly string updateInstruction = "UPDATE appartament SET Street = @0, House = @1, Flat = @2, DistrictId = @3, Floors = @4, Floor = @5, TypeHouse = @6, TypeToilet = @7," +
                                                     " TypePlan = @8, SqAll = @9, Private = @10, Phone = @11, Cost = @12, Photo = @13, Plan = @14, RegID = @15, AgentId = @16 WHERE (idAppartament = @17)";
        private readonly bool editing;
        private readonly int id;
        private readonly DataRow selectedRow;
        public WinAddNewAppartmentRow(DataRow _selectedRow, bool _editing)
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
