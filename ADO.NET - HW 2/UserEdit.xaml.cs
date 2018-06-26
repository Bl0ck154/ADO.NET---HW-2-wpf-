using System;
using System.Collections.Generic;
using System.Configuration;
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
		public UserEdit()
		{
			InitializeComponent();
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}
		//public UserEdit(/*params*/) 
		//{

		//}

		private void CreateButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				using (SqlConnection sqlConnection =
					new SqlConnection(ConfigurationManager.ConnectionStrings["mainConnect"].ConnectionString))
				{
					sqlConnection.Open();
					SqlCommand command = new SqlCommand(@"insert into Users VALUES (@login, @password, @address, @phone, " +
						(AdminFlag.IsChecked == true ? "1)" : "0)"), sqlConnection);
					command.Parameters.AddWithValue("@login", LoginTextbox.Text);
					command.Parameters.AddWithValue("@password", PasswordTextbox.Password.GetHashCode());
					command.Parameters.AddWithValue("@address", AddressTextbox.Text);
					command.Parameters.AddWithValue("@phone", PhoneTextbox.Text);
					command.ExecuteNonQuery();
					DialogResult = true;
					Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void LoginTextbox_TextChanged(object sender, RoutedEventArgs e)
		{
			CreateButton.IsEnabled = LoginTextbox.Text.Length > 0 && PasswordTextbox.Password.Length > 0 ? true : false;
		}
	}
}
