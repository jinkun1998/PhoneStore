using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using PhoneStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneStore.Firebase
{
    public class FirebaseHelper
    {
        public string ApiKey = "AIzaSyB9e3hsSJPqZ46yYibyQnsodi-65KaxC_k";
        public FirebaseClient firebaseClient;
        public FirebaseAuthLink auth;
        public FirebaseAuthProvider authProvider;
        public string userEmail = "tiensieqquocthao@gmail.com";
        public string userPw = "123456789";

        public FirebaseHelper()
        {
            LoginWithEmail(userEmail, userPw);
        }

        public void LoginWithEmail(string email, string password)
        {
            authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            auth = authProvider.SignInWithEmailAndPasswordAsync(email, password).Result;
            auth = Task.Run(() => auth.GetFreshAuthAsync()).Result;

            firebaseClient = new FirebaseClient(
            "https://thebossapp-dee9f.firebaseio.com/",
            new FirebaseOptions
            {
                AuthTokenAsyncFactory = async () => (await auth.GetFreshAuthAsync().ConfigureAwait(true)).FirebaseToken
            });
        }

        public async Task<List<ItemModel>> GetAllItem()
        {
            return (await firebaseClient
              .Child("Items")
              .OnceAsync<ItemModel>().ConfigureAwait(true)).Select(item => new ItemModel
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

        public async Task AddUserCart(CartModel cart)
        {
            await firebaseClient
                .Child("Carts")
                .PostAsync(cart).ConfigureAwait(true);
        }

        public async Task<List<CartModel>> GetCartItems()
        {
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

        public async Task<List<CartModel>> GetUserCartItems(string userEmail)
        {
            var carts = await GetCartItems();
            await firebaseClient
              .Child("Carts")
              .OnceAsync<CartModel>().ConfigureAwait(true);
            return carts.Where(it => it.UserEmail == userEmail && it.InOrder == false).ToList();
        }

        public async Task UpdateUserCartItem(CartModel cart)
        {
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
        public async Task DeleteUserCartInOrder(CartModel cart)
        {

            var toDeleteCart = (await firebaseClient
            .Child("Carts")
            .OnceAsync<CartModel>().ConfigureAwait(true)).Where(a => a.Object.UserEmail == cart.UserEmail && a.Object.Code == cart.Code).FirstOrDefault();
            await firebaseClient.Child("Carts").Child(toDeleteCart.Key).DeleteAsync().ConfigureAwait(true);


        }

        public async Task AddUserOrder(OrderModel order)
        {
            await firebaseClient
                .Child("Orders")
                .PostAsync(order).ConfigureAwait(true);
        }
    }
}
