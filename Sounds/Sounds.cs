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
        String happyMusicPath = "AlumnoEjemplos\\NatusVincere\\happy.mp3";
        String windPath = "AlumnoEjemplos\\NatusVincere\\wind.mp3";
        String victoriaPath = "AlumnoEjemplos\\NatusVincere\\victory.mp3";
        String gameOverPath = "AlumnoEjemplos\\NatusVincere\\gameOver.mp3";
        String talarArbolPath = "AlumnoEjemplos\\NatusVincere\\talar.mp3";
        String intenseMusicPath = "AlumnoEjemplos\\NatusVincere\\intense.mp3";

        String happyMusicAlias = "music";
        String windAlias = "alias";
        String victoriaAlias = "victory";
        String gameOverAlias = "gameOver";
        String talarArbolAlias = "talar";
        String intenseAlias = "intense";

        Boolean isMainMusicOpened = false;
        Boolean isWindPlaying = false;

        public void playMusic()
        {
            if (isMainMusicOpened)
            {
                mediaPlayer.resume(happyMusicAlias);
            }
            
            mediaPlayer.openAndPlay(true, happyMusicPath, happyMusicAlias);
            isMainMusicOpened = true;
        }
        
        public void playIntense()
        {
            mediaPlayer.pause(happyMusicAlias);
            mediaPlayer.openAndPlay(true, happyMusicPath, happyMusicAlias);
        }
        
        public void playViento()
        {
            if (!isWindPlaying)
            {
                mediaPlayer.openAndPlay(true, windPath, windAlias);
                isWindPlaying = true;
            } 
        }

        public void stopViento()
        {
            if (isWindPlaying)
            {
                mediaPlayer.closeFile(windAlias);
                isWindPlaying = false;
            }
        }

        public void playVictoria()
        {
            mediaPlayer.closeFile("music");
            isMainMusicOpened = false;
            mediaPlayer.openAndPlay(true, victoriaPath, victoriaAlias);
        }

        public void playGameOver()
        {
            mediaPlayer.closeFile("music");
            isMainMusicOpened = false;
            mediaPlayer.openAndPlay(true, gameOverPath, gameOverAlias);
        }

        public void playTalarArbol()
        {
            mediaPlayer.openAndPlay(false, talarArbolPath, talarArbolAlias);
        }
    }
}
