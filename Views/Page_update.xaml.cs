namespace PM2Examen2Grupo1.Views;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Storage;
using Plugin.Maui.Audio;
using PM2Examen2Grupo1.Controllers;
using PM2Examen2Grupo1.Models;
using PM2Examen2Grupo1.Views;
public partial class Page_update:ContentPage 
{

    string videoPath;
    string authdomain = "examenpm2-ab397.firebaseapp.com";
    string apikey = "AIzaSyC72lTS4pIJJb27_3LmRfX1cqfQPncR2qI";
    string email = "cdarwin0426@gmail.com";
    string password = "12345678";
    string token = string.Empty;
    string rutastorage = "examenpm2-ab397.appspot.com";
    string lblaudio;
    string lblvideo;

    //Audio
    private readonly IAudioRecorder _audioRecorder;
    private bool isRecording = false;
    public string pathaudio, filename;

    public Page_update() {
        InitializeComponent();
        _audioRecorder = AudioManager.Current.CreateRecorder();
        MainThread.BeginInvokeOnMainThread(new Action(async () => await obtenerToken()));
    }

    private async Task obtenerToken()
    {
        var client = new FirebaseAuthClient(new FirebaseAuthConfig()
        {
            ApiKey = apikey,
            AuthDomain = authdomain,
            Providers = new FirebaseAuthProvider[]
            {
                    new EmailProvider()
            }
        });

        var credenciales = await client.SignInWithEmailAndPasswordAsync(email, password);
        token = await credenciales.User.GetIdTokenAsync();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        txtDescripcionUpd.Text = Page_list.descripcion;
        txtLatitudUpd.Text = Page_list.latitud.ToString();
        txtLongitudUpd.Text = Page_list.longitud.ToString();
        viewVideoUpd.Source = Page_list.url_video;
        lblUrlVideoUpd.Text = Page_list.url_video;
        lblUrlAudioUpd.Text = Page_list.url_audio;

    }


    private async Task CheckAndRequestLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (status == PermissionStatus.Granted)
        {
            await GetLocationAsync();
        }
        else if (status == PermissionStatus.Denied)
        {
            await DisplayAlert("Advertencia", "Esta aplicacion no puede funcionar si no tiene los permisos", "OK");
        }
    }

    public async Task GetLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            var location = await Geolocation.GetLocationAsync(request);

            if (location != null)
            {
                txtLatitudUpd.Text = "" + location.Latitude;
                txtLongitudUpd.Text = "" + location.Longitude;
            }

        }
        catch (FeatureNotSupportedException fnsEx)
        {
            await DisplayAlert("Advertencia", fnsEx + "", "OK");
        }
        catch (PermissionException Ex)
        {
            await DisplayAlert("Advertencia", "Esta aplicacion no puede funcionar si no tiene los permisos", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Advertencia", ex + "", "OK");
        }
    }

    public async Task<bool> CheckAndRequestPermissionAsync<T>() where T : Permissions.BasePermission, new()
    {
        var status = await Permissions.CheckStatusAsync<T>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<T>();
            if (status != PermissionStatus.Granted)
            {
                // Permiso denegado
                return false;
            }
        }
        // Permiso otorgado
        return true;
    }


    private async void btnGabarVideoUpd_Clicked(object sender, EventArgs e)
    {
        var video = await MediaPicker.CaptureVideoAsync();

        if (video != null)
        {
            var task = new FirebaseStorage(
                rutastorage,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(token),
                    ThrowOnCancel = true
                }
                )
                .Child("Videos")
                .Child(video.FileName)
                .PutAsync(await video.OpenReadAsync());

            var urlDescarga = await task;
            lblvideo = urlDescarga;
            lblUrlVideoUpd.Text = urlDescarga;
            viewVideoUpd.Source = urlDescarga;
        }
    }

    private async void btnGrabarAudioUpd_Clicked(object sender, EventArgs e)
    {
        if (!isRecording)
        {
            var permiso = await Permissions.RequestAsync<Permissions.Microphone>();
            var permiso1 = await Permissions.RequestAsync<Permissions.StorageRead>();
            var permiso2 = await Permissions.RequestAsync<Permissions.StorageWrite>();

            if (permiso != PermissionStatus.Granted || permiso1 != PermissionStatus.Granted || permiso2 != PermissionStatus.Granted)
            {
                return;
            }
            await _audioRecorder.StartAsync();
            isRecording = true;
            btnGrabarAudioUpd.Text = "Grabando";
            Console.WriteLine("Iniciando grabación...");
        }
        else
        {
            var recordedAudio = await _audioRecorder.StopAsync();

            if (recordedAudio != null)
            {
                try
                {
                    filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DateTime.Now.ToString("ddMMyyyymmss") + "_VoiceNote.wav");

                    using (var fileStorage = new FileStream(filename, FileMode.Create, FileAccess.Write))
                    {
                        recordedAudio.GetAudioStream().CopyTo(fileStorage);
                    }

                    pathaudio = filename;

                    var task = new FirebaseStorage(
                        rutastorage,
                        new FirebaseStorageOptions
                        {
                            AuthTokenAsyncFactory = () => Task.FromResult(token),
                            ThrowOnCancel = true
                        }
                    )
                    .Child("Audios")
                    .Child(Path.GetFileName(pathaudio))
                    .PutAsync(File.OpenRead(pathaudio));

                    var urlDescarga = await task;
                    lblaudio = urlDescarga;
                    lblUrlAudioUpd.Text = urlDescarga;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    await DisplayAlert("Error", "Ocurrió un error al procesar la grabación.", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Error", "La grabación de audio ha fallado.", "Ok");
            }
            isRecording = false;
            btnGrabarAudioUpd.Text = "Grabar Audio";
            Console.WriteLine("Deteniendo grabación y guardando el audio...");
        }
    }
    private string GetVideoPath(Stream stream)
    {
#if __ANDROID__
        var publicDirectoryPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
        var videoPath = Path.Combine(publicDirectoryPath, "video.mp4");

        using (var fileStream = File.Create(videoPath))
        {
            stream.CopyTo(fileStream);
        }

        return videoPath;
#else
            return string.Empty;
#endif
    }


    private async void btnSalvarUbicacionUpd_Clicked(object sender, EventArgs e)
    {
            //Actualizar Sitio
            await Update_Site();
    }


    private async Task Update_Site()
    {
        if (string.IsNullOrEmpty(txtDescripcionUpd.Text) || string.IsNullOrEmpty(txtLatitudUpd.Text) || string.IsNullOrEmpty(txtLongitudUpd.Text) || string.IsNullOrEmpty(lblUrlVideoUpd.Text) || string.IsNullOrEmpty(lblUrlAudioUpd.Text))
        {
            await DisplayAlert("Advertencia", "Todos los campos son obligatorios", "OK");
            return;
        }

        Sitios users = new Sitios();
        users.Id = Page_list.id;
        users.Descripcion = txtDescripcionUpd.Text;
        users.Latitud = Convert.ToDouble(txtLatitudUpd.Text);
        users.Longitud = Convert.ToDouble(txtLongitudUpd.Text);
        users.VideoDigital = lblUrlVideoUpd.Text;
        users.AudioFile = lblUrlAudioUpd.Text;
        string response = "";

        try
        {
            Metodos update = new Metodos();
            response = await Task.Run(() => update.insert_update_async(users, RestApi.update));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Advertencia", "" + ex, "OK");
        }



        if (response == "exitoso")
        {
            await DisplayAlert("Exitoso", "Los datos se han actualizado con exito", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Advertencia", "No se pudieron actualizar los datos: " + response, "OK");
        }
    }

    private async void btnObtenerNuevaUbicacion_Clicked(object sender, EventArgs e)
    {
        var connection = Connectivity.NetworkAccess;

        if (connection == NetworkAccess.Internet)
        {
            await GetLocationAsync();
        }
        else
        {
            await DisplayAlert("Sin Acceso a Internet", "Por favor, revisa tu conexión a internet para continuar.", "OK");
        }
    }

}