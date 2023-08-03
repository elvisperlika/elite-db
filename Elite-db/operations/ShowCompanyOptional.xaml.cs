using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class ShowCompanyOptional : ContentPage
{
    public ShowCompanyOptional()
    {
        InitializeComponent();
    }

    private void SearchOptionalsClicked(object sender, EventArgs e)
    {
        var companyName = CompanyNameBox.Text;
        
        MySqlConnection con = new("SERVER=localhost; DATABASE=ElegantMotors; " +
                                  "UID=root; PASSWORD=Elvis101");
        
        try {
            con.Open();

            var selectQuery =
                "SELECT Nome_Optional " +
                "FROM OPTIONAL_AUTO, PRODUTTORE " +
                "WHERE PRODUTTORE.Nome = @companyName " +
                "AND OPTIONAL_AUTO.P_IVA = PRODUTTORE.P_IVA";

            var cmd = new MySqlCommand(selectQuery, con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@companyName", companyName);

            var dataList = new ObservableCollection<RowData>();

            var reader = cmd.ExecuteReader();
            while (reader.Read())
                dataList.Add(new RowData(reader["Nome_Optional"].ToString()));

            dataListView.ItemsSource = dataList;
            reader.Close();
        }
        catch {
            SearchButton.Text = "Error, retry!";
        }
        finally {
            con.Close();
        }
        
        throw new NotImplementedException();
    }
    
    public class RowData
    {
        public string OptionalName { get; }

        public RowData(string optionalName)
        {
            OptionalName = optionalName;
        }
    }
}