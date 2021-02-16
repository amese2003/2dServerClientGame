﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SharedDB;

namespace SharedDB.Migrations
{
    [DbContext(typeof(SharedDbContext))]
    partial class SharedDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SharedDB.ServerDb", b =>
                {
                    b.Property<int>("ServerDbId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BusyScore")
                        .HasColumnType("int");

                    b.Property<string>("IpAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("port")
                        .HasColumnType("int");

                    b.HasKey("ServerDbId");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("ServerInfo");
                });

            modelBuilder.Entity("SharedDB.TokenDB", b =>
                {
                    b.Property<int>("TokenDbId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountDbId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Expired")
                        .HasColumnType("datetime2");

                    b.Property<int>("Token")
                        .HasColumnType("int");

                    b.HasKey("TokenDbId");

                    b.HasIndex("AccountDbId")
                        .IsUnique();

                    b.ToTable("Token");
                });
#pragma warning restore 612, 618
        }
    }
}
