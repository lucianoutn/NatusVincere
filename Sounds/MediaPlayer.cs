using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace AlumnoEjemplos.MiGrupo.Sounds
{
    class MediaPlayer
    {

        #region DLLs externas

        [DllImport("winmm.dll")]
        public static extern int mciSendString(string lpstrCommand,
        StringBuilder lpstrReturnString, int uReturnLengh, int hwndCallback);

        [DllImport("winmm.dll")]
        public static extern int mciGetErrorString(int fwdError, StringBuilder lpszErrorText,
        int cchErrorText);

        [DllImport("winmm.dll")]
        public static extern int waveOutGetNumDevs();

        [DllImport("kernel32.dll")]
        public static extern int GetShortPathName(string lpszLongPath,
        StringBuilder lpszShortPath, int cchBuffer);

        [DllImport("kernel32.dll")]
        public static extern int GetLongPathName(string
        lpszShortPath, StringBuilder lpszLongPath, int cchBuffer);

        #endregion

        // Constante con la longitud máxima de un nombre de archivo.     
        private const int MAX_PATH = 260;

        // Constante con el formato de archivo a reproducir.
        private const string WINMM_FILE_TYPE = "MPEGVIDEO";
       
        /// <summary>
        /// Inicia la reproducción del archivo MP3.
        /// <param name="playLoop">True para reproducir en loop</param>
        /// </summary>
        public void openAndPlay(bool playLoop, String soundPath, String alias)
        {
            mciSendString("open " + NombreCorto(soundPath) + " type " + WINMM_FILE_TYPE +
            " alias " + alias, null, 0, 0);

            StringBuilder command = new StringBuilder("play ");
            command.Append(alias);
            if (playLoop)
            {
                command.Append(" REPEAT");
            }

            int mciResul = mciSendString(command.ToString(), null, 0, 0);

        }
   
        /// <summary>
        /// Pausa la reproducción en proceso.
        /// </summary>
        public void pause(String alias)
        {
            // Enviamos el comando de pausa mediante la función mciSendString,
            int mciResul = mciSendString("pause " + alias, null, 0, 0);          
        }
        /// <summary>
        /// Continúa con la reproducción actual.
        /// </summary>
        public void resume(String alias)
        {
            // Enviamos el comando de pausa mediante la función mciSendString,
            int mciResul = mciSendString("resume " + alias, null, 0, 0); 
        }

        /// <summary>
        /// Detiene la reproducción actual y cierra el archivo de audio.
        /// </summary>
        public void closeFile(String aliasSound)
        {
            // Establecemos los comando detener reproducción y cerrar archivo.
            mciSendString("stop " + aliasSound, null, 0, 0);
            mciSendString("close " + aliasSound, null, 0, 0);
        }
        
        /// <summary>
        /// Método para convertir un nombre de archivo largo en uno corto,
        /// necesario para usarlo como parámetro de la función MciSendString.
        /// </summary>
        /// <param name="nombreLargo">Nombre y ruta del archivo a convertir.</param>
        /// <returns>Nombre corto del archivo especificado.</returns>
        private string NombreCorto(string NombreLargo)
        {
            // Creamos un buffer usando un constructor de la clase StringBuider.
            StringBuilder sBuffer = new StringBuilder(MAX_PATH);
            // intentamos la conversión del archivo.
            if (GetShortPathName(NombreLargo, sBuffer, MAX_PATH) > 0)
                // si la función ha tenido éxito devolvemos el buffer formateado
                // a tipo string.
                return sBuffer.ToString();
            else // en caso contrario, devolvemos una cadena vacía.
                return "";
        }

        /// <summary>
        /// Método que convierte un nombre de archivo corto, en uno largo.
        /// </summary>
        /// <param name="NombreCorto">Nombre del archivo a convertir.</param>
        /// <returns>Cadena con el nombre de archivo resultante.</returns>
        public string NombreLargo(string NombreCorto)
        {
            StringBuilder sbBuffer = new StringBuilder(MAX_PATH);
            if (GetLongPathName(NombreCorto, sbBuffer, MAX_PATH) > 0)
                return sbBuffer.ToString();
            else
                return "";
        }
        
    }
}

