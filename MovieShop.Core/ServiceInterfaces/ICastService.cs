﻿using MovieShop.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Core.ServiceInterfaces
{
    public interface ICastService
    {
        Task<Cast> GetCastDetailsWithMovies(int id);
    }
}
