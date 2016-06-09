using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using AlumnoEjemplos.MiGrupo.Sounds;
namespace AlumnoEjemplos.NatusVincere
{
    public class Sounds
    {
        MediaPlayer mediaPlayer = new MediaPlayer();
        String happyMusicPath = "AlumnoEjemplos\\NatusVincere\\Sounds\\happy.mp3";
        String windPath = "AlumnoEjemplos\\NatusVincere\\Sounds\\wind.mp3";
        String victoriaPath = "AlumnoEjemplos\\NatusVincere\\Sounds\\victory.mp3";
        String gameOverPath = "AlumnoEjemplos\\NatusVincere\\Sounds\\gameOver.mp3";
        String talarArbolPath = "AlumnoEjemplos\\NatusVincere\\Sounds\\talar.mp3";
        String intenseMusicPath = "AlumnoEjemplos\\NatusVincere\\Sounds\\intense.mp3";
        String rainPath = "AlumnoEjemplos\\NatusVincere\\Sounds\\rain.mp3";

        String happyMusicAlias = "music";
        String windAlias = "alias";
        String victoriaAlias = "victory";
        String gameOverAlias = "gameOver";
        String talarArbolAlias = "talar";
        String intenseAlias = "intense";
        String rainAlias = "rain";

        Boolean isMainMusicOpened = false;
        Boolean isWindPlaying = false;
        Boolean isIntensePlaying = false;
        Boolean isRainPlaying = false;
        Boolean isTalarPlaying = false;

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
            if (isIntensePlaying)
            {
                return;
            }
            mediaPlayer.pause(happyMusicAlias);
            mediaPlayer.openAndPlay(true, intenseMusicPath, intenseAlias);
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
                mediaPlayer.pause(windAlias);
                mediaPlayer.closeFile(windAlias);
                isWindPlaying = false;
            }
        }

        public void playVictoria()
        {
            if (isWindPlaying)
            {
                mediaPlayer.pause(windAlias);
                mediaPlayer.closeFile(windAlias);
            }
            if (isRainPlaying)
            {
                mediaPlayer.pause(rainAlias);
                mediaPlayer.closeFile(rainAlias);
            }
            mediaPlayer.pause("music");
            mediaPlayer.closeFile("music");
            isMainMusicOpened = false;
            mediaPlayer.openAndPlay(true, victoriaPath, victoriaAlias);
        }

        public void playGameOver()
        {
            if (isWindPlaying) mediaPlayer.closeFile(windAlias);
            mediaPlayer.closeFile("music");
            mediaPlayer.openAndPlay(true, gameOverPath, gameOverAlias);
        }

        public void playTalarArbol()
        {
            if (isTalarPlaying) return;
            mediaPlayer.openAndPlay(false, talarArbolPath, talarArbolAlias);
        }

        public void playRain()
        {
            if (isRainPlaying) return;
            mediaPlayer.openAndPlay(true, rainPath, rainAlias);
            isRainPlaying = true;
        }
    }
}
