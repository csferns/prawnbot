﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Prawnbot.Data;

namespace Prawnbot.Data.Migrations
{
    [DbContext(typeof(BotDatabaseContext))]
    [Migration("20190614203346_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Prawnbot.Data.Entities.SavedTranslation", b =>
                {
                    b.Property<int>("TranslationId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ToLanguage");

                    b.Property<string>("Translation");

                    b.HasKey("TranslationId");

                    b.ToTable("SavedTranslation","core");
                });

            modelBuilder.Entity("Prawnbot.Data.Entities.Yotta", b =>
                {
                    b.Property<int>("PrependId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Count");

                    b.Property<int>("PrependValue");

                    b.HasKey("PrependId");

                    b.ToTable("Yottas");
                });
#pragma warning restore 612, 618
        }
    }
}
