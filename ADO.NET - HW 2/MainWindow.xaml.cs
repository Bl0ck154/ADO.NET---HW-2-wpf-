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
		SqlCommandBuilder sqlCommandBuilder;

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
				sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);
				sqlDataAdapter.Fill(dataSet, "Users");
				UsersListbox.DataContext = dataSet;
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
			ColName1.Content = dataSet.Tables[0].Columns[0].ColumnName;
			ColName2.Content = dataSet.Tables[0].Columns[1].ColumnName;
			ColName3.Content = dataSet.Tables[0].Columns[2].ColumnName;
			ColName4.Content = dataSet.Tables[0].Columns[3].ColumnName;
			ColName5.Content = dataSet.Tables[0].Columns[4].ColumnName;
			ColName6.Content = dataSet.Tables[0].Columns[5].ColumnName;
		}

		private void UsersListbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			int selected = findAdmInMainList(UsersListbox.SelectedIndex);
			bool admin = rowIsAdmin(dataSet.Tables[0].Rows[selected]);
			if (new UserEdit( dataSet.Tables[0], selected).ShowDialog() == true)
			{
				Update();
				if (admin)
					AdminsTableBuild();
			}
		}

		private void CreateButton_Click(object sender, RoutedEventArgs e)
		{
			if(new UserEdit(dataSet.Tables[0]).ShowDialog() == true)
			{
				Update();
			}
		}

		void Update() => sqlDataAdapter.Update(dataSet, "Users");

		private void RemoveButton_Click(object sender, RoutedEventArgs e)
		{
			if(UsersListbox.SelectedItems.Count>0)
			{
				DataRow dataRow = dataSet.Tables[0].Rows[findAdmInMainList(UsersListbox.SelectedIndex)];
				bool admin = rowIsAdmin(dataRow);
				dataRow.Delete();
				Update();
				if(admin)
					AdminsTableBuild();
			}
		}
		int findAdmInMainList(int SelectedIndex)
		{
			if (!showAdminsTable)
				return SelectedIndex;

			for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
			{
				if (dataSet.Tables[0].Rows[i][0].ToString() == dataSet.Tables[1].Rows[SelectedIndex][0].ToString())
					return i;
			}
			return -1;
		}

		private void UsersListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RemoveButton.IsEnabled = UsersListbox.SelectedItems.Count == 0 ? false : true;
		}
		bool rowIsAdmin(DataRow dr) => ((bool)dr[5] == true);

		bool showAdminsTable = false;
		void AdminsTableBuild()
		{
			DataTable tempDT = dataSet.Tables[0].Copy();
			tempDT.TableName = "Admins";

			foreach (DataRow dr in tempDT.Rows)
			{
				if (!rowIsAdmin(dr))
					dr.Delete();
			}
			for (int i = 0; i < tempDT.Rows.Count; i++)
			{
				if (tempDT.Rows[i].RowState == DataRowState.Deleted)
				{
					tempDT.Rows.RemoveAt(i);
					i--;
				}
			}
			if (dataSet.Tables.Count > 1)
				dataSet.Tables.RemoveAt(1);

			dataSet.Tables.Add(tempDT);
			UsersListbox.SetBinding(ListBox.ItemsSourceProperty, "Tables[1]");
		}
		private void OnlyAdminsButton_Click(object sender, RoutedEventArgs e)
		{
			if (dataSet.Tables.Count == 1) 
				AdminsTableBuild();

			UsersListbox.SetBinding(ListBox.ItemsSourceProperty, new Binding(!showAdminsTable ? "Tables[1]" : "Tables[0]"));
			showAdminsTable = !showAdminsTable;
        }
    }
}
