using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using AlumnoEjemplos.MiGrupo.Sounds;
namespace AlumnoEjemplos.NatusVincere
{
    class Sounds
    {
        MediaPlayer mediaPlayer = new MediaPlayer();
        String musicPath = "AlumnoEjemplos\\NatusVincere\\music.mp3";
        String windPath = "AlumnoEjemplos\\NatusVincere\\wind.mp3";
        String victoriaPath = "AlumnoEjemplos\\NatusVincere\\victoria.mp3";
        String gameOverPath = "AlumnoEjemplos\\NatusVincere\\gameOver.mp3";
        String talarArbolPath = "AlumnoEjemplos\\NatusVincere\\talarArbol.mp3";

        String musicAlias = "music";
        String windAlias = "alias";

        public void playMusic()
        {
            mediaPlayer.FileName = musicPath;
            mediaPlayer.play(true, musicPath, musicAlias);
        }

        public void playViento()
        {
            mediaPlayer.FileName = windPath;
            mediaPlayer.play(true, windPath, windAlias);
        
        }

        public void playVictoria()
        {
            ;
        }

        public void playGameOver()
        {
            ;
        }

        public void playTalarArbol()
        {
            ;
        }
    }
}
