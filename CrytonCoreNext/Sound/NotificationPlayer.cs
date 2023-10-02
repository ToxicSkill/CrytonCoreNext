using Microsoft.Win32;
using System;
using System.Media;

namespace CrytonCoreNext.Sound
{
    public static class NotificationPlayer
    {
        private const string NotificationPath = @"AppEvents\Schemes\Apps\.Default\Notification.Default\.Current";

        public static void PlayNotificationSound()
        {
            bool found = false;
            try
            {
                using RegistryKey key = Registry.CurrentUser.OpenSubKey(NotificationPath);
                if (key != null)
                {
                    var o = key.GetValue(null); // pass null to get (Default)
                    if (o != null)
                    {
                        var theSound = new SoundPlayer((String)o);
                        theSound.Play();
                        found = true;
                    }
                }
            }
            catch
            { }
            if (!found)
            {
                SystemSounds.Beep.Play(); // consolation prize
            }
        }
    }
}
