using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;


namespace EfDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var context = GetContextWithData();

            var original = new Entity
            {
                Id = 1,
                Description = "Original",
                ChildCode = "ChildCode-Original",
                Child = new Child
                {
                    Code = "ChildCode-Original",
                    Description = "Original child"
                }
            };

            var changed = new Entity
            {
                Id = 2,
                Description = "Changed",
                ChildCode = "ChildCode-Changed",
                Child = new Child
                {
                    Code = "ChildCode-Changed",
                    Description = "Changed child"
                }
            };

            ////////////////////////////////////
            context.Entities.Add(original);
            context.SaveChanges();
            ///////////////////////////////////

            /////////////////////////
            changed.ChildCode = original.ChildCode;
            changed.Description = original.Description;
            /////////////////////////

            original.Description = "i'm changed";

            context.Entities.UpdateRange(new[] { original });
            context.Entities.AddRange(new[] { changed });


            context.SaveChanges();

            Console.WriteLine($"EF Core 2.2");
            Console.WriteLine($"Codes - {string.Join(",", context.Entities.Select(e => e.ChildCode))}");
            Console.WriteLine($"ChildrenCodes - {string.Join(",", context.Entities.Select(e => e.Child.Code))}");

            var entity1 = context.Entities.Single(e => e.Id == 1);
            var entity2 = context.Entities.Single(e => e.Id == 2);

            Console.WriteLine($"OriginalEntity. EntityId = {entity1.Id}, Description = {entity1.Description}, ChildCode = {entity1.ChildCode}");
            Console.WriteLine($"ChangedEntity. EntityId = {entity2.Id}, Description = {entity2.Description}, ChildCode = {entity2.ChildCode}");

            Console.WriteLine($"OriginalEntity. Child. ChildCode = {entity1.Child.Code}, Description = {entity1.Child.Description}");
            Console.WriteLine($"ChangedEntity. Child. ChildCode = {entity2.Child.Code}, Description = {entity2.Child.Description}");

            Console.ReadKey();
        }

        private static DemoDbContext GetContextWithData()
        {
            var options = new DbContextOptionsBuilder<DemoDbContext>()
                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                              .Options;

            var context = new DemoDbContext(options);

            return context;
        }
    }

    public partial class Entity
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public Child Child { get; set; }

        public string ChildCode { get; set; }
    }

    public partial class Child
    {
        public string Code { get; set; }

        public string Description { get; set; }
    }

    public class DemoDbContext : DbContext
    {
        public DemoDbContext()
        {

        }

        public DemoDbContext(DbContextOptions options) : base(options)
        {

        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("EfDemo");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entity>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.ToTable("offering_descriptors", "app");

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasColumnName("description");

                entity.Property(e => e.ChildCode)
                    .HasColumnName("offering_descriptor_code")
                    .HasMaxLength(4);

                entity.HasOne(e => e.Child)
                    .WithMany()
                    .HasForeignKey(x => x.ChildCode)
                    .HasConstraintName("fk_offering_descriptors_offering_descriptor_code");
            });

            modelBuilder.Entity<Child>(entity =>
            {
                entity.HasKey(e => e.Code)
                    .HasName("pk_id");

                entity.ToTable("child", "lookup");

                entity.Property(e => e.Code)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(64);
            });
        }

        public DbSet<Entity> Entities { get; set; }

        public DbSet<Child> Children { get; set; }
    }
}