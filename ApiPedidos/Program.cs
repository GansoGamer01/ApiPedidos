using ApiPedidos.BancoDeDados;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var conexao = builder.Configuration.GetConnectionString("conexao");
builder.Services.AddDbContext<PedidoContexto>(config =>
{
    config.UseSqlServer(conexao);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// criação do banco \\
using(var e = app.Services.CreateScope())
{
    var contexto = e.ServiceProvider.GetRequiredService<PedidoContexto>();
    contexto.Database.Migrate();
}

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
