using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BillBot.Database
{
    public sealed class ApplicationContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<UserStatus> UserStatuses { get; set; }
        public DbSet<Phrase> Phrases { get; set; }
        public DbSet<Achievement> Achievements { get; set; }

        public ApplicationContext()
        
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies().UseSqlite("Filename=BillDb");//.UseMySQL("server=localhost;database=botDb;user=;password=Test123");.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=botdb;Trusted_Connection=True;MultipleActiveResultSets=true");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatusWithAchievement>()
                .HasKey(t => new {t.AchievementId, t.UserStatusId});
            modelBuilder.Entity<StatusWithAchievement>()
                .HasOne(a => a.Achievement)
                .WithMany(a => a.StatusWithAchievements)
                .HasForeignKey(swa => swa.AchievementId);
            modelBuilder.Entity<Phrase>().HasData(
                new Phrase() {Id = 1,Text = "Властью данной мне leatherman'ом, назначаю {0} пидором"},
                new Phrase(){Id=2,Text="Лучше не ронять мыло рядом с {0} "},
                new Phrase(){Id=3,Text = "Вилкой в глаз или в жопу раз? Ответ на эту загадку на себе прочувсвовал {0}. Подсказка: он выбрал 2 вариант "}
                ,new Phrase(){Id=4,Text="Пидором не становятся , а рождаются. Именно таким родился {0} "},
                new Phrase(){Id=5,Text="Однажды, шла рота солдат  по равнине и наткнулась на ручей. Пить стали по очереди. Первым дали попить людям, вторым блядям , третьим матросам , а четвёртым  хуесосам. Именно в 4 взводе был {0}"});
            
            modelBuilder.Entity<Achievement>().HasData(
                new Achievement()
                {
                    Id=1,
                    Name = "firstAchivement", Description = "Станьте пидором 1 раз",
                    PathToImage = @"https://imgur.com/dNthUVX"
                });
            
            base.OnModelCreating(modelBuilder);
            
        }
    }
}