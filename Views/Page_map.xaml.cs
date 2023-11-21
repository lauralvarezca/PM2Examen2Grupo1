namespace PM2Examen2Grupo1.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public partial class Page_map:ContentPage {
    public Page_map() {
        InitializeComponent();
    }

    protected async override void OnAppearing() {
        base.OnAppearing();

        var connection = Connectivity.Current.NetworkAccess;

        if(connection==NetworkAccess.Internet) {
            bool isLocationPermissionGranted = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>()==PermissionStatus.Granted;

            if(!isLocationPermissionGranted) {
                isLocationPermissionGranted=await Permissions.RequestAsync<Permissions.LocationWhenInUse>()==PermissionStatus.Granted;
            }

            if(isLocationPermissionGranted) {
                var geolocationRequest = new GeolocationRequest(GeolocationAccuracy.Medium);
                var userLocation = await Geolocation.GetLocationAsync(geolocationRequest);

                if(userLocation!=null) {
                    var pinEstatico = new Pin {
                        Type=PinType.Place,
                        Location=new Location(Page_list.latitud,Page_list.longitud),
                        Label=Page_list.descripcion,
                    };

                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(Page_list.latitud,Page_list.longitud),Distance.FromKilometers(1)));
                    map.Pins.Add(pinEstatico);
                    map.IsShowingUser=true;
                } else {
                    await DisplayAlert("Adevertencia","Ubicacion no disponible","OK");
                }
            }

        } else {
            await DisplayAlert("Sin Acceso a internet","Por favor, revisa tu conexion a internet para continuar.","OK");
        }
    }


    public async void Button_Clicked(object sender,EventArgs e) {
        var googleMapsUrl = $"https://www.google.com/maps/dir/?api=1&travelmode=driving&destination={Page_list.latitud},{Page_list.longitud}";

        try {
            await Launcher.OpenAsync(googleMapsUrl);
        } catch(Exception ex) {
            await DisplayAlert("Error","No se pudo abrir Google Maps: "+ex.Message,"OK");
        }
    }
}