using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
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

        // Alias asignado al archivo especificado.
        private const string WINMM_FILE_ALIAS = "TgcMp3MediaFile";
        
        private string fileName;
        /// <summary>
        /// Path del archivo a reproducir
        /// </summary>
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        /// <summary>
        /// Estados del reproductor
        /// </summary>
        public enum States
        {
            Open,
            Playing,
            Paused,
            Stopped,
        }

        /// <summary>
        /// Inicia la reproducción del archivo MP3.
        /// <param name="playLoop">True para reproducir en loop</param>
        /// </summary>
        public void play(bool playLoop, String soundPath, String alias)
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
        public void pause()
        {
            // Enviamos el comando de pausa mediante la función mciSendString,
            int mciResul = mciSendString("pause " + WINMM_FILE_ALIAS, null, 0, 0);
            if (mciResul != 0)
            {
                throw new Exception(MciMensajesDeError(mciResul));
            }
        }
        /// <summary>
        /// Continúa con la reproducción actual.
        /// </summary>
        public void resume()
        {
            // Enviamos el comando de pausa mediante la función mciSendString,
            int mciResul = mciSendString("resume " + WINMM_FILE_ALIAS, null, 0, 0);
            if (mciResul != 0)
            {
                throw new Exception(MciMensajesDeError(mciResul));
            }
        }

        /// <summary>
        /// Detiene la reproducción del archivo de audio.
        /// </summary>
        public void stop()
        {
            // Detenemos la reproducción, mediante el comando adecuado.
            mciSendString("stop " + WINMM_FILE_ALIAS, null, 0, 0);
        }

        /// <summary>
        /// Detiene la reproducción actual y cierra el archivo de audio.
        /// </summary>
        public void closeFile()
        {
            // Establecemos los comando detener reproducción y cerrar archivo.
            mciSendString("stop " + WINMM_FILE_ALIAS, null, 0, 0);
            mciSendString("close " + WINMM_FILE_ALIAS, null, 0, 0);
        }


        /// <summary>
        /// Obtiene el estado de la reproducción en proceso.
        /// </summary>
        /// <returns>Estado del reproducto</returns>
        public States getStatus()
        {
            StringBuilder sbBuffer = new StringBuilder(MAX_PATH);
            // Obtenemos la información mediante el comando adecuado.
            mciSendString("status " + WINMM_FILE_ALIAS + " mode", sbBuffer, MAX_PATH, 0);
            string strState = sbBuffer.ToString();

            if (strState == "playing")
            {
                return States.Playing;
            }
            if (strState == "paused")
            {
                return States.Paused;
            }
            if (strState == "stopped")
            {
                return States.Stopped;
            }
            return States.Open;
        }

        /// <summary>
        /// Mueve el apuntador de archivo al inicio del mismo.
        /// </summary>
        public void goToBeginning()
        {
            // Establecemos la cadena de comando para mover el apuntador del archivo,
            // al inicio de este.
            int mciResul = mciSendString("seek " + WINMM_FILE_ALIAS + " to start", null, 0, 0);
            if (mciResul != 0)
            {
                throw new Exception(MciMensajesDeError(mciResul));
            }
        }
        /// <summary>
        /// Mueve el apuntador de archivo al final del mismo.
        /// </summary>
        public void goToEnd()
        {
            // Establecemos la cadena de comando para mover el apuntador del archivo,
            // al final de este.
            int mciResul = mciSendString("seek " + WINMM_FILE_ALIAS + " to end", null, 0, 0);
            if (mciResul != 0)
            {
                throw new Exception(MciMensajesDeError(mciResul));
            }
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

        /// <summary>
        /// Método para convertir los mensajes de error numéricos, generados por la
        /// función mciSendString, en su correspondiente cadena de caracteres.
        /// </summary>
        /// <param name="ErrorCode">Código de error devuelto por la función 
        /// mciSendString</param>
        /// <returns>Cadena de tipo string, con el mensaje de error</returns>
        private string MciMensajesDeError(int ErrorCode)
        {
            // Creamos un buffer, con suficiente espacio, para almacenar el mensaje
            // devuelto por la función.
            StringBuilder sbBuffer = new StringBuilder(MAX_PATH);
            // Obtenemos la cadena de mensaje.
            if (mciGetErrorString(ErrorCode, sbBuffer, MAX_PATH) != 0)
                // Sí la función ha tenido éxito, valor devuelto diferente de 0,
                // devolvemos el valor del buffer, formateado a string.
                return sbBuffer.ToString();
            else // si no, devolvemos una cadena vacía.
                return "";
        }

    }
}

