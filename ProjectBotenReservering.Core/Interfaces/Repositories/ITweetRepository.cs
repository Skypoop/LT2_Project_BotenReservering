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
        Task<int> PostMediaAsync(string file);
        Task<string> PostTweetAsync(string tweetContent);
    }
}
