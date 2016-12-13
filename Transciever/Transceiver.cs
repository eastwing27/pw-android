using Newtonsoft.Json;
using PW.DataTransciever.DTO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PW.DataTransciever
{
    /// <summary>
    /// Use instances of this class to send/receive data from PW server
    /// </summary>
    public class Transceiver
    {
        public string Server { get; set; }
        /// <summary>
        /// Stores status message of last exchange operation
        /// </summary>
        public string StatusMessage { get; private set; }
        private HttpClient client;

        /// <summary>
        /// Initializes new instance and define server address
        /// </summary>
        /// <param name="Server"></param>
        public Transceiver(string Server)
        {
            //User can specify another server for some reason
            this.Server = Server;
            Server.TrimEnd('/');

            if (!Server.ToLower().StartsWith("http://") || !Server.ToLower().StartsWith("https://"))
                Server = $"http://{Server}";

            StatusMessage = "new";

            client = new HttpClient();
        }

        //POST requests
        private async Task<string> Post(string Route, string Body)
        {
            var response = await client.PostAsync($"{Server}/api/{Route}", new StringContent(Body, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                StatusMessage = "ok";
                return await response.Content.ReadAsStringAsync();
            }

            throw new HttpRequestException(response.StatusCode.ToString());
        }

        //GET requests
        private async Task<T> Get<T>(string Route)
        {
            var response = await client.GetAsync($"{Server}/api/{Route}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                StatusMessage = "ok";
                return JsonConvert.DeserializeObject<T>(json);
            }

            throw new HttpRequestException(response.StatusCode.ToString());
        }

        /// <summary>
        /// Get user name and balance using id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<UserDTO> GetUserInfo(int Id)
        {
            try
            {
                return await Get<UserDTO>($"account/{Id}/userinfo");
            }
            catch(HttpRequestException e)
            {
                StatusMessage = "Error getting data";
                return null;
            }
            
        }

        public async Task<double?> GetUserBalance(int Id)
        {
            try
            {
                return await Get<double>($"account/{Id}/balance");
            }
            catch(HttpRequestException e)
            {
                StatusMessage = "Error getting data";
                return null;
            }
        }

        /// <summary>
        /// Gets user list from current server
        /// </summary>
        /// <returns></returns>
        public async Task<ReceiverDTO[]> GetUserList()
        {
            try
            {
                return await Get<ReceiverDTO[]>("account/userlist");
            }
            catch(HttpRequestException e)
            {
                StatusMessage = "Error getting data";
                return null;
            }
        }

        /// <summary>
        /// Get transaction history using user id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<TransactDTO[]> GetUserTransactions(int Id)
        {
            try
            {
                return await Get<TransactDTO[]>($"transact/{Id}/all");
            }
            catch (HttpRequestException e)
            {
                if (e.Message == HttpStatusCode.InternalServerError.ToString())
                    StatusMessage = "Internal Server Error occurred";
                else
                    StatusMessage = "Error getting data";

                return null;
            }
        }

        /// <summary>
        /// Trying to register new user on specified server
        /// </summary>
        /// <param name="RegisterData"></param>
        /// <returns></returns>
        public async Task<int> Register(string RegisterData)
        {
            try
            {
                var Id = await Post("account/register", RegisterData);
                return int.Parse(Id);
            }
            catch(HttpRequestException e)
            {
                if (e.Message == ((HttpStatusCode)422).ToString())
                    StatusMessage = "Your name or e-mail already registered";
                else
                    StatusMessage = "Error occurred. Registration failed";

                return - 1;
            }
        }

        /// <summary>
        /// Trying to authenticate user
        /// </summary>
        /// <param name="LoginData"></param>
        /// <returns></returns>
        public async Task<int> Login(string LoginData)
        {
            try
            {
                var Id = await Post("account/login", LoginData);
                return int.Parse(Id);
            }
            catch (HttpRequestException e)
            {
                if (e.Message == ((HttpStatusCode)422).ToString())
                    StatusMessage = "Check your e-mail and password";
                else
                    StatusMessage = "Error occurred. Login failed";

                return -1;
            }
        }

        public async Task Logout()
        {
            try
            {
                await Post("account/logout", "");
            }
            catch (HttpRequestException)
            {
                StatusMessage = "HTTP error";
            }
        }

        /// <summary>
        /// Trying to send PW to specified user
        /// </summary>
        /// <param name="SendData"></param>
        /// <returns></returns>
        public async Task<bool> Send (string SendData)
        {
            try
            {
                await Post("transact/send", SendData);
                return true;
            }
            catch (HttpRequestException e)
            {
                StatusMessage = "Sending error";
                return false;
            }
        }
    }
}
