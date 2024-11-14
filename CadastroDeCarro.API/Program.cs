using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Corrigindo a string de conexão para o banco de dados correto
string connectionString = "Server=YVARLAN\\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
app.MapGet("/Carro", async () =>
{
    List<Carro> carros = new List<Carro>();

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string query = "SELECT Id, Marca, Ano, Cor, Preco, Quilometragem, Tipo_de_combustivel, Numero_de_portas FROM Carro";
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            await connection.OpenAsync();

            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    carros.Add(new Carro
                    {
                        Id = reader.GetInt32(0),
                        Marca = reader.GetString(1),
                        Ano = reader.GetInt32(2),
                        Cor = reader.GetString(3),
                        Preco = reader.GetDecimal(4),
                        Quilometragem = reader.GetInt32(5),
                        Tipo_de_combustivel = reader.GetString(6),
                        Numero_de_portas = reader.GetInt32(7)
                    });
                }
            }
        }
    }

    return Results.Ok(carros);
});

app.MapGet("/carros/{id:int}", async (int id) =>
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string query = "SELECT Id, Marca, Ano, Cor, Preco, Quilometragem, Tipo_de_combustivel, Numero_de_portas FROM Carro WHERE Id = @Id";
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Id", id);  // Corrigido o nome do parâmetro

            await connection.OpenAsync();

            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    var carro = new Carro
                    {
                        Id = reader.GetInt32(0),
                        Marca = reader.GetString(1),
                        Ano = reader.GetInt32(2),
                        Cor = reader.GetString(3),
                        Preco = reader.GetDecimal(4),
                        Quilometragem = reader.GetInt32(5),
                        Tipo_de_combustivel = reader.GetString(6),
                        Numero_de_portas = reader.GetInt32(7)
                    };

                    return Results.Ok(carro);
                }
            }
        }
    }

    return Results.NotFound();
});

app.MapPost("/carros", async (Carro carro) =>
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string query = "INSERT INTO Carro (Marca, Ano, Cor, Preco, Quilometragem, Tipo_de_combustivel, Numero_de_portas) " +
                       "VALUES (@Marca, @Ano, @Cor, @Preco, @Quilometragem, @Tipo_de_combustivel, @Numero_de_portas);";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Marca", carro.Marca);
            command.Parameters.AddWithValue("@Ano", carro.Ano);
            command.Parameters.AddWithValue("@Cor", carro.Cor);
            command.Parameters.AddWithValue("@Preco", carro.Preco);
            command.Parameters.AddWithValue("@Quilometragem", carro.Quilometragem);
            command.Parameters.AddWithValue("@Tipo_de_combustivel", carro.Tipo_de_combustivel);
            command.Parameters.AddWithValue("@Numero_de_portas", carro.Numero_de_portas);

            await connection.OpenAsync();

            int id = Convert.ToInt32(await command.ExecuteScalarAsync());  // Usando ExecuteScalar para obter o ID gerado
            return Results.Created($"/carros/{id}", carro);
        }
    }
});

app.MapPut("/carros/{id:int}", async (int id, Carro carro) =>
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string query = "UPDATE Carro SET Marca = @Marca, Ano = @Ano, Cor = @Cor, Preco = @Preco, Quilometragem = @Quilometragem, " +
                       "Tipo_de_combustivel = @Tipo_de_combustivel, Numero_de_portas = @Numero_de_portas WHERE Id = @Id;";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Marca", carro.Marca);
            command.Parameters.AddWithValue("@Ano", carro.Ano);
            command.Parameters.AddWithValue("@Cor", carro.Cor);
            command.Parameters.AddWithValue("@Preco", carro.Preco);
            command.Parameters.AddWithValue("@Quilometragem", carro.Quilometragem);
            command.Parameters.AddWithValue("@Tipo_de_combustivel", carro.Tipo_de_combustivel);
            command.Parameters.AddWithValue("@Numero_de_portas", carro.Numero_de_portas);

            await connection.OpenAsync();

            int rowsAffected = await command.ExecuteNonQueryAsync();  // Atualiza o carro no banco
            return rowsAffected > 0 ? Results.NoContent() : Results.NotFound();  // Se nenhuma linha for afetada, retorna NotFound
        }
    }
});

app.MapDelete("/carros/{id:int}", async (int id) =>
{
    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        string query = "DELETE FROM Carro WHERE Id = @Id";
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Id", id);

            await connection.OpenAsync();

            int rowsAffected = await command.ExecuteNonQueryAsync();  // Exclui o carro do banco
            return rowsAffected > 0 ? Results.NoContent() : Results.NotFound();  // Se nenhuma linha for afetada, retorna NotFound
        }
    }
});

app.Run();

public class Carro
{
    public int Id { get; set; }
    public string Marca { get; set; }
    public int Ano { get; set; }
    public string Cor { get; set; }
    public decimal Preco { get; set; }
    public int Quilometragem { get; set; }
    public string Tipo_de_combustivel { get; set; }
    public int Numero_de_portas { get; set; }
}