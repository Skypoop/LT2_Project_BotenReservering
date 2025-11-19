using ProjectBotenReservering.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectBotenReservering.Core.Models;

namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface IMailService
{
    public Task SendMailAsync(List<string> recivers, string subject, string body);
}