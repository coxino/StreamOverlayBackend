using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StreamApi.Controllers
{
    public class Declaratie : IEquatable<Declaratie>
    {
        public Declaratie()
        {
        }

        public string ModelDeclaratie { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Declaratie);
        }

        public bool Equals(Declaratie other)
        {
            return other != null &&
                   ModelDeclaratie == other.ModelDeclaratie;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ModelDeclaratie);
        }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class FiscolineController : ControllerBase
    {
        public string comerciant = "O_3e4658d1";
        public Byte[] key2 = Encoding.UTF8.GetBytes("w92WG@V@N1[_Q8*56r^o");

        [HttpGet("ListaDeclaratii")]
        public Dictionary<string, string[]> Get()
        {
            Dictionary<string, string[]> keyValuePairs = new Dictionary<string, string[]>();
            var directoare = Directory.GetDirectories(@"c:\Declaratii\");

            foreach (var director in directoare)
            {
                var fisiere = Directory.GetFiles(director);

                keyValuePairs.Add(director, fisiere);
            }

            return keyValuePairs;
        }

        [HttpPost("recieveDeclaratie")]
        public async Task<ActionResult<string>> RecieveDeclaratieAsync([FromHeader] string authorization, [FromBody] object declaratie)
        {
            dynamic declare = JObject.Parse(declaratie.ToString());
            var declare2 = JsonConvert.SerializeObject(declare.declaratie);
            
            var q = DateTime.Now.Ticks.ToString();
            await System.IO.File.WriteAllTextAsync(@"c:\Declaratii\" + q + ".fsl",  declare2);

            return Ok(new { mesaj = q });
        }

        [HttpGet("getDeclaratie")]
        public async Task<ActionResult<string>> GetDeclaratie([FromHeader] string authorization, [FromHeader] string declaratieID)
        {
           var response = await System.IO.File.ReadAllTextAsync(@"c:\Declaratii\" + declaratieID + ".fsl");

           return Ok(new { response = response});
        }

        [HttpPost("confirmare")]
        public async Task<string> ConfirmarePlata()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var query = await reader.ReadToEndAsync();
                var done = query;
            }



            return "";

            //HMACMD5 hmacMD5 = new HMACMD5(key2);
            //hmacMD5.ComputeHash(Encoding.UTF8.GetBytes(HASH));
            ///*
            //hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(urlPath));
            //foreach (string kvstr in list)
            //{
            //    hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(kvstr));
            //}
            // */
            //byte[] hash = hmacMD5.Hash;
            ////TO HEX
            //var query = BitConverter.ToString(hash).Replace("-", string.Empty).ToUpper();

        }
    }


    public class ModelDeclaratie : IEquatable<ModelDeclaratie>
    {
        public int AnulDeclaratiei { get; set; }

        public UserModel UserModel { get; set; }
        public SistemReal SistemReal { get; set; }
        public DateFirma DateFirma { get; set; }

        public NormaDeVenit NormaDeVenit { get; set; }

        public ModelDeclaratie()
        {
        }

        public TipulDeclaratiei TipulDeclaraiatiei { get; set; }
        public SistemVenit SistemVenit { get; set; }
        public DomeniiNeautorizate DomeniiNeautorizate { get; set; }
        public List<ContractInchiriere> ContracteInchiriere { get; set; }
        public List<ContractInScopTuristic> ContracteInScopTuristic { get; set; }

        public bool Knowdata { get; set; }
        public TipAutorizatie TipulAutorizatiei { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ModelDeclaratie);
        }

        public bool Equals(ModelDeclaratie other)
        {
            return other != null &&
                   AnulDeclaratiei == other.AnulDeclaratiei &&
                   EqualityComparer<UserModel>.Default.Equals(UserModel, other.UserModel) &&
                   EqualityComparer<SistemReal>.Default.Equals(SistemReal, other.SistemReal) &&
                   EqualityComparer<DateFirma>.Default.Equals(DateFirma, other.DateFirma) &&
                   EqualityComparer<NormaDeVenit>.Default.Equals(NormaDeVenit, other.NormaDeVenit) &&
                   TipulDeclaraiatiei == other.TipulDeclaraiatiei &&
                   SistemVenit == other.SistemVenit &&
                   DomeniiNeautorizate == other.DomeniiNeautorizate &&
                   EqualityComparer<List<ContractInchiriere>>.Default.Equals(ContracteInchiriere, other.ContracteInchiriere) &&
                   EqualityComparer<List<ContractInScopTuristic>>.Default.Equals(ContracteInScopTuristic, other.ContracteInScopTuristic) &&
                   Knowdata == other.Knowdata &&
                   TipulAutorizatiei == other.TipulAutorizatiei;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(AnulDeclaratiei);
            hash.Add(UserModel);
            hash.Add(SistemReal);
            hash.Add(DateFirma);
            hash.Add(NormaDeVenit);
            hash.Add(TipulDeclaraiatiei);
            hash.Add(SistemVenit);
            hash.Add(DomeniiNeautorizate);
            hash.Add(ContracteInchiriere);
            hash.Add(ContracteInScopTuristic);
            hash.Add(Knowdata);
            hash.Add(TipulAutorizatiei);
            return hash.ToHashCode();
        }
    }

    public enum TipAutorizatie
    {
        Niciunul,
        Autorizat,
        NeAutorizat
    }

    public class SistemReal : IEquatable<SistemReal>
    {
        public SistemReal()
        {
        }

        public double VenitBrut { get; set; }
        public double CheltuieliDeductibile { get; set; }
        public double CAS { get; set; }
        public double VenitNet { get; set; }
        public double ImpozitCalculat { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as SistemReal);
        }

        public bool Equals(SistemReal other)
        {
            return other != null &&
                   VenitBrut == other.VenitBrut &&
                   CheltuieliDeductibile == other.CheltuieliDeductibile &&
                   CAS == other.CAS &&
                   VenitNet == other.VenitNet &&
                   ImpozitCalculat == other.ImpozitCalculat;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(VenitBrut, CheltuieliDeductibile, CAS, VenitNet, ImpozitCalculat);
        }
    }

    public class NormaDeVenit : IEquatable<NormaDeVenit>
    {
        public NormaDeVenit()
        {
        }

        public double NormaDeVenitN { get; set; }
        public double Impozit { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as NormaDeVenit);
        }

        public bool Equals(NormaDeVenit other)
        {
            return other != null &&
                   NormaDeVenitN == other.NormaDeVenitN &&
                   Impozit == other.Impozit;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NormaDeVenitN, Impozit);
        }
    }

    public class DateFirma : IEquatable<DateFirma>
    {
        public DateFirma()
        {
        }

        public int CodCaen { get; set; }
        public string Sediu { get; set; }
        public int CUI { get; set; }
        public string DataInfiintare { get; set; }
        public string DataIncepereActivitate { get; set; }
        public string DataIncheiereActivitate { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as DateFirma);
        }

        public bool Equals(DateFirma other)
        {
            return other != null &&
                   CodCaen == other.CodCaen &&
                   Sediu == other.Sediu &&
                   CUI == other.CUI &&
                   DataInfiintare == other.DataInfiintare &&
                   DataIncepereActivitate == other.DataIncepereActivitate &&
                   DataIncheiereActivitate == other.DataIncheiereActivitate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CodCaen, Sediu, CUI, DataInfiintare, DataIncepereActivitate, DataIncheiereActivitate);
        }
    }

    public enum TipulDeclaratiei
    {
        Niciuna,
        DeclaratieInitiala,
        DeclaratieRectificativa
    }

    public enum SistemVenit
    {
        Niciunul,
        SistemReal,
        NormaVenit
    }

    public enum DomeniiNeautorizate
    {
        Niciunul,
        ContractInchiriere,
        InScopTuristic
    }

    public class ContractInchiriere : IEquatable<ContractInchiriere>
    {
        public ContractInchiriere()
        {
        }

        public string NumarContract { get; set; }
        public string DataContract { get; set; }
        public string Adresa { get; set; }
        public string DataIncepere { get; set; }
        public string DataIncetare { get; set; }
        public double VenitLunar { get; set; }
        public double VenitAnual { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContractInchiriere);
        }

        public bool Equals(ContractInchiriere other)
        {
            return other != null &&
                   NumarContract == other.NumarContract &&
                   DataContract == other.DataContract &&
                   Adresa == other.Adresa &&
                   DataIncepere == other.DataIncepere &&
                   DataIncetare == other.DataIncetare &&
                   VenitLunar == other.VenitLunar &&
                   VenitAnual == other.VenitAnual;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NumarContract, DataContract, Adresa, DataIncepere, DataIncetare, VenitLunar, VenitAnual);
        }
    }

    public class ContractInScopTuristic : IEquatable<ContractInScopTuristic>
    {
        public ContractInScopTuristic()
        {
        }

        public string Judet { get; set; }
        public string Localitate { get; set; }
        public string Nrcamere { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContractInScopTuristic);
        }

        public bool Equals(ContractInScopTuristic other)
        {
            return other != null &&
                   Judet == other.Judet &&
                   Localitate == other.Localitate &&
                   Nrcamere == other.Nrcamere;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Judet, Localitate, Nrcamere);
        }
    }


    public class UserModel : IEquatable<UserModel>
    {
        public UserModel()
        {
        }

        public string CNP { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Prenume { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as UserModel);
        }

        public bool Equals(UserModel other)
        {
            return other != null &&
                   CNP == other.CNP &&
                   Telefon == other.Telefon &&
                   Email == other.Email &&
                   Name == other.Name &&
                   Prenume == other.Prenume;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CNP, Telefon, Email, Name, Prenume);
        }
    }
}
