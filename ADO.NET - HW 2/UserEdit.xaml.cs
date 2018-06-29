using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

namespace ADO.NET___HW_2
{
	/// <summary>
	/// Interaction logic for UserEdit.xaml
	/// </summary>
	public partial class UserEdit : Window
	{
		DataTable dataTable;
		int editIndex = -1;
		public UserEdit(DataTable dt)
		{
			InitializeComponent();
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
			dataTable = dt;
		}
		public UserEdit(DataTable dt, int selectedIndex) : this(dt)
		{
			LoginTextbox.Text = dataTable.Rows[selectedIndex][1].ToString();
			PasswordTextbox.Password = dataTable.Rows[selectedIndex][2].ToString();
			AddressTextbox.Text = dataTable.Rows[selectedIndex][3].ToString();
			PhoneTextbox.Text = dataTable.Rows[selectedIndex][4].ToString();
			AdminFlag.IsChecked = (bool)dataTable.Rows[selectedIndex][5];
			editIndex = selectedIndex;
			if (editIndex != -1)
				CreateButton.Content = "Update user";
		}

		private void CreateButton_Click(object sender, RoutedEventArgs e)
		{
			if(editIndex == -1 && dataTable.AsEnumerable().Where(field => field.ItemArray[1].ToString().ToLower() == LoginTextbox.Text.ToLower()).Count() > 0)
			{
				MessageBox.Show("User already exists!");
				return;
			}

			DataRow dr = (editIndex > -1) ? dataTable.Rows[editIndex] : dataTable.NewRow();
			dr[1] = LoginTextbox.Text;
			dr[2] = PasswordTextbox.Password.GetHashCode();
			dr[3] = AddressTextbox.Text;
			dr[4] = PhoneTextbox.Text;
			dr[5] = AdminFlag.IsChecked;
			if (editIndex == -1)
			{
				dr[0] = int.Parse(dataTable.Rows[dataTable.Rows.Count-1][0].ToString()) + 1;
				dataTable.Rows.Add(dr);
			}

			DialogResult = true;
			Close();
		}

		private void LoginTextbox_TextChanged(object sender, RoutedEventArgs e)
		{
			CreateButton.IsEnabled = LoginTextbox.Text.Length > 0 && PasswordTextbox.Password.Length > 0 ? true : false;
		}
	}
}
