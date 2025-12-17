using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBotenReservering.Core.Interfaces.Repositories
{
    public interface ITweetRepository
    {
        // todo find type for postmedia
        Task<string> PostMedia(string file);
        Task<string> PostTweet(string prompt);
    }
}
