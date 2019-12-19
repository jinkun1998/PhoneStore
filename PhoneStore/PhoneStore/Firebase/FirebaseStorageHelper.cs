using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PhoneStore.Firebase
{
    public class FirebaseStorageHelper
    {
        public FirebaseStorage firebaseStorage = new FirebaseStorage("gs://thebossapp-dee9f.appspot.com/");

        public async Task<string> UploadFile(Stream fileStream, string fileName)
        {
            var imageUrl = await firebaseStorage
                .Child("UserAvatars")
                .Child(fileName)
                .PutAsync(fileStream);
            return imageUrl;
        }
    }
}
