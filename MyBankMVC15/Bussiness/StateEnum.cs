using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyBankMVC15.Bussiness
{
    enum StateEnum : int
    {
        OVERDRAW = 0,
        SILVER = 1000,
        GOLD = 2500,
        PLATINUM,
    }
}