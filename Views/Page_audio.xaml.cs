using Android.Media;
using CommunityToolkit.Maui.Views;
using Plugin.Maui.Audio;

namespace PM2Examen2Grupo1.Views;

public partial class Page_audio : ContentPage
{
    public Page_audio()
	{
		InitializeComponent();
        MediaPlayer mediaPlayer = new MediaPlayer();
        mediaPlayer.SetAudioAttributes(new AudioAttributes.Builder()
                                       .SetContentType(AudioContentType.Music)
                                       .SetUsage(AudioUsageKind.Media)
                                       .Build());
        mediaPlayer.SetDataSource(Page_list.url_audio); // Reemplaza con tu URL
        mediaPlayer.PrepareAsync();
        mediaPlayer.Start();
    }
}