using System;
using Microsoft.EntityFrameworkCore;
using MeetupSurvey.API.Models;

namespace MeetupSurvey.API.Models
{
    public class MeetupSurveyContext : DbContext
    {
        public MeetupSurveyContext(DbContextOptions<MeetupSurveyContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.RemovePluralizingTableNameConvention();

            modelBuilder.Entity<PushRegistration>().ToTable("PushRegistration")
                .HasIndex(x => x.CreatedAt)
                .ForSqlServerIsClustered();
            modelBuilder.Entity<PushRegistration>().ToTable("PushRegistration")
            .HasIndex(x => x.Id)
            .IsUnique()
                .ForSqlServerIsClustered(false);

            modelBuilder.Entity<Survey>().ToTable("Survey")
                .HasIndex(x => x.CreatedAt)
                .ForSqlServerIsClustered();
            modelBuilder.Entity<Survey>().ToTable("Survey")
            .HasIndex(x => x.Id)
            .IsUnique()
                .ForSqlServerIsClustered(false);

            modelBuilder.Entity<Question>().ToTable("Question")
                .HasIndex(x => x.CreatedAt)
                .ForSqlServerIsClustered();
            modelBuilder.Entity<Question>().ToTable("Question")
            .HasIndex(x => x.Id)
            .IsUnique()
                .ForSqlServerIsClustered(false);

            modelBuilder.Entity<Answer>().ToTable("Answer")
                .HasIndex(x => x.CreatedAt)
                .ForSqlServerIsClustered();
            modelBuilder.Entity<Answer>().ToTable("Answer")
            .HasIndex(x => x.Id)
            .IsUnique()
                .ForSqlServerIsClustered(false);

            modelBuilder.Entity<Group>().ToTable("Group")
                .HasIndex(x => x.CreatedAt)
                .ForSqlServerIsClustered();
            modelBuilder.Entity<Group>().ToTable("Group")
            .HasIndex(x => x.Id)
            .IsUnique()
                .ForSqlServerIsClustered(false);


            modelBuilder.Entity<GroupUser>().ToTable("GroupUser")
                .HasIndex(x => x.CreatedAt)
                .ForSqlServerIsClustered();
            modelBuilder.Entity<GroupUser>().ToTable("GroupUser")
            .HasIndex(x => x.Id)
            .IsUnique()
                .ForSqlServerIsClustered(false);

            modelBuilder.Entity<Prize>().ToTable("Prize")
                .HasIndex(x => x.CreatedAt)
                .ForSqlServerIsClustered();
            modelBuilder.Entity<Prize>().ToTable("Prize")
            .HasIndex(x => x.Id)
            .IsUnique()
                .ForSqlServerIsClustered(false);

            modelBuilder.Entity<UserAccount>().ToTable("UserAccount")
                .HasIndex(x => x.MeetupUserId)
                .IsUnique();

            modelBuilder.Entity<UserAccount>().ToTable("UserAccount")   
                .HasIndex(x => x.CreatedAt)
                .ForSqlServerIsClustered();

            modelBuilder.Entity<UserAccount>().ToTable("UserAccount")
                .HasIndex(x => x.Id)
                .IsUnique()
                .ForSqlServerIsClustered(false);



        }
        

        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<MeetupSurvey.API.Models.Group> Groups { get; set; }
        public DbSet<MeetupSurvey.API.Models.GroupUser> GroupUsers { get; set; }
        public DbSet<MeetupSurvey.API.Models.UserAccount> UserAccounts { get; set; }
        public DbSet<MeetupSurvey.API.Models.Prize> Prizes { get; set; }

        public DbSet<PushRegistration> PushRegistrations { get; set; }

    }
}
