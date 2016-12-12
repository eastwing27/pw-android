using Android.Content;
using Android.Widget;
using PW.DataTransciever;
using PW.DataTransciever.DTO;
using System.Threading.Tasks;

namespace PW.Android
{
    public static class App
    {
        public static Transceiver Service { get; private set; }
        public static double Balance { get; set; }
        public static int UserId { get; private set; }
        public static string UserName { get; private set; }

        public static ReceiverDTO[] Users { get; private set; }
        public static TransactDTO[] Transactions { get; private set; }

        public static string UserInfo => $"{UserName}, {Balance:### ### ##0.00} pws";

        public static void Initialize(string Server)
        {
            Service = new Transceiver(Server);
        }

        /// <summary>
        /// Trying login and get user data. Returns false if something went wrong. Error message placed at Service.StatusMessage
        /// </summary>
        /// <param name="LoginData">JSON string represented Email and password hash</param>
        /// <returns></returns>
        public static async Task<bool> TryLogin(string LoginData)
        {
            UserId = await Service.Login(LoginData);

            if (UserId < 0)
                return false;

            var dto = await Service.GetUserInfo(UserId);

            if (dto == null)
                return false;

            Balance = dto.Balance;
            UserName = dto.Name;
            return true;
        }

        /// <summary>
        /// trying to send register data to current server. Authenticates user and returns true if succeed, returns false if not. Error message placed at Service.StatusMessage
        /// </summary>
        /// <param name="RegData">JSON string represented User name, Email and password hash</param>
        /// <returns></returns>
        public static async Task<bool> TryRegister(string RegData)
        {
            UserId = await Service.Register(RegData);

            if (UserId < 0)
                return false;

            var dto = await Service.GetUserInfo(UserId);

            if (dto == null)
                return false;

            Balance = dto.Balance;
            UserName = dto.Name;
            return true;
        }

        /// <summary>
        /// Updates list of all PW users on current server and place it at Users property
        /// </summary>
        public static async Task UpdateUserList()
        {
            Users = await Service.GetUserList();
        }

        /// <summary>
        /// Updates user transactions history and place it to Transactions property
        /// </summary>
        /// <returns></returns>
        public static async Task UpdateTransactions()
        {
            Transactions = await Service.GetUserTransactions(UserId);
        }

        /// <summary>
        /// Updates entire data such as user balance and current server's user list
        /// </summary>
        /// <returns></returns>
        public static async Task Update(Context CallingContext)
        {
            Users = await Service.GetUserList();
            if (Service.StatusMessage != "ok")
            {
                Toast.MakeText(CallingContext, $"Update error: {Service.StatusMessage}", ToastLength.Short);
                return;
            }

            var dto = await Service.GetUserInfo(UserId);
            if(dto == null)
            {
                Toast.MakeText(CallingContext, $"Update error: {Service.StatusMessage}", ToastLength.Short);
                return;
            }

            UserName = dto.Name;
            Balance = dto.Balance;
        }
    }
}