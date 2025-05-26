using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace mecanico_plus.Services
{
    public class InteligenciaArtificial
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string OPENAI_API_URL = "https://api.openai.com/v1/chat/completions"; // Nuevo endpoint

        public InteligenciaArtificial(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["OpenAI:ApiKey"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public class OpenAIResponse
        {
            [JsonPropertyName("choices")]
            public List<Choice> Choices { get; set; }
        }

        public class Choice
        {
            [JsonPropertyName("message")]
            public Message Message { get; set; }
        }

        public class Message
        {
            [JsonPropertyName("content")]
            public string Content { get; set; }
        }

        public async Task<string> GenerateResponse(string contexto)
        {
            try
            {
                // Estructura para chat
                var messages = new List<object>
                {
                    new { 
                        role = "system", 
                        content = "Eres un analista experto. Interpreta estas estadísticas y ofrece insights relevantes en español." 
                    },
                    new { 
                        role = "user", 
                        content = contexto 
                    }
                };

                var requestBody = new 
                {
                    model = "gpt-3.5-turbo",
                    messages = messages,
                    max_tokens = 300,
                    temperature = 0.2, // Más preciso para análisis
                    top_p = 1,
                    frequency_penalty = 0.5
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(OPENAI_API_URL, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"Error: {errorContent}";
                }

                var result = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<OpenAIResponse>(result);
                
                return apiResponse?.Choices?[0]?.Message?.Content?.Trim() 
                    ?? "No se recibió una respuesta válida.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}