﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoginsAU.Datos
{
    public class AplicacionDbConext : IdentityDbContext
    {
        public AplicacionDbConext(DbContextOptions options) : base(options)
        {
        }

        // Modelos necesarios del proyecto
    }
}
