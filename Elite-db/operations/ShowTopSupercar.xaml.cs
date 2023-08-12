using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class ShowTopSupercar : ContentPage
{
    private readonly MySqlConnection _con;
    public ShowTopSupercar(MySqlConnection mySqlConnection)
    {
        InitializeComponent();
        _con = mySqlConnection;
        try {
            _con.Open();
            var selectSegmentQuery = "SELECT Nome FROM SEGMENTO";
            var cmdSegment = new MySqlCommand(selectSegmentQuery, _con);
            var readerSegment = cmdSegment.ExecuteReader();
            while (readerSegment.Read()) {
                DropdownPicker.Items.Add(readerSegment["Nome"].ToString());
            }
            readerSegment.Close();
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
        finally {
            _con.Close();
        }
    }

    private void DropdownPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedItem = DropdownPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(selectedItem)) {
            showSupercars(selectedItem);
        }
    }

    private void showSupercars(string selectedItem)
    {
        try {
            _con.Open();
            string selectSupercars = "SELECT PRODUTTORE.Nome_Produttore, SUPERCAR.Nome_Modello, SUPERCAR.Cavalli_Potenza " +
                                     "FROM SUPERCAR, PRODUTTORE " +
                                     "WHERE SUPERCAR.Nome = @segment " +
                                     "AND SUPERCAR.P_IVA = PRODUTTORE.P_IVA " +
                                     "ORDER BY Cavalli_Potenza DESC";
            var cmd = new MySqlCommand(selectSupercars, _con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@segment", selectedItem);
        
            var dataList = new ObservableCollection<ModelRowData>();
            var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                dataList.Add(new ModelRowData(reader["Nome_Produttore"].ToString(),
                    reader["Nome_Modello"].ToString(), reader["Cavalli_Potenza"].ToString()));
            }
            DataListViewSupercars.ItemsSource = dataList;
            reader.Close();
        }
        catch (Exception e) {
            Console.WriteLine(e);
            throw;
        } 
        finally {
            _con.Close();
        }
        
    }
}

internal class ModelRowData
{
    public string Company { get; }
    public string Model { get; }
    public string HorsePower { get; }

    public ModelRowData(string company, string model, string horsePower)
    {
        Company = company;
        Model = model;
        HorsePower = horsePower;
    }
}