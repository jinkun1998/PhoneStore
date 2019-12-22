
using Firebase.Database;
using Firebase.Database.Query;
using PhoneStore.Models;
using Plugin.FirebaseAuth;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStore.Firebase
{
    public class FirebaseHelper
    {
        public FirebaseHelper()
        {
            //LoginWithEmail(userEmail, userPw);
        }

        #region Rotators
        public async Task<List<RotatorModel>> GetRotators()
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            return (await firebaseClient
              .Child("Rotators")
              .OnceAsync<RotatorModel>()
              .ConfigureAwait(true)).Select(item => new RotatorModel
              {
                  Image = item.Object.Image,
              }).ToList();
        }
        #endregion

        #region Items
        #region FavoriteItems
        public async Task<List<ItemModel>> GetAllFavoriteItems()
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            return (await firebaseClient
              .Child("Favorites")
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
                  RotatorImages = item.Object.RotatorImages,
                  UserEmail = item.Object.UserEmail,
                  CreatedDate = item.Object.CreatedDate,
                  Type = item.Object.Type,
              }).ToList();
        }

        public async Task AddUserFavoriteItem(ItemModel item)
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            await firebaseClient
                .Child("Favorites")
                .PostAsync(item).ConfigureAwait(true);
        }

        public async Task DeleteUserFavoriteItem(ItemModel item)
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            var toDeleteCart = (await firebaseClient
            .Child("Favorites")
            .OnceAsync<CartModel>().ConfigureAwait(true)).Where(a => a.Object.Code == item.Code && a.Object.UserEmail == item.UserEmail).FirstOrDefault();
            await firebaseClient.Child("Favorites").Child(toDeleteCart.Key).DeleteAsync().ConfigureAwait(true);
        }
        #endregion

        public async Task<List<ItemModel>> GetAllItems()
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
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
                  RotatorImages = item.Object.RotatorImages,
                  CreatedDate = item.Object.CreatedDate,
                  Type = item.Object.Type,
              }).ToList();
        }
        #endregion

        #region CartItems
        public async Task AddUserCart(CartModel cart)
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            await firebaseClient
                .Child("Carts")
                .PostAsync(cart).ConfigureAwait(true);
        }

        public async Task<List<CartModel>> GetCartItems()
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
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
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            var carts = await GetCartItems();
            await firebaseClient
              .Child("Carts")
              .OnceAsync<CartModel>().ConfigureAwait(true);
            return carts.Where(it => it.UserEmail == userEmail && it.InOrder == false).ToList();
        }

        public async Task UpdateUserCartItem(CartModel cart)
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
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
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            var toDeleteCart = (await firebaseClient
            .Child("Carts")
            .OnceAsync<CartModel>().ConfigureAwait(true)).Where(a => a.Object.UserEmail == cart.UserEmail && a.Object.Code == cart.Code).FirstOrDefault();
            await firebaseClient.Child("Carts").Child(toDeleteCart.Key).DeleteAsync().ConfigureAwait(true);
        }
        #endregion

        #region User
        public async Task<List<UserModel>> GetAllUsers()
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            return (await firebaseClient
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
                  DoB = item.Object.DoB,
              }).ToList();
        }
        public async Task<UserModel> GetUser(string userEmail)
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
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
                  DoB = item.Object.DoB,
              }).ToList();
            return allUsers.Where(it => it.Email == userEmail).FirstOrDefault();
        }

        public async Task AddUser(UserModel user)
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            await firebaseClient
                .Child("Users")
                .PostAsync(user).ConfigureAwait(true);
        }

        public async Task UpdateUser(UserModel user)
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            var toUpdateItem = (await firebaseClient
              .Child("Users")
              .OnceAsync<UserModel>().ConfigureAwait(true)).Where(a => a.Object.Email == user.Email && a.Object.Code == user.Code).FirstOrDefault();

            await firebaseClient
              .Child("Users")
              .Child(toUpdateItem.Key)
              .PutAsync(new UserModel()
              {
                  Email = user.Email,
                  Address = user.Address,
                  AvatarLink = user.AvatarLink,
                  Code = user.Code,
                  FullName = user.FullName,
                  Phone = user.Phone,
                  DoB = user.DoB,
              })
            .ConfigureAwait(true);
        }
        #endregion

        #region Order
        public async Task AddUserOrder(OrderModel order)
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            await firebaseClient
                .Child("Orders")
                .PostAsync(order).ConfigureAwait(true);
        }

        public async Task DeleteUserOrder(OrderModel order)
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            await firebaseClient
                .Child("Orders")
                .PostAsync(order).ConfigureAwait(true);
        }

        public async Task<List<OrderModel>> GetAllOrders()
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            return (await firebaseClient
              .Child("Orders")
              .OnceAsync<OrderModel>().ConfigureAwait(true)).Select(item => new OrderModel
              {
                  Code = item.Object.Code,
                  UserEmail = item.Object.UserEmail,
                  Address = item.Object.Address,
                  Carts = item.Object.Carts,
                  CreatedOn = item.Object.CreatedOn,
                  Note = item.Object.Note,
                  Payment = item.Object.Payment,
                  Status = item.Object.Status,
                  TotalPrice = item.Object.TotalPrice,
                  Shipments = item.Object.Shipments,
              }).ToList();
        }

        public async Task<List<OrderModel>> GetAllUserOrders(string userEmail)
        {
            var allOrders = GetAllOrders().Result;
            return allOrders.Where(it => it.UserEmail == userEmail && it.Status != OrderModel.OrderStatus.Cancelled).ToList();
        }

        public async Task UpdateUserOrder(OrderModel order)
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            var toUpdateItem = (await firebaseClient
              .Child("Orders")
              .OnceAsync<OrderModel>().ConfigureAwait(true)).Where(a => a.Object.UserEmail == order.UserEmail && a.Object.Code == order.Code).FirstOrDefault();

            await firebaseClient
              .Child("Orders")
              .Child(toUpdateItem.Key)
              .PutAsync(new OrderModel()
              {
                  Code = order.Code,
                  Address = order.Address,
                  Carts = order.Carts,
                  CreatedOn = order.CreatedOn,
                  Note = order.Note,
                  Payment = order.Payment,
                  Shipments = order.Shipments,
                  Status = order.Status,
                  TotalPrice = order.TotalPrice,
                  UserEmail = order.UserEmail,
              }).ConfigureAwait(true);
        }
        #endregion

        #region SignIn, SignOut
        #endregion

        #region Promo
        public async Task AddUserPromo(QRPromoModel promo)
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            await firebaseClient
                .Child("Promos")
                .PostAsync(promo).ConfigureAwait(true);
        }
        public async Task<List<QRPromoModel>> GetAllPromos()
        {
            var firebaseClient = new FirebaseClient("https://thebossapp-dee9f.firebaseio.com/");
            return (await firebaseClient
              .Child("Promos")
              .OnceAsync<QRPromoModel>().ConfigureAwait(true)).Select(item => new QRPromoModel
              {
                  UserEmail = item.Object.UserEmail,
                  QREmail = item.Object.QREmail,
                  IsUsed = item.Object.IsUsed,
                  Discount = item.Object.Discount,
              }).ToList();
        }
        public async Task<List<QRPromoModel>> GetAllUserPromo(string userEmail)
        {
            var allOrders = GetAllPromos().Result;
            return allOrders.Where(it => it.UserEmail == userEmail && it.IsUsed == false).ToList();
        }
        #endregion
    }
}
