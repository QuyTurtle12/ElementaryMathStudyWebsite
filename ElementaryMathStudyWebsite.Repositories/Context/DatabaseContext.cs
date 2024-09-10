using ElementaryMathStudyWebsite.Contract.Repositories.Entity;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Repositories.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Chapter> Chapter { get; set; }
        public DbSet<Option> Option { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderDetail> OrderDetail { get; set; }
        public DbSet<Progress> Progress { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Quiz> Quiz { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<Topic> Topic { get; set; }
        public DbSet<UserAnswer> UserAnswer { get; set; }

        // Mapping Configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite key
            modelBuilder.Entity<Progress>()
                .HasKey(p => new { p.StudentId, p.QuizId });

            // Composite key
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.SubjectId, od.StudentId });

            // Composite key
            modelBuilder.Entity<UserAnswer>()
                .HasKey(a => new { a.QuestionId, a.UserId});

            // User - Role Relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // User - Order Relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // User - UserAnswer Relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.Answers)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // UserAnswer - Option Relationship
            modelBuilder.Entity<UserAnswer>()
                .HasOne(a => a.Option)
                .WithMany(o => o.Answers)
                .HasForeignKey(a => a.OptionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // UserAnswer - Question Relationship
            modelBuilder.Entity<UserAnswer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.OptionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // Question - Option Relationship
            modelBuilder.Entity<Question>()
                .HasMany(q => q.Options)
                .WithOne(o => o.Question)
                .HasForeignKey(a => a.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // Question - Quiz Relationship
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(question => question.Questions)
                .HasForeignKey(q => q.QuizId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // Quiz - Topic Relationship
            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Topic)
                .WithOne(t => t.Quiz)
                .HasForeignKey<Topic>(t => t.QuizId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // Quiz - Progress Relationship
            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Progresses)
                .WithOne(p => p.Quiz)
                .HasForeignKey(p => p.QuizId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // Quiz - Chapter Relationship
            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Chapter)
                .WithOne(c => c.Quiz)
                .HasForeignKey<Chapter>(c => c.QuizId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // Topic - Chapter Relationship
            modelBuilder.Entity<Topic>()
                .HasOne(t => t.Chapter)
                .WithMany(c => c.Topics)
                .HasForeignKey(t => t.ChapterId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // Chapter - Subject Relationship
            modelBuilder.Entity<Chapter>()
                .HasOne(c => c.Subject)
                .WithMany(s => s.Chapters)
                .HasForeignKey(c => c.SubjectId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // Subject - Progress Relationship
            modelBuilder.Entity<Subject>()
                .HasMany(s => s.Progresses)
                .WithOne(p => p.Subject)
                .HasForeignKey(c => c.SubjectId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // Subject - Order Detail Relationship
            modelBuilder.Entity<Subject>()
                .HasMany(s => s.Detail)
                .WithOne(d => d.Subject)
                .HasForeignKey(d => d.SubjectId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);


            // Order Detail - Order Relationship
            modelBuilder.Entity<OrderDetail>()
                .HasOne(d => d.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            // Order Detail - User Relationship
            modelBuilder.Entity<OrderDetail>()
                .HasOne(d => d.User)
                .WithMany(u => u.OrderDetails)
                .HasForeignKey(d => d.StudentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
