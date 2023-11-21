using CommunityToolkit.Maui.Views;
using PM2Examen2Grupo1.Controllers;
using PM2Examen2Grupo1.Models;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace PM2Examen2Grupo1.Views;

public partial class Page_list:ContentPage {
    public static int id = 0;
    public static string url_video, url_audio, descripcion;
    public static double latitud, longitud;

    public ObservableCollection<Sitios> Items
    {
        get; set;
    }

    public Page_list() {
        InitializeComponent();

        Items=new ObservableCollection<Sitios>();

        this.BindingContext=this;
        load_record_date();
    }

    private async Task load_record_date() {
        try {
            var services_bd = await get_date_records();

            if(services_bd!=null) {
                foreach(var data in services_bd) {
                    Items.Add(data);
                }
            }

        } catch(Exception ex) {
            await DisplayAlert("Advertencia","error: "+ex.ToString(),"OK");
        }
    }

    private async Task<List<Sitios>> get_date_records() {
        Sitios data = new Sitios();
        string response = "";

        try {
            Metodos insert = new Metodos();
            response=await Task.Run(() => insert.select_async(data,RestApi.select_sitios));
        } catch(Exception ex) {
            await DisplayAlert("Advertencia",""+ex,"OK");
        }

        if(response!=""&&response!=null) {
            List<Sitios> list = JsonSerializer.Deserialize<List<Sitios>>(response);

            if(list.Count>0) {
                return list;
            }
        }

        return null;
    }

    private async void action(object sender,TappedEventArgs e) {
        var stackLayout = (StackLayout) sender;

        var item = (Sitios) stackLayout.BindingContext;

        if(item!=null) {
            id=item.Id;
            url_video=item.VideoDigital;
            url_audio=item.AudioFile;
            latitud=item.Latitud;
            longitud=item.Longitud;
            descripcion=item.Descripcion;

            await DisplayAlert("Mensaje","Seleccionaste el Sitio #: "+id,"OK");
        }
    }

    private async void delete_Clicked(object sender,EventArgs e) {
        if(await verification_id()) {
            await delete();
        }
    }

    private async Task<bool> verification_id() {
        if(id!=0) {
            return true;
        } else {
            await DisplayAlert("Advertencia","Elige un item","OK");
        }

        return false;
    }

    private async Task delete() {

        Sitios users = new Sitios();
        users.Id=id;

        string response = "";

        try {
            Metodos insert = new Metodos();
            response=await Task.Run(() => insert.insert_update_async(users,RestApi.delete));
        } catch(Exception ex) {
            await DisplayAlert("Advertencia",""+ex,"OK");
        }

        if(response=="exitoso") {
            await DisplayAlert("Exitoso","Tu email se ha cambiado exitosamente","OK");
            Items.Clear();
            await load_record_date();
        } else {
            await DisplayAlert("Advertencia","No se modifico contraseña: "+response,"OK");
        }
    }

    private async void update_Clicked(object sender,EventArgs e) {
        if(await verification_id()) {
            await Navigation.PushAsync(new Page_update());
        }
    }

    private async void view_map_Clicked(object sender,EventArgs e) {
        if(await verification_id()) {
            await Navigation.PushAsync(new Page_map());
        }
    }

    private async void view_video_Clicked(object sender,EventArgs e) {
        if(await verification_id()) {
            await Navigation.PushAsync(new Page_video());
        }
    }

    private async void view_audio_Clicked(object sender,EventArgs e) {
        if(await verification_id()) {
            
            if(!string.IsNullOrEmpty(url_audio)) {
                ReproducirAudio();
            } else {
                await DisplayAlert("Advertencia","No hay URL de audio disponible.","OK");
            }

        }
    }

    private void ReproducirAudio() {
        MediaElement mediaElement = new MediaElement {
            Source=url_audio,
            ShouldAutoPlay=true
        };

        container.Add(mediaElement);
    }
}