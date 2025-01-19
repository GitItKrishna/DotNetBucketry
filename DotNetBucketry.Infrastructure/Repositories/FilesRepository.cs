using DotNetBucketry.Core.Communication.Files;
using DotNetBucketry.Core.Interfaces;
using System.Collections;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;

namespace DotNetBucketry.Infrastructure.Repositories;

public class FilesRepository : IFilesRepository
{
    private IAmazonS3 _s3Client;
    public FilesRepository(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }
    public async Task UploadFilesLowLevelAPI(string bucketName, IFormFile formFile)
    {
        _s3Client = new AmazonS3Client(RegionEndpoint.USEast1);
        List<UploadPartResponse> uploadResponses = new List<UploadPartResponse>();
        InitiateMultipartUploadRequest initiateRequest = new InitiateMultipartUploadRequest
        {
            BucketName = bucketName,
            Key = formFile.FileName
        };
        // Initiate the upload.
        InitiateMultipartUploadResponse initResponse =
            await _s3Client.InitiateMultipartUploadAsync(initiateRequest);
        // Upload parts.
        long contentLength = formFile.Length;
        long partSize = 5 * (long)Math.Pow(2, 20); // 5 MB
         try 
         {
                Console.WriteLine("Uploading parts");
        
                long filePosition = 0;
                for (int i = 1; filePosition < contentLength; i++)
                {
                    UploadPartRequest uploadRequest = new UploadPartRequest
                        {
                            BucketName = bucketName,
                            Key = formFile.FileName,
                            UploadId = initResponse.UploadId,
                            PartNumber = i,
                            PartSize = partSize,
                            FilePosition = filePosition
                        };

                    // Track upload progress.
                    uploadRequest.StreamTransferProgress +=
                        new EventHandler<StreamTransferProgressArgs>(UploadPartProgressEventCallback);

                    // Upload a part and add the response to our list.
                    uploadResponses.Add(await _s3Client.UploadPartAsync(uploadRequest));

                    filePosition += partSize;
                }

                // Setup to complete the upload.
                CompleteMultipartUploadRequest completeRequest = new CompleteMultipartUploadRequest
                    {
                        BucketName = bucketName,
                        Key = formFile.FileName,
                        UploadId = initResponse.UploadId
                     };
                completeRequest.AddPartETags(uploadResponses);

                // Complete the upload.
                CompleteMultipartUploadResponse completeUploadResponse =
                    await _s3Client.CompleteMultipartUploadAsync(completeRequest); 
         }
         catch (Exception exception)
         {
                Console.WriteLine("An AmazonS3Exception was thrown: { 0}", exception.Message);

                // Abort the upload.
                AbortMultipartUploadRequest abortMPURequest = new AbortMultipartUploadRequest
                {
                    BucketName = bucketName,
                    Key = formFile.FileName,
                    UploadId = initResponse.UploadId
                };
               await _s3Client.AbortMultipartUploadAsync(abortMPURequest);
         }
    }
    private static void UploadPartProgressEventCallback(object sender, StreamTransferProgressArgs e)
    {
        // Process event. 
        Console.WriteLine("{0}/{1}", e.TransferredBytes, e.TotalBytes);
    }
    public async Task<AddFileResponse> UploadFiles(string bucketName, IList<IFormFile> formFiles)
    {
        var response = new List<string>();
        foreach (var file in formFiles)
        {
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = file.OpenReadStream(),
                Key = file.FileName,
                BucketName = bucketName,
                CannedACL = S3CannedACL.NoACL
            };
            using(var fileTransferUtility = new TransferUtility(_s3Client))
            {
                await fileTransferUtility.UploadAsync(uploadRequest);
            }

            var urlExpiryRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = file.FileName,
                Expires = DateTime.Now.AddHours(1)
            };
            var url = _s3Client.GetPreSignedURL(urlExpiryRequest);
            response.Add(url);
        }
        return new AddFileResponse
        {
            PreSignedUrls = response
        };
    }
    public async Task<IEnumerable<ListFilesResponse>> ListFiles(string bucketName)
    {
        var response = await _s3Client.ListObjectsAsync(bucketName);
        var listObjectsResponse = response.S3Objects.Select(b=> new ListFilesResponse
        {
            BucketName = b.BucketName,
            Key = b.Key,
            Owner = b.Owner.DisplayName,
            Size = b.Size
        });
        return listObjectsResponse;
    }
    public async Task DownloadFile(string BucketName, string fileName)
    {
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var downloadPath = Path.Combine(desktopPath, "DownloadedFiles");
        
        if(string.IsNullOrEmpty(BucketName) || string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException("BucketName and fileName are required");
        }
        var downloadRequest = new TransferUtilityDownloadRequest
        {
            BucketName = BucketName,
            Key = fileName,
            FilePath = Path.Combine(downloadPath, fileName)
        };
        using(var fileTransferUtility = new TransferUtility(_s3Client))
        {
            await fileTransferUtility.DownloadAsync(downloadRequest);
        }
    }
    public async Task<DeleteFileResponse> DeleteFile(string bucketName, string fileName)
    {
        if(string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentNullException("BucketName and fileName are required");
        }
        var deleteRequest = new DeleteObjectsRequest
        {
            BucketName = bucketName
        };
        deleteRequest.AddKey(fileName);
        var response = await _s3Client.DeleteObjectsAsync(deleteRequest);
        return new DeleteFileResponse
        {
            TotalDeletedFiles = response.DeletedObjects.Count
        };
    }
    public async Task<GetObjectResponse> GetFileForDownloadAsync(string bucketName, string bucketKey, CancellationToken cancellationToken)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = bucketKey
            };
            var response = await _s3Client.GetObjectAsync(request, cancellationToken);
            return response;
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new GetObjectResponse();
        }
    }
}   