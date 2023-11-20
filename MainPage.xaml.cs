using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Storage;

using Microsoft.Maui.Maps;
using Plugin.Maui.Audio;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Storage;
using PM2Examen2Grupo1.Models;
using PM2Examen2Grupo1.Controllers;
using PM2Examen2Grupo1.Views;

namespace PM2Examen2Grupo1
{
    public partial class MainPage : ContentPage
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
        public MainPage()
        {
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
                    txtLatitud.Text = "" + location.Latitude;
                    txtLongitud.Text = "" + location.Longitude;
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




        private void OnCounterClicked(object sender, EventArgs e)
        {
           
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {

        }

        private void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {

        }

        //public async Task<bool> CheckAndRequestStoragePermission()
        //{
        //    var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
        //    if (status != PermissionStatus.Granted)
        //    {
        //        status = await Permissions.RequestAsync<Permissions.StorageWrite>();
        //    }

        //    return status == PermissionStatus.Granted;
        //}

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


        private async void btnGabarVideo_Clicked(object sender, EventArgs e)
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
                lblUrl.Text = urlDescarga;
                viewVideo.Source = urlDescarga;
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

        private async void btnSalvarUbicacion_Clicked(object sender, EventArgs e)
        {
            await insert_user();

        }

        private async Task insert_user() {
            Sitios users = new Sitios();
            users.Descripcion = txtDescripcion.Text;
            users.Latitud = Convert.ToDouble(txtLatitud.Text);
            users.Longitud = Convert.ToDouble(txtLongitud.Text);
            users.VideoDigital = lblvideo;
            users.AudioFile = lblaudio;
            string response = "";
             
             try {
                 Metodos insert = new Metodos();
                 response=await Task.Run(() => insert.insert_update_async(users, RestApi.insert));
             } catch(Exception ex) {
                 await DisplayAlert("Advertencia",""+ex,"OK");
             }

          

             if(response=="exitoso") {
                 await DisplayAlert("Exitoso","Tu cuenta se ha creado exitosamente, debes esperar unas minutos o horas para darte acceso","OK");
             } else {
                 await DisplayAlert("Advertencia","No se inserto usuario: "+response,"OK");
             }
         }

        private async void btnGrabarAudio_Clicked(object sender, EventArgs e)
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
                btnGrabarAudio.Text = "Grabando";
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
                        lblUrl.Text = urlDescarga;
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
                btnGrabarAudio.Text = "Grabar Audio";
                Console.WriteLine("Deteniendo grabación y guardando el audio...");
            }
        }

        private async void btnUbicacionSalvadas_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Page_list());
        }

    }
}