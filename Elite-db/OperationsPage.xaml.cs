using MySql.Data.MySqlClient;

namespace Elite_db;

public partial class OperationsPage : ContentPage
{
    public OperationsPage()
    {
        InitializeComponent();
    }

    private void InsertNewCustomerClicked(object sender, EventArgs e)
    {
        var InsertNewCustomerPage = new InsertNewCostumer();
        ContentPresenter.Content = InsertNewCustomerPage.Content;
    }

    private void ViewCustomersOrderClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ShowOptionalsCompanyClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void InsertNewOrderClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void InsertConsignmentClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ShowEmployeesWithBonusClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ShowTop10SupercarClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void InsertNewSupercarVersionClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void InsertMaintananceClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void ShowOldestCostumersClicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}