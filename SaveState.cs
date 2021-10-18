using System;
using System.Collections.Generic;
using System.Text;

namespace TrexGame
{
    [Serializable]
    public class SaveState
    {
        public int HighScore { get; set; }
        public DateTime HighScoreDate { get; set; }
    }
}
