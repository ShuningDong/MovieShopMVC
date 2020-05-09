using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace MovieShop.Core.Entities
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Cast, Movie>();
        }
    }
}
