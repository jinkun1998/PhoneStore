using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using PhoneStore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Firebase
{
    public class FirebaseHelper
    {
        public static string ApiKey = "AIzaSyB9e3hsSJPqZ46yYibyQnsodi-65KaxC_k";
        public static string userToken;
        public static string userEmail;
        //public string userPw = "123456789";
        public static UserModel user;

        public FirebaseHelper()
        {
            //LoginWithEmail(userEmail, userPw);
            user = new UserModel();
        }

        public void LoginWithEmail(string email, string password)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var auth = authProvider.SignInWithEmailAndPasswordAsync(email, password).Result;
            auth = Task.Run(() => auth.GetFreshAuthAsync()).Result;

            var firebase = new FirebaseClient(
            "https://thebossapp-dee9f.firebaseio.com/",
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = async () => (await auth.GetFreshAuthAsync().ConfigureAwait(true)).FirebaseToken
            });
        }

        public async Task<List<ItemModel>> GetAllItem(string token)
        {
            var firebaseClient = new FirebaseClient(
              "https://thebossapp-dee9f.firebaseio.com/",
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(token)
              });
            return (await firebaseClient
              .Child("Items")
              .OnceAsync<ItemModel>()
              .ConfigureAwait(true)).Select(item => new ItemModel
              {
                  Code = item.Object.Code,
                  Name = item.Object.Name,
                  Image = item.Object.Image,
                  Price = item.Object.Price,
                  Rate = item.Object.Rate,
                  Description = item.Object.Description,
                  DescriptionLink = item.Object.DescriptionLink,
                  Shortdescription = item.Object.Shortdescription,
                  Colors = item.Object.Colors,
              }).ToList();
        }

        public async Task AddUserCart(CartModel cart, string token)
        {
            var firebaseClient = new FirebaseClient(
              "https://thebossapp-dee9f.firebaseio.com/",
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(token)
              });
            await firebaseClient
                .Child("Carts")
                .PostAsync(cart).ConfigureAwait(true);
        }

        public async Task<List<CartModel>> GetCartItems(string token)
        {
            var firebaseClient = new FirebaseClient(
              "https://thebossapp-dee9f.firebaseio.com/",
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(token)
              });
            return (await firebaseClient
              .Child("Carts")
              .OnceAsync<CartModel>().ConfigureAwait(true)).Select(item => new CartModel
              {
                  Code = item.Object.Code,
                  Name = item.Object.Name,
                  Image = item.Object.Image,
                  Price = item.Object.Price,
                  Quantity = item.Object.Quantity,
                  Rate = item.Object.Rate,
                  Description = item.Object.Description,
                  DescriptionLink = item.Object.DescriptionLink,
                  Shortdescription = item.Object.Shortdescription,
                  Color = item.Object.Color,
                  UserEmail = item.Object.UserEmail,
              }).ToList();
        }

        public async Task<List<CartModel>> GetUserCartItems(string userEmail, string token)
        {
            var firebaseClient = new FirebaseClient(
              "https://thebossapp-dee9f.firebaseio.com/",
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(token)
              });
            var carts = await GetCartItems(token);
            await firebaseClient
              .Child("Carts")
              .OnceAsync<CartModel>().ConfigureAwait(true);
            return carts.Where(it => it.UserEmail == userEmail && it.InOrder == false).ToList();
        }

        public async Task UpdateUserCartItem(CartModel cart, string token)
        {
            var firebaseClient = new FirebaseClient(
              "https://thebossapp-dee9f.firebaseio.com/",
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(token)
              });
            var toUpdateItem = (await firebaseClient
              .Child("Carts")
              .OnceAsync<CartModel>().ConfigureAwait(true)).Where(a => a.Object.UserEmail == cart.UserEmail && a.Object.Code == cart.Code).FirstOrDefault();

            await firebaseClient
              .Child("Carts")
              .Child(toUpdateItem.Key)
              .PutAsync(new CartModel()
              {
                  Code = cart.Code,
                  Name = cart.Name,
                  Image = cart.Image,
                  Price = cart.Price,
                  Description = cart.Description,
                  DescriptionLink = cart.DescriptionLink,
                  Shortdescription = cart.Shortdescription,
                  Color = cart.Color,
                  Quantity = cart.Quantity,
                  UserEmail = cart.UserEmail,
                  InOrder = cart.InOrder,
              }).ConfigureAwait(true);
        }
        public async Task DeleteUserCartInOrder(CartModel cart, string token)
        {
            var firebaseClient = new FirebaseClient(
              "https://thebossapp-dee9f.firebaseio.com/",
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(token)
              });
            var toDeleteCart = (await firebaseClient
            .Child("Carts")
            .OnceAsync<CartModel>().ConfigureAwait(true)).Where(a => a.Object.UserEmail == cart.UserEmail && a.Object.Code == cart.Code).FirstOrDefault();
            await firebaseClient.Child("Carts").Child(toDeleteCart.Key).DeleteAsync().ConfigureAwait(true);


        }

        public async Task AddUserOrder(OrderModel order, string token)
        {
            var firebaseClient = new FirebaseClient(
              "https://thebossapp-dee9f.firebaseio.com/",
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(token)
              });
            await firebaseClient
                .Child("Orders")
                .PostAsync(order).ConfigureAwait(true);
        }

        public async Task<UserModel> GetUser(string token)
        {
            var firebaseClient = new FirebaseClient(
              "https://thebossapp-dee9f.firebaseio.com/",
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult(token)
              });
            var allUsers = (await firebaseClient
              .Child("Users")
              .OnceAsync<UserModel>()
              .ConfigureAwait(true)).Select(item => new UserModel
              {
                  Code = item.Object.Code,
                  FullName = item.Object.FullName,
                  Address = item.Object.Address,
                  Email = item.Object.Email,
                  Phone = item.Object.Phone,
                  AvatarLink = item.Object.AvatarLink,
              }).ToList();
            return allUsers.Where(it => it.Email == userEmail).FirstOrDefault();
        }
    }
}
