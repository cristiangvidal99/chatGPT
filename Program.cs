using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3;
using Microsoft.Data.SqlClient;


// api key se consigue en la web de openai
string apiKey = "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";

string mensaje = "\"Tengo una base de datos" + "puedes darme una consulta SQL en la tabla Beer, " + "mostrame los campos nombre, marca y precio de forma ordenada \"";

// CREAR OBJETO
var openAiService = new OpenAIService(new OpenAiOptions()
{
    ApiKey = apiKey
});

var completionResult = await openAiService.ChatCompletion.CreateCompletion(
    new ChatCompletionCreateRequest
    {
        Messages = new List<ChatMessage>
        {
           // el rol de user recibe un string
           ChatMessage.FromUser(mensaje)
        },
        Model = Models.ChatGpt3_5Turbo
    });

if (completionResult.Successful)
{
    var response = completionResult.Choices.First().Message.Content;
    var SQL = response.Substring(response.IndexOf("SELECT"));
    Console.WriteLine(SQL);

    var cs = @"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=Pub; Trusted_Connection=True";

    using (SqlConnection connection = new SqlConnection(cs))
    {
        SqlCommand command = new SqlCommand(SQL, connection);
        connection.Open();

        // leer la informacion
        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine(String.Format("{0}, {1}, {2}",
                reader[0], reader[1], reader[2]));
        }
    }
}

