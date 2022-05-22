using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EnukhinPr16
{
    class FriendsService
    {
        const string Url = "https://192.168.0.9:5000/api/friends"; // обращайте внимание на конечный слеш
        // настройки для десериализации для нечувствительности к регистру символов
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        // настройка клиента
        private HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }

        // получаем всех друзей
        public async Task<IEnumerable<Friend>> Get()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Url);
            HttpContent responseContent = response.Content;
            string result = await responseContent.ReadAsStringAsync();

            return JsonSerializer.Deserialize<IEnumerable<Friend>>(result, options);
        }

        // добавляем одного друга
        public async Task<Friend> Add(Friend friend)
        {
            HttpClient client = GetClient();
            var response = await client.PostAsync(Url,
                new StringContent(
                    JsonSerializer.Serialize(friend),
                    Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonSerializer.Deserialize<Friend>(
                await response.Content.ReadAsStringAsync(), options);
        }
        // обновляем друга
        public async Task<Friend> Update(Friend friend)
        {
            HttpClient client = GetClient();
            var response = await client.PutAsync(Url,
                new StringContent(
                    JsonSerializer.Serialize(friend),
                    Encoding.UTF8, "application/json"));

            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonSerializer.Deserialize<Friend>(
                await response.Content.ReadAsStringAsync(), options);
        }
        // удаляем друга
        public async Task<Friend> Delete(int id)
        {
            HttpClient client = GetClient();
            var response = await client.DeleteAsync(Url + id);
            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonSerializer.Deserialize<Friend>(
               await response.Content.ReadAsStringAsync(), options);
        }

    }
}
