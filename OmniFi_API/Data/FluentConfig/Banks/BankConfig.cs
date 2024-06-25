﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmniFi_API.Models.Banks;
using OmniFi_API.Utilities;

namespace OmniFi_API.Data.FluentConfig.Banks
{
    public class BankConfig : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> builder)
        {
            builder.HasKey(x => x.BankID);

            builder
                .Property(x => x.BankID)
                .ValueGeneratedOnAdd();

            builder
                .Property(x => x.BankName)
                .HasMaxLength(30)
                .IsRequired();

            builder
                .Property(x => x.ImageUrl)
                .HasColumnType("varbinary(max)");

            builder.HasData(new Bank()
            {
                BankID = 1,
                BankName = BankNames.BoursoBank,
                ImageUrl = "https://upload.wikimedia.org/wikipedia/fr/thumb/3/3d/Logo-boursorama-banque.svg/1024px-Logo-boursorama-banque.svg.png"
            }); 

        }
    }
}
