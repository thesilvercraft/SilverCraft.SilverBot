﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SilverBotDS.Objects;

namespace SilverBotDS.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20211030114547_add webshot to serversettngs")]
    partial class addwebshottoserversettngs
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0-preview.5.21301.9");

            modelBuilder.Entity("SilverBotDS.Objects.Database.Classes.PlannedEvent", b =>
                {
                    b.Property<string>("EventID")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("ChannelID")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Handled")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("MessageID")
                        .HasColumnType("INTEGER");

                    b.Property<ulong?>("ResponseMessageID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Time")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("UserID")
                        .HasColumnType("INTEGER");

                    b.HasKey("EventID");

                    b.ToTable("plannedEvents");
                });

            modelBuilder.Entity("SilverBotDS.Objects.Database.Classes.UserExperience", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("XPString")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("userExperiences");
                });

            modelBuilder.Entity("SilverBotDS.Objects.Database.Classes.UserQuote", b =>
                {
                    b.Property<string>("QuoteId")
                        .HasColumnType("TEXT");

                    b.Property<string>("QuoteContent")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("QuoteId");

                    b.ToTable("userQuotes");
                });

            modelBuilder.Entity("SilverBotDS.Objects.ServerSettings", b =>
                {
                    b.Property<ulong>("ServerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("EmotesOptin")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LangName")
                        .HasColumnType("TEXT");

                    b.Property<string>("PrefixesInJson")
                        .HasColumnType("TEXT");

                    b.Property<bool>("RepeatThings")
                        .HasColumnType("INTEGER");

                    b.Property<ulong?>("ServerStatsCategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ServerStatsTemplatesInJson")
                        .HasColumnType("TEXT");

                    b.Property<bool>("WebShot")
                        .HasColumnType("INTEGER");

                    b.HasKey("ServerId");

                    b.ToTable("serverSettings");
                });

            modelBuilder.Entity("SilverBotDS.Objects.UserSettings", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsBanned")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LangName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("userSettings");
                });
#pragma warning restore 612, 618
        }
    }
}
