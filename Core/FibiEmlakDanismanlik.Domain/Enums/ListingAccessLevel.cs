using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibiEmlakDanismanlik.Domain.Enums
{
    /// <summary>
    /// Danışmanın belirli bir ilan üzerindeki erişim düzeyi.
    /// </summary>
    public enum ListingAccessLevel
    {
        /// <summary>
        /// İlanı yalnızca görüntüleme yetkisi.
        /// </summary>
        View = 1,

        /// <summary>
        /// İlanı görüntüleme ve düzenleme yetkisi.
        /// </summary>
        Edit = 2
    }
}
