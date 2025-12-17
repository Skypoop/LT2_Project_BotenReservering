using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBotenReservering.Core.Exceptions;

public sealed class NotEnoughBoatsException : Exception
{
    public int Needed { get; }
    public int Available { get; }

    public NotEnoughBoatsException(int needed, int available)
        : base($"Niet genoeg boten beschikbaar. Nodig: {needed}, Beschikbaar: {available}.")
    {
        Needed = needed;
        Available = available;
    }
}
