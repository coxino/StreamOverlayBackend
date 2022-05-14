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
            MeciuriOptimi = new Meci[8];
            MeciuriSferturi = new Meci[4];
            MeciuriSemiFinale = new Meci[2];
            MeciFinal = new Meci();            
        }

        public Tournament(TournamentCreateInfos json)
        { 
            MeciuriOptimi = new Meci[8] { new Meci(json), new Meci(json), new Meci(json), new Meci(json), new Meci(json), new Meci(json), new Meci(json), new Meci(json) };            
            MeciuriSferturi = new Meci[4] { new Meci(json), new Meci(json), new Meci(json), new Meci(json) };
            MeciuriSemiFinale = new Meci[2] { new Meci(json), new Meci(json) };
            MeciFinal = new Meci(json);

            IsOptimi = json.TournamentModel.IsOptimi;
            IsQuarter = json.TournamentModel.IsQuarter;
            IsSemis = json.TournamentModel.IsSemis;
        }

        public bool IsOptimi { get; set; }
        public bool IsQuarter { get; set; }
        public bool IsSemis { get; set; }

        public Meci[] MeciuriOptimi { get; set; }
        public Meci[] MeciuriSferturi { get; set; }
        public Meci[] MeciuriSemiFinale { get; set; }
        public Meci MeciFinal { get; set; }
        public bool IsFinal { get; set; }
        public bool IsPaused { get; set; }
    }
}
