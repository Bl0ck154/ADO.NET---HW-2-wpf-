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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace ADO.NET___HW_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		SqlConnection sqlConnection;
		SqlDataAdapter sqlDataAdapter;
		DataSet dataSet;

		public MainWindow()
        {
            InitializeComponent();
			Loaded += MainWindow_Loaded;
			sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["mainConnect"].ConnectionString);
			
			WindowStartupLocation = WindowStartupLocation.CenterScreen;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				sqlConnection.Open();

				try // first use create table Users
				{
					new SqlCommand(@"create table Users ( 
id INT not null primary key identity,
login varchar(32) not null,
password varchar(32) not null, 
address varchar(100), 
phone varchar(16),
admin BIT default 0)", sqlConnection).ExecuteNonQuery();
				}
				catch (Exception)
				{
				}

				 sqlDataAdapter = new SqlDataAdapter("select * from Users", sqlConnection);
				dataSet = new DataSet();
				sqlDataAdapter.Fill(dataSet, "Users");
				foreach (DataRow row in dataSet.Tables["Users"].Rows)
				{
					ListBoxItem lbi = new ListBoxItem();
					lbi.MouseDoubleClick += UsersListbox_MouseDoubleClick;

					string tmp = "";
					foreach (var item in row.ItemArray)
					{
						tmp  += item + " ";

					}
					lbi.Content = tmp;

					UsersListbox.Items.Add(lbi);
				
				}
				
				//UsersListbox.ItemsSource = dataSet.Tables["Users"].DefaultView;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				if(sqlConnection.State == ConnectionState.Open)
					sqlConnection.Close();
			}

		}

		private void UsersListbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if(UsersListbox.SelectedItems.Count > 0)
				MessageBox.Show(UsersListbox.SelectedItem.ToString());
		}

		private void CreateButton_Click(object sender, RoutedEventArgs e)
		{
			if(new UserEdit().ShowDialog() == true)
			{
				sqlDataAdapter.Update(dataSet);
			}
		}

		private void RemoveButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void UsersListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RemoveButton.IsEnabled = UsersListbox.SelectedItems.Count == 0 ? false : true;
		}
	}
}
