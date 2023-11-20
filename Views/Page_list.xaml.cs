using PM2Examen2Grupo1.Controllers;
using PM2Examen2Grupo1.Models;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace PM2Examen2Grupo1.Views;

public partial class Page_list : ContentPage
{

    public ObservableCollection<Sitios> Items
    {
        get; set;
    }

    public Page_list(){
		InitializeComponent();

        Items = new ObservableCollection<Sitios>();

        this.BindingContext = this;
        load_record_date();
    }

    private async Task load_record_date()
    {
        try
        {
            var services_bd = await get_date_records();

            if (services_bd != null)
            {
                foreach (var data in services_bd)
                {
                    Items.Add(data);
                }
            }

        }
        catch (Exception ex)
        {
            await DisplayAlert("Advertencia", "error: " + ex.ToString(), "OK");
        }
    }

    private async Task<List<Sitios>> get_date_records()
    {
        Sitios data = new Sitios();
        string response = "";

        try
        {
            Metodos insert = new Metodos();
            response = await Task.Run(() => insert.select_async(data, RestApi.select_sitios));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Advertencia", "" + ex, "OK");
        }

        if (response != "" && response != null)
        {
            List<Sitios> list = JsonSerializer.Deserialize<List<Sitios>>(response);

            if (list.Count > 0)
            {
                return list;
            }
        }

        return null;
    }
}