using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Configuration;
using System.IO;
using System.Text;

namespace RedHill.SalesInsight.Web.Html5.Utils
{
    public class AwsUtils
    {
        public string BucketName
        {
            get
            {
                return ConfigurationManager.AppSettings["AWS_BUCKET_NAME"];
            }
        }

        public AmazonS3Client Client
        {
            get
            {
                var awsAccessKey = ConfigurationManager.AppSettings["AWS_ACCESS_KEY"];
                var awsSecretKey = ConfigurationManager.AppSettings["AWS_SECRET_KEY"];
                var client = new AmazonS3Client(awsAccessKey, awsSecretKey, Amazon.RegionEndpoint.USWest1);
                return client;
            }
        }

        public void UploadFile(string path, string key, string contentType)
        {
            TransferUtility utility = new TransferUtility(Client);

            var uploadRequest = new TransferUtilityUploadRequest();
            uploadRequest.FilePath = path;
            uploadRequest.BucketName = BucketName;
            uploadRequest.Key = key;
            uploadRequest.ContentType = contentType;
            uploadRequest.Metadata.Add("x-amz-acl", "public-read");
            if (null != contentType)
                uploadRequest.Metadata.Add("content-type", contentType);

            var resp = Client.PutObject(new PutObjectRequest() { BucketName = BucketName, Key = key, CannedACL = S3CannedACL.PublicReadWrite });

            utility.Upload(uploadRequest);

        }

        private byte[] ConvertToBytes(Stream sObj)
        {
            byte[] b;
            using (BinaryReader br = new BinaryReader(sObj))
            {
                b = br.ReadBytes(Convert.ToInt32(sObj.Length));
            }
            return b;
        }




        public string GetPreSignedURL(string key, DateTime expiresAt)
        {
            string url = null;

            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest()
            {
                BucketName = this.BucketName,
                Key = key,
                Expires = expiresAt
            };

            url = Client.GetPreSignedURL(request);

            return url;
        }

        public string GetQuotePreSignedURL(string key, DateTime expiresAt)
        {
            string url = null;

            if (key != "")
            {
                string subKey = key.Substring(48);
                string[] newKey = subKey.Split('?');
                key = newKey[0];

                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest()
                {
                    BucketName = this.BucketName,
                    Key = key,
                    Expires = expiresAt
                };

                url = Client.GetPreSignedURL(request);
            }
            else
            {
                url = "";
            }

            return url;
        }

        public byte[] DownloadNoteFile(string key)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = key
            };

            byte[] fileBytes;
            using (GetObjectResponse response = Client.GetObject(request))
            {
                using (Stream sr = response.ResponseStream)
                {
                    fileBytes = ConvertToBytes(sr);
                };
            }
            return fileBytes;
        }

        public void DeleteFile(string key)
        {
            if (!string.IsNullOrEmpty(key))
                Client.DeleteObject(new DeleteObjectRequest() { BucketName = BucketName, Key = key });
        }
        public GetObjectResponse GetFile(string key)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = key
            };
            return Client.GetObject(request);
        }

        public static string GenerateKey(params object[] paths)
        {
            if (paths == null)
                return null;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < paths.Length; i++)
            {
                if (i > 0)
                    sb.Append("/");
                sb.Append(paths[i]);
            }
            return sb.ToString();
        }
    }
}