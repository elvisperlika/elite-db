using MySql.Data.MySqlClient;

namespace Elite_db.operations;

public partial class CreateNewOrder : ContentPage
{
    private readonly MySqlConnection _con;
    private readonly string _mail;
    private Button _addOptionalBtn;
    private Picker _brandPicker;
    private Button _confirmSupercarBtn;
    private Picker _modelPicker;
    private Picker _optionalPicker;
    private string _orderId;
    private StackLayout _subVerticalSupercarStackLayout;
    private Picker _versionPicker;
    private string _chasssisNr;

    public CreateNewOrder(string mainPageUserMail, MySqlConnection mySqlConnection)
    {
        InitializeComponent();
        _con = mySqlConnection;
        _mail = mainPageUserMail;
        FillCostumerPicker();
    }

    private void FillBrandPicker()
    {
        try {
            _con.Open();
            var selectBrands = "SELECT NomeProduttore FROM PRODUTTORE";
            var cmd = new MySqlCommand(selectBrands, _con);
            var reader = cmd.ExecuteReader();
            while (reader.Read()) _brandPicker.Items.Add(reader["NomeProduttore"].ToString());
        }
        catch {
            Console.WriteLine("error while loading brands");
        }
        finally {
            _con.Close();
        }
    }

    private void FillCostumerPicker()
    {
        try {
            _con.Open();
            var selectCostumers = "SELECT IDBadge FROM CLIENTE " +
                                  "WHERE DataScadenza > (curdate() - 365)";
            var cmd = new MySqlCommand(selectCostumers, _con);
            var reader = cmd.ExecuteReader();
            while (reader.Read()) CostumerIdPicker.Items.Add(reader["IDBadge"].ToString());
        }
        catch (Exception exception) {
            Console.WriteLine("error while loading costumers");
            throw;
        }
        finally {
            _con.Close();
        }
    }

    private void AddSupercarBtn_Clicked(object sender, EventArgs e)
    {
        _brandPicker = new Picker {
            AutomationId = "BrandPicker",
            Title = "Select the brand"
        };

        _brandPicker.SelectedIndexChanged += BrandPicker_SelectedIndexChanged;

        _modelPicker = new Picker {
            AutomationId = "ModelPicker",
            Title = "Select the model"
        };
        FillBrandPicker();
        _modelPicker.SelectedIndexChanged += ModelPicker_SelectedIndexChanged;

        _versionPicker = new Picker {
            AutomationId = "VersionPicker",
            Title = "Select the version"
        };

        _addOptionalBtn = new Button {
            IsEnabled = true,
            AutomationId = "AddOptionalBtn",
            Text = "Add optional"
        };

        _addOptionalBtn.Clicked += AddOptionalBtn_Clicked;

        // add the elements to the horizontal stack layout
        var horizontalStackLayout = new HorizontalStackLayout {
            AutomationId = "SupercarHorizontalStackLayout",
            HorizontalOptions = LayoutOptions.Center,
            Children = {
                _brandPicker, _modelPicker, _versionPicker, _addOptionalBtn
            }
        };

        _subVerticalSupercarStackLayout = new StackLayout {
            AutomationId = "VerticalSupercarStackLayout",
            Children = {
                horizontalStackLayout
            }
        };

        _confirmSupercarBtn = new Button {
            Text = "Confirm supercar",
            HorizontalOptions = LayoutOptions.Center
        };
        _confirmSupercarBtn.Clicked += ConfirmSupercarBtn_Clicked;

        var verticalSupercarStackLayout = new StackLayout {
            AutomationId = "VerticalSupercarStackLayout",
            Children = {
                _subVerticalSupercarStackLayout, _confirmSupercarBtn
            }
        };

        SupercarStackLayout.Add(verticalSupercarStackLayout);
        AddSupercarBtn.IsEnabled = false;
    }

    private void ConfirmSupercarBtn_Clicked(object sender, EventArgs e)
    {
        _brandPicker.IsEnabled = false;
        _modelPicker.IsEnabled = false;
        _versionPicker.IsEnabled = false;
        _addOptionalBtn.IsEnabled = false;
        _confirmSupercarBtn.IsEnabled = false;
        AddSupercarBtn.IsEnabled = true;
    }

    private void ModelPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedModel = _modelPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(selectedModel)) {
            _versionPicker.Items.Clear();
            try {
                _con.Open();
                var selectVersions = "SELECT NomeVersione " +
                                     "FROM VERSIONE " +
                                     "WHERE NomeModello = @model " +
                                     "AND VERSIONE.CodOrdine IS NULL";
                var cmd = new MySqlCommand(selectVersions, _con);
                cmd.Parameters.AddWithValue("@model", selectedModel);
                var reader = cmd.ExecuteReader();
                while (reader.Read()) _versionPicker.Items.Add(reader["NomeVersione"].ToString());
            }
            catch {
                Console.WriteLine("error while loading versions");
            }
            finally {
                _con.Close();
            }
        }
        else
            DisplayAlert("Error", "You must select the brand and model first", "Ok");
    }

    private void BrandPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedBrand = _brandPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(selectedBrand))
            try {
                _con.Open();
                var selectModels = "SELECT NomeModello " +
                                   "FROM SUPERCAR " +
                                   "WHERE NomeProduttore = @brand";
                var cmd = new MySqlCommand(selectModels, _con);
                cmd.Parameters.AddWithValue("@brand", selectedBrand);
                var reader = cmd.ExecuteReader();
                while (reader.Read()) _modelPicker.Items.Add(reader["NomeModello"].ToString());
            }
            catch {
                Console.WriteLine("error while loading models");
                throw;
            }
            finally {
                _con.Close();
            }
        else
            DisplayAlert("Error", "You must select the brand first", "Ok");
    }

    private void AddOptionalBtn_Clicked(object sender, EventArgs e)
    {
        var selectedBrand = _brandPicker.SelectedItem?.ToString();
        var selectedModel = _modelPicker.SelectedItem?.ToString();
        var selectedVersion = _versionPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(selectedBrand) && !string.IsNullOrEmpty(selectedModel) &&
            !string.IsNullOrEmpty(selectedVersion)) {
            // display picker
            _brandPicker.IsEnabled = false;
            _modelPicker.IsEnabled = false;
            _versionPicker.IsEnabled = false;

            try {
                _con.Open();
                var updateVersion = "UPDATE VERSIONE " +
                                    "SET CodOrdine = @orderId " +
                                    "WHERE NomeProduttore = @brand " +
                                    "AND NomeModello = @model " +
                                    "AND NomeVersione = @version";
                var cmd = new MySqlCommand(updateVersion, _con);
                cmd.Parameters.AddWithValue("@orderId", _orderId);
                cmd.Parameters.AddWithValue("@brand", selectedBrand);
                cmd.Parameters.AddWithValue("@model", selectedModel);
                cmd.Parameters.AddWithValue("@version", selectedVersion);
                if (cmd.ExecuteNonQuery() == 1) {
                    
                    var selectVersionChassis = "SELECT NrTelaio FROM VERSIONE " +
                                               " WHERE NomeProduttore = @brand " +
                                               " AND NomeModello = @model " +
                                               " AND NomeVersione = @version";
                    cmd = new MySqlCommand(selectVersionChassis, _con);
                    cmd.Parameters.AddWithValue("@brand", selectedBrand);
                    cmd.Parameters.AddWithValue("@model", selectedModel);
                    cmd.Parameters.AddWithValue("@version", selectedVersion);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read()) {
                        _chasssisNr = reader["NrTelaio"].ToString();
                    }
                    reader.Close();
                }
            }
            catch (Exception exception) {
                Console.WriteLine(exception);
                throw;
            }
            finally {
                _con.Close();
            }

            _optionalPicker = new Picker {
                AutomationId = "OptionalPicker",
                Title = "Select the optional",
                HorizontalOptions = LayoutOptions.Center
            };
            _subVerticalSupercarStackLayout.Children.Add(_optionalPicker);
            FillOptionalPicker(_optionalPicker);
            _addOptionalBtn.IsEnabled = false;
            _optionalPicker.SelectedIndexChanged += OptionalPicker_SelectedIndexChanged;
        }
        else {
            DisplayAlert("Error", "You must select the brand, model and version first", "Ok");
        }
    }

    private void OptionalPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedOptional = _optionalPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(selectedOptional))
            try {
                _con.Open();
                var updateOptional = "UPDATE OPTIONAL_AUTO " +
                                     "SET NrTelaio = @chassisNr " +
                                     "WHERE NomeOptional = @optional";
                var cmd = new MySqlCommand(updateOptional, _con);
                cmd.Parameters.AddWithValue("@chassisNr", _chasssisNr);
                cmd.Parameters.AddWithValue("@optional", selectedOptional);
                if (cmd.ExecuteNonQuery() == 1) {
                    _optionalPicker.TextColor = Colors.Green;
                }
            }
            catch {
                Console.WriteLine("error while loading optional price");
            }
            finally {
                _con.Close();
                _optionalPicker.IsEnabled = false;
                _addOptionalBtn.IsEnabled = true;
            }
        // UPDATE THE OPTIONAL AND ENABLE THE ADD OPTIOANL BTN
    }

    private void FillOptionalPicker(Picker optionalPicker)
    {
        try {
            _con.Open();
            var selectOptionals = "SELECT NomeOptional " +
                                  "FROM OPTIONAL_AUTO, supporto " +
                                  "WHERE OPTIONAL_AUTO.CodOptional = supporto.CodOptional " +
                                  "AND supporto.NomeSegemento = (SELECT NomeSegemento " +
                                  "                             FROM SUPERCAR" +
                                  "                             WHERE SUPERCAR.NomeModello = @model)" +
                                  "AND OPTIONAL_AUTO.NrTelaio IS NULL";
            var cmd = new MySqlCommand(selectOptionals, _con);
            cmd.Parameters.AddWithValue("@model", _modelPicker.SelectedItem?.ToString());
            var reader = cmd.ExecuteReader();
            while (reader.Read()) optionalPicker.Items.Add(reader["NomeOptional"].ToString());
        }
        catch {
            Console.WriteLine("error while loading optionals");
        }
        finally {
            _con.Close();
        }
    }

    private void CalcAmountBtn_Clicked(object sender, EventArgs e)
    {
        try {
            _con.Open();
            var updateAmountQuery = "UPDATE ORDINE " +
                                    "SET ORDINE.Importo = ((SELECT SUM(VERSIONE.Prezzo) " +
                                                            "FROM VERSIONE " +
                                                            "WHERE VERSIONE.CodOrdine = @orderId) " +
                                                            "+ (SELECT SUM(OPTIONAL_AUTO.Prezzo) " +
                                                                    "FROM OPTIONAL_AUTO " +
                                                                        "WHERE OPTIONAL_AUTO.NrTelaio IN (SELECT VERSIONE.NrTelaio " +
                                                                        "FROM VERSIONE " +
                                                                        "WHERE VERSIONE.CodOrdine = @orderId))) " +
                                    "WHERE ORDINE.CodOrdine = @orderId";
            var cmd = new MySqlCommand(updateAmountQuery, _con);
            cmd.Parameters.AddWithValue("@orderId", _orderId);
            if (cmd.ExecuteNonQuery() == 1) {
                var selectAmount = "SELECT Importo " +
                                   "FROM ORDINE " +
                                   "WHERE ORDINE.CodOrdine = @orderId ";
                cmd = new MySqlCommand(selectAmount, _con);
                cmd.Parameters.AddWithValue("@orderId", _orderId);
                var reader = cmd.ExecuteReader();
                while (reader.Read()) 
                    TotalLabel.Text = reader["Importo"].ToString();
                reader.Close();
            }
        }
        catch {
            Console.WriteLine("error while calculating amount");
            throw;
        }
        finally {
            _con.Close();
        }
    }

    private void ConfirmSelectionBtn_Clicked(object sender, EventArgs e)
    {
        var selectedCostumer = CostumerIdPicker.SelectedItem?.ToString();
        if (!string.IsNullOrEmpty(selectedCostumer))
            try {
                _con.Open();
                var insertOrderQuery = "INSERT INTO ORDINE(Importo, DataOrdine, EmailAziendale, IDBadge) " +
                                       "VALUES(0, curdate(), @mail, @idBadge)";
                var cmd = new MySqlCommand(insertOrderQuery, _con);
                cmd.Parameters.AddWithValue("@mail", _mail);
                cmd.Parameters.AddWithValue("@idBadge", selectedCostumer);
                if (cmd.ExecuteNonQuery() == 1) {
                    var selectOrderIdQuery = "SELECT CodOrdine " +
                                             "FROM ORDINE " +
                                             "ORDER BY CodOrdine DESC " +
                                             "LIMIT 1";
                    cmd = new MySqlCommand(selectOrderIdQuery, _con);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read()) _orderId = reader["CodOrdine"].ToString();
                }
            }
            catch (Exception exception) {
                Console.WriteLine(exception);
                throw;
            }
            finally {
                _con.Close();
                CostumerIdPicker.IsEnabled = false;
                ConfirmSelectionBtn.IsEnabled = false;
            }
    }
}