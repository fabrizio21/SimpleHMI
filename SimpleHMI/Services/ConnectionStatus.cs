using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHMI.Services
{
    public enum ConnectionStatus
    {
        Idle,
        Offline,
        Connecting,
        Online
    }
}