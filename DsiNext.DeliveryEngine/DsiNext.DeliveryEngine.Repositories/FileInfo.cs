using System;

namespace DsiNext.DeliveryEngine.Repositories
{
    public class FileInfo
    {
        private string _foN;
        private string _fiN;
        private string _md5;

        public FileInfo(string foN, string fiN, string md5)
        {
            if (foN == null) throw new ArgumentNullException("foN");
            if (fiN == null) throw new ArgumentNullException("fiN");
            if (md5 == null) throw new ArgumentNullException("md5");

            _foN = foN;
            _fiN = fiN;
            _md5 = md5;
        }

        /// <summary>
        /// Angivelse af sti til den mappe i arkiveringsversionen, hvor filen findes
        /// </summary>
        public string foN
        {
            get { return _foN; }
        }

        /// <summary>
        /// Filens navn
        /// </summary>
        public string fiN
        {
            get { return _fiN; }
        }

        /// <summary>
        /// Filens kontrolsum af typen MD5 iht. IETFRFC1321 - The MD5 Message- Digest Algorithm:
        /// 128 bit (16 bytes) repræsenteret som 32 hexadecimale cifre, alle angivet med enten
        /// minuskler eller versaler (små eller store bogstaver). Krav om 32 hexadecimale cifre
        /// medfører således krav om, at der afhængigt af værdien anvendes foranstillede nuller
        /// </summary>
        public string Md5
        {
            get { return _md5; }
        }
    }
}
