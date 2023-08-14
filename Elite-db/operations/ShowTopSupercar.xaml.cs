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
            var selectSegmentQuery = "SELECT NomeSegemento FROM SEGMENTO";
            var cmdSegment = new MySqlCommand(selectSegmentQuery, _con);
            
            var readerSegment = cmdSegment.ExecuteReader();
            while (readerSegment.Read()) {
                DropdownPicker.Items.Add(readerSegment["NomeSegemento"].ToString());
            }
            readerSegment.Close();
        }
        catch (Exception e) {
            Console.WriteLine("Error while loading segments");
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
            string selectSupercars = "SELECT PRODUTTORE.NomeProduttore, SUPERCAR.NomeModello, SUPERCAR.CavalliPotenza " +
                                     "FROM SUPERCAR, PRODUTTORE " +
                                     "WHERE SUPERCAR.NomeSegemento = @segment " +
                                     "AND SUPERCAR.NomeProduttore = PRODUTTORE.NomeProduttore " +
                                     "ORDER BY CavalliPotenza DESC";
            var cmd = new MySqlCommand(selectSupercars, _con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@segment", selectedItem);
        
            var dataList = new ObservableCollection<ModelRowData>();
            var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                var row = new ModelRowData {
                    Company = reader["NomeProduttore"].ToString(),
                    Model = reader["NomeModello"].ToString(),
                    HorsePower = reader["CavalliPotenza"].ToString()
                };
                dataList.Add(row);
            }
            DataListViewSupercars.ItemsSource = dataList;
            reader.Close();
            
            string avarageQuery = "SELECT AVG(CavalliPotenza) AS AvarageHorsePower " +
                                  "FROM SUPERCAR " +
                                  "WHERE NomeSegemento = @segment";
            var cmdAvarage = new MySqlCommand(avarageQuery, _con);
            cmdAvarage.CommandType = CommandType.Text;
            cmdAvarage.Parameters.AddWithValue("@segment", selectedItem);
            var avarage = cmdAvarage.ExecuteScalar().ToString();
            HorsePowerAvarage.Text = avarage;
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
    public string Company { get; set; }
    public string Model { get; set; }
    public string HorsePower { get; set; }
}