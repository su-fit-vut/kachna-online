﻿// <auto-generated />
using System;
using KachnaOnline.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KachnaOnline.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20211118105455_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("KachnaOnline.Data.Entities.BoardGames.BoardGame", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<TimeSpan?>("DefaultReservationTime")
                        .HasColumnType("interval");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<int>("InStock")
                        .IsConcurrencyToken()
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NoteInternal")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<int?>("OwnerId")
                        .HasColumnType("integer");

                    b.Property<int?>("PlayersMax")
                        .HasColumnType("integer");

                    b.Property<int?>("PlayersMin")
                        .HasColumnType("integer");

                    b.Property<int>("Unavailable")
                        .HasColumnType("integer");

                    b.Property<bool>("Visible")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("OwnerId");

                    b.ToTable("BoardGames");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.BoardGames.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ColourHex")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.ToTable("BoardGameCategories");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.BoardGames.Reservation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("MadeById")
                        .HasColumnType("integer");

                    b.Property<DateTime>("MadeOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("NoteInternal")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<string>("NoteUser")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.HasKey("Id");

                    b.HasIndex("MadeById");

                    b.ToTable("BoardGameReservations");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.BoardGames.ReservationItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BoardGameId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ExpiresOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("ReservationId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BoardGameId");

                    b.HasIndex("ReservationId");

                    b.ToTable("BoardGameReservationItems");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.BoardGames.ReservationItemEvent", b =>
                {
                    b.Property<int>("ReservationItemId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("MadeOn")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("MadeById")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("NewExpiryDateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("NewState")
                        .HasColumnType("integer");

                    b.Property<string>("NoteInternal")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("ReservationItemId", "MadeOn");

                    b.HasIndex("MadeById");

                    b.ToTable("BoardGameReservationItemEvents");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.ClubStates.PlannedState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("AssociatedEventId")
                        .HasColumnType("integer");

                    b.Property<int?>("ClosedById")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("Ended")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("MadeById")
                        .HasColumnType("integer");

                    b.Property<int?>("NextPlannedStateId")
                        .HasColumnType("integer");

                    b.Property<string>("NoteInternal")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<string>("NotePublic")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<DateTime>("PlannedEnd")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("RepeatingStateId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Start")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AssociatedEventId");

                    b.HasIndex("ClosedById");

                    b.HasIndex("MadeById");

                    b.HasIndex("NextPlannedStateId")
                        .IsUnique();

                    b.HasIndex("RepeatingStateId");

                    b.ToTable("PlannedStates");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.ClubStates.RepeatingState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("integer");

                    b.Property<DateTime>("EffectiveFrom")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("EffectiveTo")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("MadeById")
                        .HasColumnType("integer");

                    b.Property<string>("NoteInternal")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<string>("NotePublic")
                        .HasMaxLength(1024)
                        .HasColumnType("character varying(1024)");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<TimeSpan>("TimeFrom")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("TimeTo")
                        .HasColumnType("interval");

                    b.HasKey("Id");

                    b.HasIndex("MadeById");

                    b.ToTable("RepeatingStates");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.Events.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("From")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("FullDescription")
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<int>("MadeById")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Place")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PlaceUrl")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<DateTime>("To")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Url")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.HasKey("Id");

                    b.HasIndex("MadeById");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.Users.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.Users.User", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<bool>("Disabled")
                        .HasColumnType("boolean");

                    b.Property<decimal?>("DiscordId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(320)
                        .HasColumnType("character varying(320)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Nickname")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.Users.UserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<int?>("AssignedByUserId")
                        .HasColumnType("integer");

                    b.Property<bool>("ForceDisable")
                        .HasColumnType("boolean");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("AssignedByUserId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.BoardGames.BoardGame", b =>
                {
                    b.HasOne("KachnaOnline.Data.Entities.BoardGames.Category", "Category")
                        .WithMany("Games")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KachnaOnline.Data.Entities.Users.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Category");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.BoardGames.Reservation", b =>
                {
                    b.HasOne("KachnaOnline.Data.Entities.Users.User", "MadeBy")
                        .WithMany("Reservations")
                        .HasForeignKey("MadeById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MadeBy");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.BoardGames.ReservationItem", b =>
                {
                    b.HasOne("KachnaOnline.Data.Entities.BoardGames.BoardGame", "BoardGame")
                        .WithMany()
                        .HasForeignKey("BoardGameId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("KachnaOnline.Data.Entities.BoardGames.Reservation", "Reservation")
                        .WithMany("Items")
                        .HasForeignKey("ReservationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BoardGame");

                    b.Navigation("Reservation");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.BoardGames.ReservationItemEvent", b =>
                {
                    b.HasOne("KachnaOnline.Data.Entities.Users.User", "MadeBy")
                        .WithMany()
                        .HasForeignKey("MadeById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("KachnaOnline.Data.Entities.BoardGames.ReservationItem", "ReservationItem")
                        .WithMany("Events")
                        .HasForeignKey("ReservationItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MadeBy");

                    b.Navigation("ReservationItem");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.ClubStates.PlannedState", b =>
                {
                    b.HasOne("KachnaOnline.Data.Entities.Events.Event", "AssociatedEvent")
                        .WithMany("LinkedPlannedStates")
                        .HasForeignKey("AssociatedEventId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("KachnaOnline.Data.Entities.Users.User", "ClosedBy")
                        .WithMany()
                        .HasForeignKey("ClosedById")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("KachnaOnline.Data.Entities.Users.User", "MadeBy")
                        .WithMany()
                        .HasForeignKey("MadeById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("KachnaOnline.Data.Entities.ClubStates.PlannedState", "NextPlannedState")
                        .WithOne()
                        .HasForeignKey("KachnaOnline.Data.Entities.ClubStates.PlannedState", "NextPlannedStateId");

                    b.HasOne("KachnaOnline.Data.Entities.ClubStates.RepeatingState", "RepeatingState")
                        .WithMany("LinkedPlannedStates")
                        .HasForeignKey("RepeatingStateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("AssociatedEvent");

                    b.Navigation("ClosedBy");

                    b.Navigation("MadeBy");

                    b.Navigation("NextPlannedState");

                    b.Navigation("RepeatingState");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.ClubStates.RepeatingState", b =>
                {
                    b.HasOne("KachnaOnline.Data.Entities.Users.User", "MadeBy")
                        .WithMany()
                        .HasForeignKey("MadeById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("MadeBy");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.Events.Event", b =>
                {
                    b.HasOne("KachnaOnline.Data.Entities.Users.User", "MadeBy")
                        .WithMany()
                        .HasForeignKey("MadeById")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("MadeBy");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.Users.UserRole", b =>
                {
                    b.HasOne("KachnaOnline.Data.Entities.Users.User", "AssignedByUser")
                        .WithMany()
                        .HasForeignKey("AssignedByUserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("KachnaOnline.Data.Entities.Users.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KachnaOnline.Data.Entities.Users.User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssignedByUser");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.BoardGames.Category", b =>
                {
                    b.Navigation("Games");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.BoardGames.Reservation", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.BoardGames.ReservationItem", b =>
                {
                    b.Navigation("Events");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.ClubStates.RepeatingState", b =>
                {
                    b.Navigation("LinkedPlannedStates");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.Events.Event", b =>
                {
                    b.Navigation("LinkedPlannedStates");
                });

            modelBuilder.Entity("KachnaOnline.Data.Entities.Users.User", b =>
                {
                    b.Navigation("Reservations");

                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}
