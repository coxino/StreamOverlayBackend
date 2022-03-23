using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Tournament
    {
        public Tournament()
        {
            MeciuriSferturi = new Meci[4];
            MeciuriSemiFinale = new Meci[2];
            MeciFinal = new Meci();
        }

        public Tournament(TournamentCreateInfos json)
        {
            MeciuriSferturi = new Meci[4] { new Meci(json), new Meci(json), new Meci(json), new Meci(json) };
            MeciuriSemiFinale = new Meci[2] { new Meci(json), new Meci(json) };
            MeciFinal = new Meci(json);

            IsQuarter = true;
            IsSemis = true;
        }
        public bool IsQuarter { get; set; }
        public bool IsSemis { get; set; }

        public Meci[] MeciuriSferturi { get; set; }
        public Meci[] MeciuriSemiFinale { get; set; }
        public Meci MeciFinal { get; set; }
        public bool IsFinal { get; set; }
        public bool IsPaused { get; set; }
    }
}
