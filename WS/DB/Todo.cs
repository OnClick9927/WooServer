﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using WS.Core;

namespace WS.DB
{
    //public class TodoDbContextFactory : IDesignTimeDbContextFactory<TodoDbContext>
    //{
    //    public TodoDbContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<TodoDbContext>();

    //        string dir = $"{AppContext.BaseDirectory}/databases";
    //        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);


    //        optionsBuilder.UseSqlite($"Data Source={dir}/_sqlite.db");




    //        return new TodoDbContext(optionsBuilder.Options);
    //    }
    //}
    public class TodoDbContext : WSDbContext<TodoDbContext>
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        //public TodoDbContext(DbContextOptions options) : base(options)
        //{
        //}

        public DbSet<Todo> Todos { get; set; }
 
        public Todo GetTodo()
        {
            return Todos.FirstOrDefault() ?? new Todo();
        }
    }

    public class Todo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
    }
    class TodoConfig : IEntityTypeConfiguration<Todo>
    {
        public void Configure(EntityTypeBuilder<Todo> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        }
    }
}
